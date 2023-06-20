using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class NotebookController : ControllerBase
{
    private readonly DataContext _context;

    public NotebookController(DataContext context)
    {
        _context = context;
    }

    // POST api/notebook/{parentId}/childnotebook
    [HttpPost("{parentId:guid}/childnotebook")]
    public async Task<ActionResult<NotebookData>> AddChildNotebook(Guid parentId, NotebookData notebookData)
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("Could not read GUID from context");
        }
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return BadRequest("Jwt still valid but user has been deleted.");
        }
        var parentFolder = await _context.Folders.FindAsync(parentId);

        if (parentFolder == null)
        {
            return NotFound();
        }
        var childNotebook = new Notebook
        {
            Name = notebookData.Name,
            Pages = notebookData.Pages,
            ParentId = parentId,
            Id = Guid.NewGuid(),
            Owner = user,
            OwnerId = (Guid)userId,
            Parent = parentFolder
        };

        parentFolder.ChildNotebooks.Add(childNotebook);
        _context.Notebooks.Add(childNotebook);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddChildNotebook), new { id = childNotebook.Id }, new NotebookData(childNotebook.Id.ToString(),childNotebook.Pages,childNotebook.Name,childNotebook.ParentId.ToString()));
    }

    // PUT api/notebook/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateNotebook(Guid id, NotebookData notebookData)
    {
        var notebook = await _context.Notebooks.FindAsync(id);

        if (notebook == null)
        {
            return NotFound();
        }

        notebook.Pages = notebookData.Pages;
        notebook.Name = notebookData.Name;

        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE api/notebook/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNotebook(Guid id)
    {
        var notebook = await _context.Notebooks.FindAsync(id);

        if (notebook == null)
        {
            return NotFound();
        }

        _context.Notebooks.Remove(notebook);
        await _context.SaveChangesAsync();

        return Ok();
    }
}