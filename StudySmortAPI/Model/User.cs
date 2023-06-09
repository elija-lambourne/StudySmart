namespace StudySmortAPI.Model;

public sealed class User
{
    public Guid Id { get; set; }
    public ICollection<Flashcard> Flashcards { get; set; }
    public Guid RootDirId { get; set; }
    public ICollection<Folder> Folders { get; set; }
    public ICollection<Deadline> Deadlines { get; set; }
}