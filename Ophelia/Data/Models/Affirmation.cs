namespace Ophelia.Data.Models;

public class Affirmation
{
    public string id { get; set; }
    public Guid user_id { get; set; }
    public string affirmation { get; set; }
    public string category { get; set; }
    public bool system { get; set; }
    
    public string create_date { get; set; }
}