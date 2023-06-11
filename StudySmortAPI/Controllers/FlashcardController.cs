using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

public class FlashcardController : ControllerBase
{
    private readonly DataContext _dbContext;

    public FlashcardController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public IActionResult DeleteCategory()
    {
        
    }
    
    [HttpPost("category")]
    [Authorize]
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

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Description("Gets all the flashcards which belong to the specified category (passed id)")]
    public IActionResult GetByCategory(string id)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }
        
        var flashcards = _dbContext.Flashcards.Where(f => f.OwnerId == new Guid(userId) && f.CategoryId.ToString() == id).ToList();
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
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Description("Gets the category data (name + id) of all the categories")]
    public IActionResult GetCategories(string id)
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
    
      
    [HttpPost("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Description("Adds the passed flashcard (in the body) to the specified category (hence id)")]
    public IActionResult Post(string id,[FromBody] FlashCardData flashcard)
    {
        var category = _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id.ToString() == id);
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
        _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id.ToString() == id)!.Flashcards.Add(flashcard2);
        _dbContext.SaveChanges();
        
        return Ok(flashcard with { Id = flashcardId, CategoryId = category.Id, CategoryName = category.Name });
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