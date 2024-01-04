using Ophelia.Data;
using Ophelia.Data.Models;
using Ophelia.Models.Flows;

namespace Ophelia.Services.Flows
{
    public class FlowService : IFlowService
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IAffirmationService _affirmationService;
        private readonly ILogger<FlowService> _logger;

        public FlowService(
            IFlowRepository flowRepository,
            ILogger<FlowService> logger,
            IAffirmationService affirmationService)
        {
            _flowRepository = flowRepository;
            _affirmationService = affirmationService;
            _logger = logger;
        }

        public async Task<string> AddFlow(Flow flow)
        {
            try
            {
                flow.id = Guid.NewGuid().ToString();
                await _flowRepository.AddFlow(flow);
                return flow.id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddFlow");
                throw;
            }
        }

        public async Task<Flow> GetFlow(string flowId)
        {
            try
            {
                return await _flowRepository.GetFlow(flowId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetFlow with ID: {flowId}");
                throw;
            }
        }

        public async Task<List<HydratedFlow>> GetAllFlowsByUser(string userId)
        {
            var hydratedFlows = new List<HydratedFlow>();
            
            try
            {
                var flows = await _flowRepository.GetAllFlowsByUser(userId).ConfigureAwait(false);

                foreach (var flow in flows)
                {
                    var flowAffirmations = await _affirmationService.GetAffirmationsByIds(flow.Affirmation_Ids.ToList())
                        .ConfigureAwait(false);

                    var hydratedFlow = new HydratedFlow
                    {
                        Flow_Id = flow.Flow_Id,
                        Title = flow.Title,
                        Category = flow.Category,
                        System = flow.System,
                        Liked_On = flow.Liked_On,
                        Affirmations = flowAffirmations
                    };
                    
                    hydratedFlows.Add(hydratedFlow);
                }
                
                return hydratedFlows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllFlowsByUser for user: {userId}");
                throw;
            }
        }

        public async Task AssociateFlowWithUser(string userId, string flowId)
        {
            try
            {
                await _flowRepository.AssociateFlowWithUser(userId, flowId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in AssociateFlowWithUser");
                throw;
            }
        }

        public async Task DissociateFlowFromUser(string userId, string flowId)
        {
            try
            {
                await _flowRepository.DissociateFlowFromUser(userId, flowId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in DissociateAffirmationFromUser");
                throw;
            }
        }
    }
}