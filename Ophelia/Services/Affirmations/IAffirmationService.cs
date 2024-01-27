using Ophelia.Data.Models;
using Ophelia.Models.Affirmations;

namespace Ophelia.Services;

public interface IAffirmationService
{
    Task<List<Affirmation>> GetAllAffirmationsAsync();

    Task<List<Affirmation>> GetAffirmationsByIds(List<string> ids);

    Task<List<Affirmation>> GetAffirmationsByCategory(string category);

    Task<Affirmation> GetAffirmationById(string id);

    Task<List<UserAffirmation>> GetAffirmationsByUser(string userId);

    Task<Affirmation> GetDailyAffirmation();
    
    Task<List<Affirmation>> PostGenerateAffirmations(string prompt, bool generateTts);
    
    Task<Affirmation> PostGenerateSingleAffirmation(string prompt, bool generateTts);

    Task PostAddUserAffirmation(string userId, string affirmationText, string category);
    
    Task AssociateAffirmationWithUser(string userId, string affirmationId);

    Task DissociateAffirmationFromUser(string userId, string affirmationId);
}