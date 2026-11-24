namespace BossQuest.Models;

public class CombatLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PartyId { get; set; }
    public Guid? EncounterId { get; set; }
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string EventType { get; set; } = "damage"; // damage, boss_attack, heal, critical, boss_defeat, player_down, revive, skill_use
    public string Message { get; set; } = "";
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string EventColor => EventType switch
    {
        "damage" => "text-orange-400",
        "boss_attack" => "text-red-500",
        "heal" => "text-emerald-400",
        "critical" => "text-yellow-300 font-semibold",
        "boss_defeat" => "text-yellow-400 font-bold",
        "player_down" => "text-red-600 font-bold",
        "revive" => "text-cyan-400",
        "skill_use" => "text-purple-400",
        _ => "text-slate-400"
    };

    public string EventIcon => EventType switch
    {
        "damage" => "⚔️",
        "boss_attack" => "💥",
        "heal" => "💚",
        "critical" => "⚡",
        "boss_defeat" => "🏆",
        "player_down" => "💀",
        "revive" => "✨",
        "skill_use" => "🔮",
        _ => "📜"
    };
}
