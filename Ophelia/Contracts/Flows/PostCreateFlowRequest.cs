namespace Ophelia.Contracts;

public class PostCreateFlowRequest
{
    public string Title { get; set; }
    public string[] affirmation_ids { get; set; }
    public string Category { get; set; }
}
