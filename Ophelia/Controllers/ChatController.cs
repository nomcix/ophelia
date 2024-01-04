using Microsoft.AspNetCore.Mvc;
using Ophelia.Services;

namespace Ophelia.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IOpenAIService _openAIService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IOpenAIService openAIService, ILogger<ChatController> logger)
    {
        _openAIService = openAIService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> GetResponse([FromBody] string prompt)
    {
        _logger.LogInformation("Received chat request with prompt: {Prompt}", prompt);
        try
        {
            var response = await _openAIService.GetResponseAsync(prompt, 1.0);
            _logger.LogInformation("Sending response to client");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing chat request");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpPost("tts")]
    public async Task<IActionResult> GenerateTts(string text)
    {
        try
        {
            var mp3Data = await _openAIService.GenerateSpeechAsync(text).ConfigureAwait(false);
            return File(mp3Data, "audio/mpeg", "speech.mp3");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing tts request");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}