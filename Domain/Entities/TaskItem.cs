namespace OnOff.Api.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}
