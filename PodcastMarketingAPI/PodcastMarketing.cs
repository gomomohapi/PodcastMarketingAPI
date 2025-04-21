using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Responses;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Images;
using System.Web;

namespace PodcastMarketingAPI
{
    public class PodcastMarketing
    {
        private readonly AzureOpenAIClient _azureOpenAIClient;
        private readonly OpenAIResponseClient _openAIResponseClient;

        public PodcastMarketing(string azureOpenAIEndpoint, string openAIModel, string openAIKey)
        {
            //Azure OpenAI Client
            _azureOpenAIClient = new AzureOpenAIClient(
                new Uri(azureOpenAIEndpoint),
                new DefaultAzureCredential());

            //OpenAIResponseClient
            _openAIResponseClient = new OpenAIResponseClient(
                model: openAIModel,
                apiKey: openAIKey);
        }

        //Get Audio Transcription
        public async Task<string> GetTranscription(string podcastUrl)
        {
            var decodedUrl = HttpUtility.UrlDecode(podcastUrl);

            using HttpClient httpClient = new();
            Stream audioStreamFromBlob = await httpClient.GetStreamAsync(decodedUrl);

            AudioClient audioClient =
                _azureOpenAIClient.GetAudioClient("whisper");
            AudioTranscription audioTranscription =
                await audioClient.TranscribeAudioAsync(audioStreamFromBlob, "file.mp3");

            return audioTranscription.Text;
        }

        //Extract Guest Name from transcription
        public async Task<string> GetGuestName(string transcription)
        {
            ChatClient client = _azureOpenAIClient.GetChatClient("gpt4.1");

            ChatCompletion chatCompletion = await client.CompleteChatAsync(
             [
                new SystemChatMessage(
                    "Extract only the guest name on the Beyond the Tech podcast " +
                    "from the following transcript. Beyond the Tech is hosted by " +
                    "Kevin Scott, so Kevin Scott will never be the guest."),
                new UserChatMessage(transcription)
             ]);

            return chatCompletion.Content[0].Text;
        }

        //Get Guest Bio from OpenAI Responses API
        public async Task<string?> GetGuestBio(string guestName)
        {
            OpenAIResponse response = await _openAIResponseClient.CreateResponseAsync(
                userInputText: $"Please retrieve a medium-length, descriptive bio for {guestName}",
                new ResponseCreationOptions()
                {
                    Tools = { ResponseTool.CreateWebSearchTool() }
                });

            var message = response.OutputItems
            .OfType<MessageResponseItem>()
            .FirstOrDefault();

            return message?.Content?.FirstOrDefault()?.Text;
        }

        //Create Social Media Blurb
        public async Task<string> GetSocialMediaBlurb(string transcription, string bio)
        {
            ChatClient client = _azureOpenAIClient.GetChatClient("gpt4.1");

            ChatCompletion chatCompletion = await client.CompleteChatAsync(
            [
                new SystemChatMessage("You are a helpful large language model that " +
                "can create a LinkedIn promo blurb for episodes of the podcast " +
                "Behind the Tech, when given transcripts of the podcasts. The Behind " +
                "the Tech podcast is hosted by Kevin Scott."),
                new UserChatMessage("Create a short summary of this podcast episode " +
                "that would be appropriate to post on LinkedIn to promote the " +
                "podcast episode. The post should be from the first-person " +
                "perspective of Kevin Scott, who hosts the podcast. \n" +
                    $"Here is the transcript of the podcast episode: {transcription} \n" +
                    $"Here is the bio of the guest: {bio}")
                    ]);

            return chatCompletion.Content[0].Text;
        }

        //Generate Dall-E Prompt
        public async Task<string> GetDallEPrompt(string socialBlurb)
        {
            ChatClient client = _azureOpenAIClient.GetChatClient("gpt4.1");

            ChatCompletion chatCompletion = await client.CompleteChatAsync(
            [
                new SystemChatMessage("You are a helpful large language model that " +
                    "generates DALL-E prompts, that when given to the DALL-E model can " +
                    "generate beautiful high-quality images to use in social media " +
                    "posts about a podcast on technology. Good DALL-E prompts will " +
                    "contain mention of related objects, and will not contain people, " +
                    "faces, or words. Good DALL-E prompts should include a reference " +
                    "to podcasting along with items from the domain of the podcast " +
                    "guest."),
                    new UserChatMessage($"Create a DALL-E prompt to create an image to " +
                    $"post along with this social media text: {socialBlurb}")
            ]);

            return chatCompletion.Content[0].Text;
        }

        //Generate social media image with Dall-E
        public async Task<string> GetImage(string prompt)
        {
            ImageClient client = _azureOpenAIClient.GetImageClient("dalle3");

            ImageGenerationOptions options = new()
            {
                Quality = GeneratedImageQuality.High,
                Size = GeneratedImageSize.W1024xH1024,
                Style = GeneratedImageStyle.Vivid,
                ResponseFormat = GeneratedImageFormat.Uri,
            };

            GeneratedImage image = await client.GenerateImageAsync(prompt +
                ", high-quality digital art", options);

            return image.ImageUri.ToString();
        }
    }
}