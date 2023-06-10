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
    
    [HttpPost("{parentId}/ChildFolder")]
    [Authorize]
    public IActionResult AddChildFolder(Guid parentId, [FromBody] Folder childFolder)
    {
        if (childFolder == null)
        {
            return BadRequest("Invalid folder data");
        }

        var parentFolder = _dbContext.Folders.FirstOrDefault(f => f.FolderId == parentId);
        if (parentFolder == null)
        {
            return NotFound("Parent folder not found");
        }

        childFolder.FolderId = Guid.NewGuid();
        parentFolder.ChildFolders.Add(childFolder);
        _dbContext.SaveChanges();

        return Ok(childFolder);
    }

    [HttpPost("{parentId}/Notebook")]
    [Authorize]
    public IActionResult AddNotebookToFolder(Guid parentId, [FromBody] Notebook notebook)
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
        notebook.OwnerId = (Guid)userId;
        parentFolder.ChildNotebooks.Add(notebook);
        _dbContext.SaveChanges();

        return Ok(notebook);
    }
    
    [HttpPut("{notebookId}/Pages")]
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