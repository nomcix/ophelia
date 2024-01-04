namespace Ophelia.Contracts.Flows;

public class GetAllFlowsByUserResponse
{
    public string Id { get; set; }
    
    public string Title { get; set; }
    
    public string Category { get; set; }
    
    public string CreateDate { get; set; }
    
    public List<string> Affirmations { get; set; }
}