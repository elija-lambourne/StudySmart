namespace StudySmortAPI.Model;

public sealed class Notebook
{
    public Guid Id { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
    public ICollection<string> Pages { get; set; }
    public void UpdatePages(ICollection<string> newPages)
    {
        Pages = newPages;
    }
}