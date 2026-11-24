namespace BossQuest.Models;

public class FocusSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int DurationMinutes { get; set; } = 25;
    public string? SubjectLabel { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public bool IsCompleted { get; set; }
    public int DamageDealt { get; set; }
    public int XpEarned { get; set; }
    public int MpRestored { get; set; }
}
