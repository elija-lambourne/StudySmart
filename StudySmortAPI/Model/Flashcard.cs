namespace StudySmortAPI.Model;

public sealed class Flashcard
{
    public Guid Id { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
    public string FlashcardCategory { get; set; }
    public string Word { get; set; }
    public string Translation { get; set; }
    public int CorrectCnt { get; set; }
    public int WrongCnt { get; set; }
    public int SkipCnt { get; set; }
}