using Amazon.DynamoDBv2.DataModel;

namespace Ophelia.Data.Models;

public class Flow
{
    public string id { get; set; }
    public string title { get; set; }
    public string category { get; set; }
    public bool system { get; set; }
    public string[] affirmation_ids { get; set; }
}

