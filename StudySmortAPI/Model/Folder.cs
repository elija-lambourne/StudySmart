namespace StudySmortAPI.Model;

public sealed class Folder
{
    public Guid OwnerId { get; set; }
    public Guid FolderId { get; set; }
    public string FolderName { get; set; }
    public ICollection<Folder> ChildFolders { get; set; }
    public ICollection<Notebook> ChildNotebooks { get; set; }
    public Folder? ParentFolder { get; set; }
    public Guid? ParentFolderId { get; set; }
    
}