namespace BossQuest.Models;

public class Party
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string InviteCode { get; set; } = "";
    public Guid LeaderId { get; set; }
    public int MaxMembers { get; set; } = 6;
    public Guid? ActiveBossId { get; set; }
    public BossEncounter? ActiveBoss { get; set; }
    public List<CharacterProfile> Members { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
