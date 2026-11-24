namespace BossQuest.Models;

public enum TaskType
{
    Daily,
    Quest
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Exam
}

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public TaskType TaskType { get; set; } = TaskType.Quest;
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public string? CourseLabel { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int DamageDealt { get; set; }
    public int XpReward { get; set; }
    public int GoldReward { get; set; }
    public int StreakCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string DifficultyColor => Difficulty switch
    {
        Difficulty.Easy => "text-emerald-400 bg-emerald-500/10 border-emerald-500/30",
        Difficulty.Medium => "text-amber-400 bg-amber-500/10 border-amber-500/30",
        Difficulty.Hard => "text-orange-400 bg-orange-500/10 border-orange-500/30",
        Difficulty.Exam => "text-red-400 bg-red-500/10 border-red-500/30",
        _ => "text-slate-400 bg-slate-800 border-slate-700"
    };

    public string DifficultyLabel => Difficulty switch
    {
        Difficulty.Easy => "Easy",
        Difficulty.Medium => "Medium",
        Difficulty.Hard => "Hard",
        Difficulty.Exam => "Exam-Level",
        _ => "Unknown"
    };
}
