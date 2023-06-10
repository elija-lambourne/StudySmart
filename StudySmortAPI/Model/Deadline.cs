namespace StudySmortAPI.Model;

public sealed class Deadline
{
    public Guid DeadlineId { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime DateTimeUtc { get; set; }
    public string Note { get; set; }
    public string Title { get; set; }
}