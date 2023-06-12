using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FlashcardController : ControllerBase
{
    private readonly DataContext _dbContext;

    public FlashcardController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpDelete("category/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteCategory(Guid id)
    {
        var userId = GetUserIdFromContext();
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
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteFlashcard(Guid id)
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest();
        }

        var ent = _dbContext.Flashcards.FirstOrDefault(x => x.Id == id);
        if (ent == null)
        {
            return NotFound();
        }
        
        _dbContext.Flashcards.Remove(ent);
        _dbContext.SaveChanges();

        return Ok();
    }
    
    [HttpPost("category")]
    [Description("Creates a new flashcard category based on the passed json data")]
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

        categoryData.Id = id;
        return Ok(categoryData);
    }

    [HttpPut("category")]
    public IActionResult UpdateCategory([FromBody] FlashcardCategoryData data)
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest();
        }

        var firstOrDefault = _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id == data.Id && x.OwnerId == userId);
        if (firstOrDefault == null)
        {
            return NotFound();
        }
        firstOrDefault.Update(data.Name);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Description("Gets all the flashcards which belong to the specified category (passed id)")]
    public IActionResult GetByCategory(Guid id)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        
        var flashcards = _dbContext.Flashcards.Where(f => f.OwnerId == new Guid(userId) && f.CategoryId == id).ToList();
        if (flashcards.Count == 0)
        {
            return NoContent();
        }

        var flashcardData = new List<FlashCardData>();
        foreach (var flashcard in flashcards)
        {
            flashcardData.Add(new FlashCardData(flashcard.Id,flashcard.Word,flashcard.Translation,flashcard.CorrectCnt,
                flashcard.WrongCnt,flashcard.SkipCnt,flashcard.CategoryId,flashcard.Category.Name));
        }
        
        return Ok(flashcardData);
    }   
    
    [HttpGet("category/all")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Description("Gets the category data (name + id) of all the categories")]
    public IActionResult GetCategories()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        
        var flashcardCategories = _dbContext.FlashcardCategories.Where(f => f.OwnerId.ToString() == userId).ToList();
        if (flashcardCategories.Count == 0)
        {
            return NoContent();
        }

        var actualColl = flashcardCategories.Select(flashcardCategory => new FlashcardCategoryData(flashcardCategory.Id, flashcardCategory.Name)).ToList();

        return Ok(actualColl);
    }
    
      
    [HttpPost("flashcard/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Description("Adds the passed flashcard (in the body) to the specified category (hence id)")]
    public IActionResult Post(Guid id,[FromBody] FlashCardData flashcard)
    {
        var category = _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id == id);
        if (flashcard == null || category == null)
        {
            return BadRequest();
        }

        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return Unauthorized("User has been deleted but JWT is still valid");
        }

        var flashcardId = Guid.NewGuid();
        var flashcard2 = new Flashcard()
        {
            Category = category,
            CategoryId = category.Id,
            CorrectCnt = flashcard.CorrectCnt,
            Id = flashcardId,
            Owner = user,
            OwnerId = user.Id,
            SkipCnt = flashcard.SkipCnt,
            Translation = flashcard.Translation,
            Word = flashcard.Word,
            WrongCnt = flashcard.WrongCnt
        };
        _dbContext.Flashcards.Add(flashcard2);
        _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id == id)!.Flashcards.Add(flashcard2);
        _dbContext.SaveChanges();
        
        return Ok(flashcard with { Id = flashcardId, CategoryId = category.Id, CategoryName = category.Name });
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Description("Adds the passed flashcard (in the body) to the specified category (hence id)")]
    public IActionResult UpdateFlashcard([FromBody] FlashCardData flashcard)
    {
        if (flashcard == null)
        {
            return BadRequest();
        }

        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }

        _dbContext.Flashcards.FirstOrDefault(x => x.Id == flashcard.Id && x.OwnerId == userId)?.UpdateData(flashcard);
        _dbContext.SaveChanges();
        
        return Ok();
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