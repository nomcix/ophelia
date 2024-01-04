using Ophelia.Data.Models;
using Ophelia.Models.Flows;

namespace Ophelia.Services.Flows;

public interface IFlowService
{
    Task<string> AddFlow(Flow flow);

    Task<Flow> GetFlow(string flowId);

    Task<List<HydratedFlow>> GetAllFlowsByUser(string userId);
    
    Task AssociateFlowWithUser(string userId, string flowId);

    Task DissociateFlowFromUser(string userId, string flowId);
}