using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FolderController : ControllerBase
{
    private readonly DataContext _context;

    public FolderController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("rootdir")]
    public async Task<ActionResult<FolderData>> GetRootDir()
    {
        var userId = AccountController.GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest("Could not read GUID from context");
        }

        if (!_context.Users.Any(x => x.Id == userId))
        {
            return BadRequest("Jwt still valid, but user has been removed");
        }

        var rootDir = await _context.Folders
            .Include(f => f.ChildFolders) // Include child folders
            .ThenInclude(cf => cf.ChildFolders) // Include child folders recursively
            .Include(f => f.ChildNotebooks)
            .FirstOrDefaultAsync(f => f.ParentFolderId == null && f.OwnerId == userId);

        if (rootDir == null)
        {
            return StatusCode(500);
        }

        var folderData = MapFolderToFolderData(rootDir);

        return Ok(folderData);
    }
    // POST api/folder/{parentId}/childfolder
    [HttpPost("{parentId:guid}/childfolder")]
    public async Task<ActionResult<FolderData>> AddChildFolder(Guid parentId, FolderData folderData)
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

        var childFolder = new Folder
        {
            FolderName = folderData.Name,
            ParentFolderId = parentId,
            FolderId = Guid.NewGuid(),
            ChildFolders = new List<Folder>(),
            ChildNotebooks = new List<Notebook>(),
            ParentFolder = parentFolder
        };

        _context.Folders.Add(childFolder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddChildFolder), new { id = childFolder.FolderId },
            new FolderData(childFolder.FolderId.ToString(), childFolder.FolderName,
                childFolder.ParentFolderId.ToString(), childFolder.ParentFolder.FolderName, new List<FolderData>(),
                new List<NotebookData>()));
    }

    // PUT api/folder/{id}/name
    [HttpPut("{id:guid}/{name}")]
    public async Task<IActionResult> UpdateFolderName(Guid id, string name)
    {
        var folder = await _context.Folders.FindAsync(id);

        if (folder == null)
        {
            return NotFound();
        }

        folder.FolderName = name;

        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE api/folder/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFolder(Guid id)
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

        var folder = await _context.Folders.FindAsync(id);

        if (folder == null)
        {
            return NotFound();
        }
        
        if (_context.Folders.FirstOrDefault(f => f.ParentFolderId == null && f.OwnerId == userId)?.FolderId == folder.FolderId)
        {
            return Unauthorized("The root dir may not be deleted!");
        }

        _context.Folders.Remove(folder);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private static FolderData MapFolderToFolderData(Folder folder)
    {
        var childFoldersData = folder.ChildFolders?.Select(cf => MapFolderToFolderData(cf)).ToList();
        var childNotebooksData = folder.ChildNotebooks?.Select(cn => MapNotebookToNotebookData(cn)).ToList();

        var folderData = new FolderData
        (
            folder.FolderId.ToString(),
            folder.FolderName,
            folder.ParentFolderId?.ToString(),
            folder.ParentFolder?.FolderName,
            childFoldersData ?? new List<FolderData>(),
            childNotebooksData ?? new List<NotebookData>()
        );

        return folderData;
    }


    private static NotebookData MapNotebookToNotebookData(Notebook notebook)
    {
        var notebookData = new NotebookData
        (
            notebook.Id.ToString(),
            notebook.Pages.ToList(),
            notebook.Name,
            notebook.ParentId.ToString()
        );

        return notebookData;
    }
}