namespace StudySmortAPI.Model;

public class FlashcardCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Flashcard> Flashcards { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
}