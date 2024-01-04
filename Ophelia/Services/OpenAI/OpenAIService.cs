using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Ophelia.Services.Aws;

namespace Ophelia.Services.OpenAI
{
    internal class OpenAIService : IOpenAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IParameterStoreService _parameterStoreService;
        private readonly ILogger<OpenAIService> _logger;
        private string _cachedApiKey;

        public OpenAIService(IHttpClientFactory httpClientFactory, 
                             IParameterStoreService parameterStoreService, 
                             ILogger<OpenAIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _parameterStoreService = parameterStoreService;
            _logger = logger;
        }

        public async Task<string> GetResponseAsync(string prompt, double temperature)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_cachedApiKey))
                {
                    _cachedApiKey = await _parameterStoreService.GetParameterValueAsync("ophelia-dev-openai-api-key");
                }

                var httpClient = _httpClientFactory.CreateClient();

                var requestData = new
                {
                    model = "gpt-4-1106-preview",
                    response_format = new { type = "json_object" },
                    messages = new[] { new { role = "user", content = prompt } },
                    temperature = temperature,
                    max_tokens = 300,
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedApiKey);

                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error in OpenAI GetResponseAsync");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in OpenAI GetResponseAsync");
                throw;
            }
        }
        
        public async Task<byte[]> GenerateSpeechAsync(string text, string voice = "alloy")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_cachedApiKey))
                {
                    _cachedApiKey = await _parameterStoreService.GetParameterValueAsync("ophelia-dev-openai-api-key");
                }

                var httpClient = _httpClientFactory.CreateClient();

                var requestData = new
                {
                    model = "tts-1",
                    input = text,
                    voice = voice
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedApiKey);

                var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/speech", requestContent);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error in OpenAI GenerateSpeechAsync");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in OpenAI GenerateSpeechAsync");
                throw;
            }
        }
    }
}
