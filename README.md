
# PodcastMarketingAPI

This is a sample .NET Web API Project. This API allows you to enter a link to your podcast and then it generates a promotional social media image and blub using the Azure OpenAI Service. 

The full demonstration can be found in this YouTube video: [Build your own agent with OpenAI, .NET, and Copilot Studio](https://www.youtube.com/live/CiyH6nL6knc)

### How it works

[picture]

 1. Starting from the podcast URL, you get the podcast transcription with the Whisper model
 2. Given that transcript, you use GPT 4.1 to extract the name of the guest
 3. With the guest's name, you retrieve their bio using the OpenAI Responses API (with the Web Search tool)
 4. With the transcription and the guest bio, you generate a social media blurb with GPT 4.1
 5. With the social media blurb, you generate a relevant DALL-E prompt with GPT 4.1
 6. Finally, you use DALL-E 3 to generate an image for the social media post with the prompt

## Disclaimer

This .NET project takes direct inspiration from Kevin Scott’s Microsoft Build 2023 Python demo (https://github.com/microsoft/podcastcopilot).
