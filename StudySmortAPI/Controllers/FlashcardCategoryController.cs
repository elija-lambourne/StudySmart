using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Authorize]
[Route("FlashcardCategory")]
public class FlashcardCategoryController : ControllerBase
{
    private DataContext _dbContext;

    public FlashcardCategoryController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteCategory(Guid id)
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest();
        }

        var ent = _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id == id);
        if (ent == null)
        {
            return NotFound();
        }
        
        _dbContext.FlashcardCategories.Remove(ent);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpPost]
    public IActionResult CreateCategory([FromBody] FlashcardCategoryData categoryData)
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest();
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return Unauthorized("User has been deleted but JWT is still valid");
        }

        var id = Guid.NewGuid();
        _dbContext.FlashcardCategories.Add(new FlashcardCategory()
        {
            Flashcards = new List<Flashcard>(),
            Id = id,
            Name = categoryData.Name,
            Owner = user,
            OwnerId = user.Id
        });
        _dbContext.SaveChanges();

        categoryData.Id = id.ToString();
        return Ok(categoryData);
    }
    
    [HttpPut]
    public IActionResult UpdateCategory([FromBody] FlashcardCategoryData data)
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest();
        }

        var firstOrDefault = _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id == new Guid(data.Id) && x.OwnerId == userId);
        if (firstOrDefault == null)
        {
            return NotFound();
        }
        firstOrDefault.Update(data.Name);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpGet("{id:guid}")]
    public IActionResult GetByCategory(Guid id)
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        
        var flashcards = _dbContext.Flashcards
            .Include(f => f.Category)
            .Where(f => f.OwnerId == userId && f.CategoryId == id)
            .ToList();

        if (flashcards.Count == 0)
        {
            return NoContent();
        }

        var flashcardData = new List<FlashCardData>();
        foreach (var flashcard in flashcards)
        {
            flashcardData.Add(new FlashCardData(flashcard.Id.ToString(),flashcard.Word,flashcard.Translation,flashcard.CorrectCnt,
                flashcard.WrongCnt,flashcard.SkipCnt,flashcard.CategoryId.ToString(),flashcard.Category.Name));
        }
        
        return Ok(flashcardData);
    }  
    
    [HttpGet("all")]
    public IActionResult GetCategories()
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        
        var flashcardCategories = _dbContext.FlashcardCategories.Where(f => f.OwnerId == userId).ToList();
        if (flashcardCategories.Count == 0)
        {
            return NoContent();
        }

        var actualColl = flashcardCategories.Select(flashcardCategory => new FlashcardCategoryData(flashcardCategory.Id.ToString(), flashcardCategory.Name)).ToList();

        return Ok(actualColl);
    }


}