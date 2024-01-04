using Ophelia.Data.Models;
using Ophelia.Models.Affirmations;

namespace Ophelia.Contracts;

public class GetAffirmationsByUserResponse
{
    public List<UserAffirmation> Affirmations { get; set; }
}