namespace BossQuest.Models;

public class ShopItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "weapon"; // weapon, armor, helmet, potion, shield, cosmetic
    public string ImageUrl { get; set; } = "/images/items/default.png";
    public int GoldCost { get; set; } = 50;
    public Dictionary<string, int> StatBonus { get; set; } = new();
    public int RequiredLevel { get; set; } = 1;
    public string RequiredClass { get; set; } = "Any";
    public bool IsConsumable { get; set; }

    public string CategoryIcon => Category switch
    {
        "weapon" => "🗡️",
        "armor" => "🛡️",
        "helmet" => "⛑️",
        "potion" => "🧪",
        "shield" => "🔰",
        "cosmetic" => "✨",
        _ => "📦"
    };
}

public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ItemId { get; set; }
    public ShopItem? Item { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsEquipped { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
}
