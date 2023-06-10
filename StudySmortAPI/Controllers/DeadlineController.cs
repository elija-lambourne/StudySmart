using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DeadlineController : ControllerBase
{
    private readonly DataContext _dbContext;

    public DeadlineController(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Get()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        
        var deadlines = _dbContext.Deadlines.Where(d => d.OwnerId == new Guid(userId)).ToList();
        if (deadlines.Count == 0)
        {
            return NoContent();
        }
        
        return Ok(deadlines);
    }
    
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateDeadline([FromBody] Deadline deadline)
    {
        if (deadline == null)
        {
            return BadRequest("Invalid deadline data");
        }

        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        deadline.OwnerId = (Guid)userId;

        deadline.DeadlineId = Guid.NewGuid();

        _dbContext.Deadlines.Add(deadline);
        _dbContext.SaveChanges();

        return Ok(deadline);
    }

    private Guid? GetUserIdFromContext()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userId == null)
        {
            return null;
        }
        
        return new Guid(userId);
    }
}