using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DeadlineController : ControllerBase
{
    private readonly DataContext _dbContext;

    public DeadlineController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Get()
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("Invalid jwt setup");
        }
        
        var deadlines = _dbContext.Deadlines.Where(d => d.OwnerId == userId).ToList();
        if (deadlines.Count == 0)
        {
            return NoContent();
        }

        var deadlineData = deadlines.Select(deadline => new DeadlineData(deadline.DeadlineId.ToString(), deadline.DateTimeUtc, deadline.Note, deadline.Title)).ToList();
        return Ok(deadlineData);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateDeadline([FromBody] DeadlineData deadline)
    {
        if (deadline == null)
        {
            return BadRequest("Invalid deadline data");
        }

        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }

        var newDeadline = new Deadline()
        {
            DeadlineId = Guid.NewGuid(),
            Note = deadline.Note,
            Title = deadline.Title,
            DateTimeUtc = deadline.DateTimeUtc,
            OwnerId = (Guid)userId,
            Owner = _dbContext.Users.FirstOrDefault(x => x.Id == userId)! //! Tf?!
        };
        _dbContext.Deadlines.Add(newDeadline);
        _dbContext.SaveChanges();

        deadline.Id = newDeadline.DeadlineId.ToString();
        return Ok(deadline);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Delete(Guid id)
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }

        var deadlineToBeRemoved = _dbContext.Deadlines
            .FirstOrDefault(d => d.DeadlineId == id);
        if (deadlineToBeRemoved == null)
        {
            return NotFound("Deadline was not found!");
        }

        _dbContext.Deadlines.Remove(deadlineToBeRemoved);
        _dbContext.SaveChanges();

        return Ok("Deadline removed");
    }
}