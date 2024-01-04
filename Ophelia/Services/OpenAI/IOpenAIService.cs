namespace Ophelia.Services;

public interface IOpenAIService
{
    Task<string> GetResponseAsync(string prompt, double temperature);

    Task<byte[]> GenerateSpeechAsync(string text, string voice = "alloy");
}
