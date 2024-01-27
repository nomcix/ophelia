using Microsoft.AspNetCore.Mvc;
using Ophelia.Contracts;
using Ophelia.Contracts.Affirmations;
using Ophelia.Services;

namespace Ophelia.Controllers;

[ApiController]
[Route("[controller]")]
public class AffirmationsController : ControllerBase
{
    private const string ErrorMessage = "An error occurred while processing your request.";
    private readonly IAffirmationService _service;
    public AffirmationsController(IAffirmationService service)
    {
        _service = service; 
    }

    [HttpGet]
    public async Task<ActionResult<AffirmationsResponse>> Get()
    {
        try
        {
            var affirmations = await _service.GetAllAffirmationsAsync().ConfigureAwait(false);
            
            return Ok(new AffirmationsResponse
            {
                Affirmations = affirmations
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }
    
    
    [HttpGet("categories/{category}")]
    public async Task<ActionResult<AffirmationsResponse>> GetAffirmationsByCategory(string category)
    {
        try
        {
            var affirmations = await _service.GetAffirmationsByCategory(category).ConfigureAwait(false);
            
            return Ok(new AffirmationsResponse
            {
                Affirmations = affirmations
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAffirmationById(string id)
    {
        try
        {
            var affirmation = await _service.GetAffirmationById(id).ConfigureAwait(false);
            
            if (string.IsNullOrWhiteSpace(affirmation.id))
            {
                return NotFound();
            }

            return Ok(affirmation);
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<GetAffirmationsByUserResponse>> GetAffirmationsByUser(string userId)
    {
        try
        {
            var affirmations = await _service.GetAffirmationsByUser(userId).ConfigureAwait(false);
            
            return Ok(new GetAffirmationsByUserResponse
            {
                Affirmations = affirmations
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }

    [HttpGet("dailyAffirmation")]
    public async Task<ActionResult> GetDailyAffirmation()
    {
        try
        {
            var dailyAffirmation = await _service.GetDailyAffirmation().ConfigureAwait(false);
            return Ok(dailyAffirmation);
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> PostAddAffirmation(PostCreateAffirmationRequest request)
    {
        try
        {
            await _service.PostAddUserAffirmation(request.UserId, request.Affirmation, request.Category)
                .ConfigureAwait(false);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }
    
    [HttpPost("generate")]
    public async Task<ActionResult<AffirmationsResponse>> PostGenerateAffirmations(PostGenerateAffirmationsRequest request)
    {
        try
        {
            var affirmations = await _service.PostGenerateAffirmations(request.Prompt, request.GenerateTts).ConfigureAwait(false);
            
            return Ok(new AffirmationsResponse
            {
                Affirmations = affirmations
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }
    
    [HttpPost("generate-single")]
    public async Task<ActionResult<AffirmationResponse>> PostGenerateSingleAffirmationRequest(PostGenerateSingleAffirmationRequest request)
    {
        try
        {
            var affirmation = await _service.PostGenerateSingleAffirmation(request.Prompt, request.GenerateTts).ConfigureAwait(false);
            
            return Ok(new AffirmationResponse
            {
                Affirmation = affirmation
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }

    [HttpPost("user-affirmations")]
    public async Task<ActionResult> AssociateAffirmationWithUser(PostAffirmationAssociationRequest request)
    {
        try
        {
            await _service.AssociateAffirmationWithUser(request.UserId, request.AffirmationId).ConfigureAwait(false);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }
    
    [HttpDelete("user-affirmations")]
    public async Task<ActionResult> DissociateAffirmationWithUser(DeleteAffirmationAssociationRequest request)
    {
        try
        {
            await _service.DissociateAffirmationFromUser(request.UserId, request.AffirmationId).ConfigureAwait(false);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorMessage);
        }
    }
}
