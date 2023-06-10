namespace StudySmortAPI.Model;

public sealed class User
{
    public Guid Id { get; set; }
    public ICollection<FlashcardCategory> FlashcardCategories { get; set; }
    public ICollection<Folder> Folders { get; set; }
    public ICollection<Deadline> Deadlines { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}