namespace BossQuest.Models;

public class Boss
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "/images/bosses/default.png";
    public int BaseHp { get; set; } = 1000;
    public int Attack { get; set; } = 15;
    public string Tier { get; set; } = "Normal"; // Normal, Elite, Legendary
    public int XpReward { get; set; } = 200;
    public int GoldReward { get; set; } = 100;
    public bool IsTemplate { get; set; } = true;

    public string TierColor => Tier switch
    {
        "Normal" => "text-slate-300 bg-slate-800 border-slate-600",
        "Elite" => "text-purple-400 bg-purple-950/60 border-purple-500/50",
        "Legendary" => "text-yellow-400 bg-yellow-950/60 border-yellow-500/50",
        _ => "text-slate-400 bg-slate-800 border-slate-700"
    };

    public string BossEmoji => Name switch
    {
        var n when n.Contains("Hydra") => "🐉",
        var n when n.Contains("Lich") => "💀",
        var n when n.Contains("Golem") => "🤖",
        var n when n.Contains("Dragon") => "🐲",
        var n when n.Contains("Specter") => "👻",
        var n when n.Contains("Titan") => "🗿",
        _ => "👾"
    };
}

public class BossEncounter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PartyId { get; set; }
    public Guid BossId { get; set; }
    public Boss? Boss { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public bool IsDefeated { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DefeatedAt { get; set; }

    public double HpPercent => MaxHp > 0 ? Math.Clamp((double)CurrentHp / MaxHp * 100, 0, 100) : 0;
}
