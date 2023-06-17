using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteFlashcard(Guid id)
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
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

    [HttpPost("{id:guid}")]
    public IActionResult Post(Guid id,[FromBody] FlashCardData flashcard)
    {
        var category = _dbContext.FlashcardCategories.FirstOrDefault(x => x.Id == id);
        if (flashcard == null || category == null)
        {
            return BadRequest();
        }

        var userId = AccountController.GetGuidFromToken(HttpContext);
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
        
        return Ok(flashcard with { Id = flashcardId.ToString(), CategoryId = category.Id.ToString(), CategoryName = category.Name });
    }
    
    [HttpPut]
    public IActionResult UpdateFlashcard([FromBody] FlashCardData flashcard)
    {
        if (flashcard == null)
        {
            return BadRequest();
        }

        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("User ID not found");
        }

        _dbContext.Flashcards.FirstOrDefault(x => x.Id == new Guid(flashcard.Id) && x.OwnerId == userId)?.UpdateData(flashcard);
        _dbContext.SaveChanges();
        
        return Ok();
    }
}