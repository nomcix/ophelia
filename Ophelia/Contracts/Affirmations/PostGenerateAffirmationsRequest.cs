using System.ComponentModel.DataAnnotations;

namespace Ophelia.Contracts.Affirmations;

public class PostGenerateAffirmationsRequest
{
    [Required(ErrorMessage = "Prompt is required.")]
    public string Prompt { get; set; }
    public bool GenerateTts { get; set; }
}