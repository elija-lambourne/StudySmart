namespace StudySmortAPI.Model;

public sealed class Deadline
{
    public Guid DeadlineId { get; set; }
    public DateTimeOffset DateTimeUtc { get; set; }
    public string DeadlineNote { get; set; }
    public string DeadlineCategory { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }
}