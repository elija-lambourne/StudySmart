namespace StudySmortAPI.Model;

public sealed class Notebook
{
    public Folder Parent { get; set; }
    public Guid ParentId { get; set; }
    public Guid Id { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
    public ICollection<string> Pages { get; set; }
}