using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FolderController : ControllerBase
{
    private readonly DataContext _dbContext;

    public FolderController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpPost("{parentId:guid}")]
    public IActionResult AddChildFolder(Guid parentId, [FromBody] FolderData childFolder)
    {
        if (childFolder == null)
        {
            return BadRequest("Invalid folder data");
        }
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
        var parentFolder = _dbContext.Folders.FirstOrDefault(f => f.FolderId == parentId);
        if (parentFolder == null)
        {
            return NotFound("Parent folder not found");
        }

        var folder = new Folder()
        {
            ChildFolders = new List<Folder>(),
            ChildNotebooks = new List<Notebook>(),
            FolderId = Guid.NewGuid(),
            FolderName = childFolder.Name,
            Owner = user,
            OwnerId = (Guid)userId,
            ParentFolder = parentFolder,
            ParentFolderId = parentFolder.FolderId
        };
        _dbContext.Folders.FirstOrDefault(f => f.FolderId == parentId)!.ChildFolders.Add(folder);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPost("Notebook/{parentId:guid}")]
    public IActionResult AddNotebookToFolder(Guid parentId, [FromBody] NotebookData notebook)
    {
        if (notebook == null)
        {
            return BadRequest("Invalid notebook data");
        }

        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("Invalid user id");
        }
        var parentFolder = _dbContext.Folders.FirstOrDefault(f => f.FolderId == parentId && f.OwnerId == userId);
        if (parentFolder == null)
        {
            return NotFound("Parent folder not found");
        }
        
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return Unauthorized("User has been deleted but JWT is still valid");
        }
        
        parentFolder.ChildNotebooks.Add(new Notebook()
        {
            Id = Guid.NewGuid(),
            Owner = user,
            OwnerId = (Guid)userId,
            Pages = notebook.Pages.ToList()
        });
        _dbContext.SaveChanges();

        return Ok(notebook);
    }
    
    [HttpPut("Notebook/Pages/{notebookId:guid}")]
    public IActionResult UpdateNotebookPages(Guid notebookId, [FromBody] ICollection<string> newPages)
    {
        var notebook = _dbContext.Notebooks.FirstOrDefault(n => n.Id == notebookId);
        if (notebook == null)
        {
            return NotFound("Notebook not found");
        }

        notebook.UpdatePages(newPages);
        _dbContext.SaveChanges();

        return Ok(notebook);
    }

    [HttpPut]
    public IActionResult UpdateFolder([FromBody] FolderData data)
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("Invalid user id");
        }

        var firstOrDefault = _dbContext.Folders.FirstOrDefault(x => x.FolderId == data.Id && x.OwnerId == userId);
        if (firstOrDefault == null)
        {
            return NotFound();
        }
        
        firstOrDefault.Update(data);
        _dbContext.SaveChanges();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteFolder(Guid id)
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("Invalid user id");
        }

        var ent = _dbContext.Folders.FirstOrDefault(x => x.FolderId == id && x.OwnerId == userId);
        if (ent == null)
        {
            return NotFound();
        }

        _dbContext.Folders.Remove(ent);
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpDelete("notebook/{id:guid}")]
    public IActionResult DeleteNotebook(Guid id)
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("Invalid user id");
        }

        var ent = _dbContext.Notebooks.FirstOrDefault(x => x.Id == id && x.OwnerId == userId);
        if (ent == null)
        {
            return NotFound();
        }

        _dbContext.Notebooks.Remove(ent);
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