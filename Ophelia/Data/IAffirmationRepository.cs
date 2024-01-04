using Ophelia.Data.Models;

namespace Ophelia.Data;

public interface IAffirmationRepository
{
    Task<IEnumerable<Affirmation>> GetAllAffirmations();

    Task<List<Affirmation>> GetAffirmationsByCategory(string category);

    Task<Affirmation> GetAffirmationById(string id);

    Task<List<Affirmation>> GetAffirmationsByIds(List<string> ids);
    
    Task<List<UserAffirmation>> GetAffirmationsByUser(string userId);

    Task InsertAffirmations(List<Affirmation> affirmations);

    Task InsertAffirmation(Affirmation affirmation);

    Task AssociateAffirmationWithUser(string userId, string affirmationId);

    Task DissociateAffirmationFromUser(string userId, string affirmationId);
}