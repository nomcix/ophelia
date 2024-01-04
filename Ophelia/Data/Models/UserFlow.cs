namespace Ophelia.Data.Models;

public class UserFlow
{
    public string Flow_Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public bool System { get; set; }
    public string Liked_On { get; set; }
    public string[] Affirmation_Ids { get; set; }
}
