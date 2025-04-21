namespace PodcastMarketingAPI.Models
{
    public class TranscriptionResponse
    {
        public required string Transcription { get; set; }
    }

    public class GuestNameResponse
    {
        public required string GuestName { get; set; }
    }

    public class GuestBioResponse
    {
        public required string GuestBio { get; set; }
    }

    public class SocialMediaBlurbResponse
    {
        public required string SocialMediaBlurb { get; set; }
    }

    public class DallEPromptResponse
    {
        public required string DallEPrompt { get; set; }
    }

    public class ImageResponse
    {
        public required string ImageUrl { get; set; }
    }
}
