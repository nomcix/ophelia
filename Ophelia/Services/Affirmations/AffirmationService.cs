using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Ophelia.Data;
using Ophelia.Data.Models;
using Ophelia.Models.Affirmations;
using Ophelia.OpenAI;
using Ophelia.Services.Aws;

namespace Ophelia.Services.Affirmations;

internal class AffirmationService : IAffirmationService
{
    private readonly IAffirmationRepository _repository;
    private readonly IOpenAIService _openAiService;
    private readonly IS3Service _s3Service;
    private readonly ILogger<AffirmationService> _logger;

    public AffirmationService(
        IAffirmationRepository repository,
        IOpenAIService openAiService,
        ILogger<AffirmationService> logger,
        IS3Service s3Service)
    {
        _repository = repository;
        _openAiService = openAiService;
        _s3Service = s3Service;
        _logger = logger;
    }

    public async Task<List<Affirmation>> GetAllAffirmationsAsync()
    {
        try
        {
            var affirmations = await _repository.GetAllAffirmations().ConfigureAwait(false);
            return affirmations.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAllAffirmationsAsync");
            throw;
        }
    }
    
    public async Task<List<Affirmation>> GetAffirmationsByIds(List<string> ids)
    {
        try
        {
            if (ids == null || !ids.Any())
            {
                throw new ArgumentException("The list of IDs cannot be null or empty.");
            }

            var affirmations = await _repository.GetAffirmationsByIds(ids).ConfigureAwait(false);
            return affirmations.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetAffirmationsByIdsAsync");
            throw;
        }
    }

    public async Task<List<Affirmation>> GetAffirmationsByCategory(string category)
    {
        try
        {
            return await _repository.GetAffirmationsByCategory(category).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred in GetAffirmationsByCategory with category: {category}");
            throw;
        }
    }

    public async Task<Affirmation> GetAffirmationById(string id)
    {
        try
        {
            return await _repository.GetAffirmationById(id).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred in GetAffirmationById with ID: {id}");
            throw;
        }
    }

    public async Task<List<UserAffirmation>> GetAffirmationsByUser(string userId)
    {
        try
        {
            return await _repository.GetAffirmationsByUser(userId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $@"Error occurred in GetAffirmationsByUser for user: {userId}");
            throw;
        }
    }

    public async Task PostAddUserAffirmation(string userId, string affirmationText, string category)
    {
        try
        {
            var affirmation = new Affirmation
            {
                id = ComputeSha256Hash(affirmationText)[..32],
                user_id = new Guid(userId),
                affirmation = affirmationText,
                category = category,
                system = false
            };

            await _repository.InsertAffirmation(affirmation).ConfigureAwait(false);

            await _repository.AssociateAffirmationWithUser(userId, affirmation.id)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred adding affirmation for user: {userId}");
        }
    }

    public async Task<List<Affirmation>> PostGenerateAffirmations(string prompt, bool generateTts)
    {
        try
        {
            var content = await GetChatCompletionResponse(prompt, 1.0).ConfigureAwait(false);
            
            AffirmationsContainer? container;
            
            try
            {
                container = JsonConvert.DeserializeObject<AffirmationsContainer>(content);
                
                if (container == null)
                {
                    throw new ApplicationException("Failed to generate Affirmations");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to de-serialize chat completion content.");
                throw;
            }
            
            var affirmationItems = container.Affirmations.Select(x => new Affirmation
            {
                affirmation = x,
                id = ComputeSha256Hash(x)[..32],
                category = "generated",
                system = false
            }).ToList();

            // only insert on save?
            try
            {
                await _repository.InsertAffirmations(affirmationItems).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,"Error occurred inserting affirmations.");
                throw;
            }

            if (generateTts)
            {
                foreach (var affirmation in affirmationItems)
                {
                    var mp3Data = await _openAiService.GenerateSpeechAsync(affirmation.affirmation).ConfigureAwait(false);
                    try
                    {
                        using var mp3DataStream = new MemoryStream(mp3Data);
                        await _s3Service.PutObjectAsync("ophelia", $"generated/{affirmation.id}.mp3", mp3DataStream, "audio/mpeg").ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while uploading TTS MP3 to S3");
                        throw;
                    }
                }
            }

            return affirmationItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in PostGenerateAffirmations");
            throw;
        }
    }

    public async Task<Affirmation> PostGenerateSingleAffirmation(string prompt, bool generateTts)
    {
        try
        {
            var content = await GetChatCompletionResponse(prompt, 1.2).ConfigureAwait(false);
            
            GeneratedAffirmation? generatedAffirmation;
            
            try
            {
                generatedAffirmation = JsonConvert.DeserializeObject<GeneratedAffirmation>(content);
                
                if (generatedAffirmation == null)
                {
                    throw new ApplicationException("Failed to generate Affirmation");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to de-serialize chat completion content.");
                throw;
            }

            var affirmationItem = new Affirmation
            {
                affirmation = generatedAffirmation.Affirmation,
                id = ComputeSha256Hash(generatedAffirmation.Affirmation)[..32],
                category = "generated",
                system = false
            };

            // only insert on save?
            try
            {
                await _repository.InsertAffirmation(affirmationItem).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,"Error occurred inserting affirmations.");
                throw;
            }

            if (generateTts)
            {
                var mp3Data = await _openAiService.GenerateSpeechAsync(affirmationItem.affirmation).ConfigureAwait(false);
                try
                {
                    using var mp3DataStream = new MemoryStream(mp3Data);
                    await _s3Service.PutObjectAsync("ophelia", $"generated/{affirmationItem.id}.mp3", mp3DataStream, "audio/mpeg").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while uploading TTS MP3 to S3");
                    throw;
                }
            }

            return affirmationItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in PostGenerateSingleAffirmation");
            throw;
        }
    }

    public async Task AssociateAffirmationWithUser(string userId, string affirmationId)
    {
        try
        {
            await _repository.AssociateAffirmationWithUser(userId, affirmationId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured in AssociateAffirmationWithUser");
            throw;
        }
    }

    public async Task DissociateAffirmationFromUser(string userId, string affirmationId)
    {
        try
        {
            await _repository.DissociateAffirmationFromUser(userId, affirmationId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured in DissociateAffirmationFromUser");
            throw;
        }
    }

    private async Task<string> GetChatCompletionResponse(string prompt, double temperature)
    {
        var chatCompletionResponse = await _openAiService.GetResponseAsync(prompt, temperature).ConfigureAwait(false);
        var chatCompletion = DeserializeChatCompletionResponse(chatCompletionResponse);
        var content = chatCompletion.Choices.First().Message.Content;
        return content;
    }

    private static ChatCompletionResponse DeserializeChatCompletionResponse(string json)
    {
        return JsonConvert.DeserializeObject<ChatCompletionResponse>(json) ??
               throw new InvalidOperationException("Failed to deserialize chat completion response.");
    }

    private static string ComputeSha256Hash(string affirmationContent)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(affirmationContent));

        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}
