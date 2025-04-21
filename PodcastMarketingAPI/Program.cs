using PodcastMarketingAPI;
using PodcastMarketingAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Podcast Marketing Instance
var podcastMarketing = new PodcastMarketing(
    "\"https://your-azure-openai-resource.com\"", 
    "gpt-4.1", 
    Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

//Define Endpoints
app.MapGet("/GetTranscription/{podcastUrl}", async (string podcastUrl) =>
{
    var transcription = await podcastMarketing.GetTranscription(podcastUrl);
    return Results.Ok(new TranscriptionResponse { Transcription = transcription });
})
    .WithName("GetTranscription")
    .WithSummary("Retrieve Podcast Transcription")
    .WithDescription("Generates a transcription of the podcast episode based on the provided podcast URL.")
    .WithOpenApi();

app.MapGet("/GetGuestName/{transcription}", async (string transcription) =>
{
    var guestName = await podcastMarketing.GetGuestName(transcription);
    return Results.Ok(new GuestNameResponse { GuestName = guestName });
})
    .WithName("GetGuestName")
    .WithSummary("Extract Guest Name")
    .WithDescription("Extracts the name of the guest from the podcast transcript.")
    .WithOpenApi();

app.MapGet("/GetGuestBio/{guestName}", async (string guestName) =>
{
    var guestBio = await podcastMarketing.GetGuestBio(guestName);
    return Results.Ok(new GuestBioResponse { GuestBio = guestBio });
})
    .WithName("GetGuestBio")
    .WithSummary("Retrieve Guest Bio")
    .WithDescription("Retrieves the guest's biography based on their name.")
    .WithOpenApi();

app.MapGet("/GetSocialMediaBlurb/{transcription}/{guestBio}", async (string transcription, string guestBio) =>
{
    var socialBlurb = await podcastMarketing.GetSocialMediaBlurb(transcription, guestBio);
    return Results.Ok(new SocialMediaBlurbResponse { SocialMediaBlurb = socialBlurb });
})
    .WithName("GetSocialMediaBlurb")
    .WithSummary("Generate Social Media Blurb")
    .WithDescription("Creates a compelling social media blurb for the podcast episode using the transcript and guest bio.")
    .WithOpenApi();

app.MapGet("/GetDallEPrompt/{socialMediaBlurb}", async (string socialMediaBlurb) =>
{
    var dallePrompt = await podcastMarketing.GetDallEPrompt(socialMediaBlurb);
    return Results.Ok(new DallEPromptResponse { DallEPrompt = dallePrompt });
})
    .WithName("GetDallEPrompt")
    .WithSummary("Generate DALL-E Prompt")
    .WithDescription("Generates a relevant DALL-E prompt based on the social media blurb.")
    .WithOpenApi();

app.MapGet("/GetImage/{dallePrompt}", async (string dallePrompt) =>
{
    var imageUrl = await podcastMarketing.GetImage(dallePrompt);
    return Results.Ok(new ImageResponse { ImageUrl = imageUrl });
})
    .WithName("GetImage")
    .WithSummary("Generate Image with DALL-E")
    .WithDescription("Creates an image for the social media post using the generated prompt.")
    .WithOpenApi();

app.Run();
