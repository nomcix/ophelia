using Ophelia.Data.Models;

namespace Ophelia.Data;

public interface IFlowRepository
{
    Task AddFlow(Flow flow);

    Task<Flow> GetFlow(string flowId);

    Task<List<UserFlow>> GetAllFlowsByUser(string userId);

    Task AssociateFlowWithUser(string userId, string flowId);

    Task DissociateFlowFromUser(string userId, string flowId);
}