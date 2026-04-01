using SvigermorApi.Core;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;

namespace SvigermorApi.Api
{
    public class GeminiTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        
        public GeminiTranslationService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<TranslationResponse> Translate(TranslationRequest request)
        {
            string promptPath = _config["Gemini:PromptPath"];
            string prompt = await File.ReadAllTextAsync(promptPath);

            var body = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = prompt } }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = request.Input } }
                    }
                }
            };

            string json = JsonSerializer.Serialize(body);

            var key = _config["GEMINI_API_KEY"];
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("GEMINI_API_KEY is not configured.");

            var baseUrl = _config["Gemini:BaseUrl"]?.TrimEnd('/')
                ?? throw new InvalidOperationException("Gemini:BaseUrl is not configured.");

            var model = _config["Gemini:Model"]
                ?? throw new InvalidOperationException("Gemini:Model is not configured.");

            string url = $"{baseUrl}/v1beta/models/{model}:generateContent?key={key}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            string responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Gemini API returned {(int)response.StatusCode}: {responseString}",
                    null,
                    response.StatusCode);

            JsonDocument doc = JsonDocument.Parse(responseString);

            string text;
            try
            {
                text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString()
                    ?? throw new InvalidOperationException("Gemini response 'text' field was null.");
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected Gemini response shape: {responseString}", ex);
            }

            return new TranslationResponse(text);

        }
    }
}