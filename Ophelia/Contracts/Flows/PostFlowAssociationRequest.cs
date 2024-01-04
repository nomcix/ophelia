using System.ComponentModel.DataAnnotations;

namespace Ophelia.Contracts.Flows;

public class PostFlowAssociationRequest
{
    [Required(ErrorMessage = "UserId can not be empty")]
    public string UserId { get; set; }
    [Required(ErrorMessage = "FlowId can not be empty")]
    public string FlowId { get; set; }
}