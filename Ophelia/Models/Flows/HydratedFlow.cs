using Ophelia.Data.Models;

namespace Ophelia.Models.Flows;

public class HydratedFlow
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public bool System { get; set; }
    public string Liked_On { get; set; }
    public List<Affirmation> Affirmations { get; set; }
}