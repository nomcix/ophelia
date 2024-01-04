namespace Ophelia.Data.Models;

public class UserAffirmation
{
    public string id { get; set; }
    
    public string affirmation { get; set; }
    
    public string category { get; set; }
    
    public bool system { get; set; }
    
    public string liked_on { get; set; }
}
