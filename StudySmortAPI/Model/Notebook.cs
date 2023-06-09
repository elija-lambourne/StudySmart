namespace StudySmortAPI.Model;

public sealed class Notebook
{
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
    public ICollection<string> Pages { get; set; }
}