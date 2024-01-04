using System.ComponentModel.DataAnnotations;

namespace Ophelia.Contracts;

public class PostAffirmationAssociationRequest
{
    [Required(ErrorMessage = "UserId can not be empty")]
    public string UserId { get; set; }
    [Required(ErrorMessage = "AffirmationId can not be empty")]
    public string AffirmationId { get; set; }
}