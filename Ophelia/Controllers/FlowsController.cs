using Microsoft.AspNetCore.Mvc;
using Ophelia.Contracts;
using Ophelia.Contracts.Flows;
using Ophelia.Data.Models;
using Ophelia.Services;
using Ophelia.Services.Flows;

namespace Ophelia.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlowsController : ControllerBase
    {
        private const string ErrorMessage = "An error occurred while processing your request.";
        private readonly IFlowService _flowService;

        public FlowsController(IFlowService flowService)
        {
            _flowService = flowService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlow(PostCreateFlowRequest request)
        {
            try
            {
                var flow = new Flow
                {
                    title = request.Title,
                    affirmation_ids = request.affirmation_ids,
                    category = request.Category 
                };

                var flowId =await _flowService.AddFlow(flow);
                return Ok(new PostCreateFlowResponse
                {
                    FlowId = flowId
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the flow.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlow(string id)
        {
            try
            {
                var flow = await _flowService.GetFlow(id).ConfigureAwait(false);
                return Ok(flow);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorMessage);
            }
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetAllFlowsByUser(string userId)
        {
            try
            {
                var flows = await _flowService.GetAllFlowsByUser(userId).ConfigureAwait(false);
                
                return Ok(flows);
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorMessage);
            }
        }
        
        [HttpPost("user-flows")]
        public async Task<ActionResult> AssociateAffirmationWithUser(PostFlowAssociationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FlowId) || string.IsNullOrWhiteSpace(request.UserId))
                {
                    return BadRequest("User or Flow cannot be empty");
                }

                await _flowService.AssociateFlowWithUser(request.UserId, request.FlowId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorMessage);
            }
        }
    
        [HttpDelete("user-flows")]
        public async Task<ActionResult> DissociateAffirmationWithUser(DeleteFlowAssociationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FlowId) || string.IsNullOrWhiteSpace(request.UserId))
                {
                    return BadRequest("User or Flow cannot be empty");
                }

                await _flowService.DissociateFlowFromUser(request.UserId, request.FlowId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorMessage);
            }
        }
    }
}