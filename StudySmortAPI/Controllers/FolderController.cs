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

    private readonly ILogger<FolderController> _logger;
    public FolderController(DataContext dbContext,ILogger<FolderController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    [HttpPost("{parentId:guid}")]
    public IActionResult AddChildFolder(Guid parentId, [FromBody] FolderData childFolder)
    {
        _logger.LogInformation("POST Folder/{{parentId:guid}}");
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

    [HttpPut("{name}+{id:guid}")]
    public IActionResult RenameFolder(string name,Guid id)
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("Invalid user id");
        }

        var firstOrDefault = _dbContext.Folders.FirstOrDefault(x => x.FolderId == id && x.OwnerId == userId);
        if (firstOrDefault == null)
        {
            return NotFound();
        }
        
        firstOrDefault.Update(name);
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
    
    
    [HttpGet]
    public IActionResult GetAllFolders()
    {
        var userId = GetUserIdFromContext();
        if (userId == null)
        {
            return BadRequest("Invalid user id");
        }

        var root = _dbContext.Users.FirstOrDefault(x => x.Id == userId)?.RootDir;
        if (root == null)
        {
            return BadRequest("User delete, but JWT still valid");
        }

        return Ok(ConvertFolderToFolderData(root));
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
    
    private static FolderData ConvertFolderToFolderData(Folder folder)
    {
        // Recursively convert child folders
        var childFolders = folder.ChildFolders?.Select(ConvertFolderToFolderData)?.ToList();

        // Convert child notebooks to a desired format (assuming you have a similar conversion method)
        var childNotebooks = folder.ChildNotebooks?.Select(ConvertNotebookToNotebookData)?.ToList();

        // Create the FolderData object using the provided properties
        var folderData = new FolderData(
            folder.FolderId,
            folder.FolderName,
            folder.ParentFolderId,
            folder.ParentFolder?.FolderName ?? "-",
            childFolders ?? new List<FolderData>(),
            childNotebooks ?? new List<NotebookData>()
        );

        return folderData;
    }

    private static NotebookData ConvertNotebookToNotebookData(Notebook notebook)
    {
        var notebookData = new NotebookData(
            notebook.Id,
            notebook.Pages,
            notebook.OwnerId
        );

        return notebookData;
    }

}