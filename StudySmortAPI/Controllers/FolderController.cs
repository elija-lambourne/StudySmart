using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

public class FolderController : ControllerBase
{
    private readonly DataContext _dbContext;

    public FolderController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpPost("Folder/{parentId}")]
    [Authorize]
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
        
        _dbContext.Folders.FirstOrDefault(f => f.FolderId == parentId)!.ChildFolders.Add(new Folder()
        {
            ChildFolders = new List<Folder>(),
            ChildNotebooks = new List<Notebook>(),
            FolderId = Guid.NewGuid(),
            FolderName = childFolder.Name,
            Owner = user,
            OwnerId = (Guid)userId,
            ParentFolder = parentFolder,
            ParentFolderId = parentFolder.FolderId
        });
        _dbContext.SaveChanges();

        return Ok();
    }

    [HttpPost("Notebook/{parentId}")]
    [Authorize]
    public IActionResult AddNotebookToFolder(Guid parentId, [FromBody] NotebookData notebook)
    {
        if (notebook == null)
        {
            return BadRequest("Invalid notebook data");
        }

        var parentFolder = _dbContext.Folders.FirstOrDefault(f => f.FolderId == parentId);
        if (parentFolder == null)
        {
            return NotFound("Parent folder not found");
        }

        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("Invalid user id");
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
    
    [HttpPut("Notebook/Pages/{notebookId}")]
    [Authorize]
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