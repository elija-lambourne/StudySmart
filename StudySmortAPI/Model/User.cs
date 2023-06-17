namespace StudySmortAPI.Model;

public sealed class User
{
    public Guid Id { get; set; }
    public ICollection<FlashcardCategory> FlashcardCategories { get; set; }

    public Folder RootDir { get; set; }
    public Guid FolderId { get; set; }
    public ICollection<Deadline> Deadlines { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; } 
    public string Image { get; set; }
}