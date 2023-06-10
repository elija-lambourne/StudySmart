using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

public class FlashcardController : ControllerBase
{
    private readonly DataContext _dbContext;

    public FlashcardController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("category")]
    [Authorize]
    public IActionResult CreateCategory([FromBody] FlashcardCategoryData categoryData)
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest();
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return Unauthorized("User has been deleted but JWT is still valid");
        }

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
        
        var deadlines = _dbContext.Flashcards.Where(f => f.OwnerId == new Guid(userId)).ToList();
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
    public IActionResult Create([FromBody] Flashcard flashcard)
    {
        if (flashcard == null)
        {
            return BadRequest("Invalid flashcard data");
        }

        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        flashcard.OwnerId = (Guid)userId;

        flashcard.Id = Guid.NewGuid();

        _dbContext.Flashcards.Add(flashcard);
        _dbContext.SaveChanges();

        return Ok(flashcard);
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