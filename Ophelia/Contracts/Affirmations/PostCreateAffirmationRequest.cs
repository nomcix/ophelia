namespace Ophelia.Contracts;

public class PostCreateAffirmationRequest
{
    public string Affirmation { get; set; }
    public string UserId { get; set; }
    
    public string Category { get; set; }
}