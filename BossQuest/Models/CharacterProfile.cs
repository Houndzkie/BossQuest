namespace BossQuest.Models;

public class CharacterProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = "Adventurer";
    public string AvatarUrl { get; set; } = "/images/avatars/default.png";
    public string Class { get; set; } = "None"; // None, Warrior, Mage, Healer, Rogue
    public int Level { get; set; } = 1;
    public int Xp { get; set; } = 0;
    public int XpToNext { get; set; } = 100;
    public int Hp { get; set; } = 50;
    public int MaxHp { get; set; } = 50;
    public int Mp { get; set; } = 30;
    public int MaxMp { get; set; } = 30;
    public int Gold { get; set; } = 0;
    public int Attack { get; set; } = 10;
    public int StreakDays { get; set; } = 0;
    public Guid? PartyId { get; set; }
    public Dictionary<string, string> EquippedGear { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public double HpPercent => MaxHp > 0 ? Math.Clamp((double)Hp / MaxHp * 100, 0, 100) : 0;
    public double MpPercent => MaxMp > 0 ? Math.Clamp((double)Mp / MaxMp * 100, 0, 100) : 0;
    public double XpPercent => XpToNext > 0 ? Math.Clamp((double)Xp / XpToNext * 100, 0, 100) : 0;

    public string ClassBadgeColor => Class switch
    {
        "Warrior" => "bg-red-500/20 text-red-400 border-red-500/40",
        "Mage" => "bg-blue-500/20 text-blue-400 border-blue-500/40",
        "Healer" => "bg-emerald-500/20 text-emerald-400 border-emerald-500/40",
        "Rogue" => "bg-amber-500/20 text-amber-400 border-amber-500/40",
        _ => "bg-slate-700/50 text-slate-400 border-slate-600"
    };

    public string ClassRoleTitle => Class switch
    {
        "Warrior" => "Crammer",
        "Mage" => "Scholar",
        "Healer" => "Support",
        "Rogue" => "Sprinter",
        _ => "Novice"
    };

    public string ClassIcon => Class switch
    {
        "Warrior" => "⚔️",
        "Mage" => "🔮",
        "Healer" => "💚",
        "Rogue" => "🗡️",
        _ => "👤"
    };
}
