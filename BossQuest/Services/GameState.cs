using BossQuest.Models;

namespace BossQuest.Services;

public class GameState
{
    public event Action? OnStateChanged;

    public CharacterProfile CurrentUser { get; private set; }
    public Party? CurrentParty { get; private set; }
    public BossEncounter? ActiveEncounter { get; private set; }
    public List<TaskItem> Tasks { get; private set; }
    public List<CombatLogEntry> CombatLog { get; private set; }
    public List<Boss> Bosses { get; private set; }
    public List<ShopItem> ShopItems { get; private set; }
    public List<InventoryItem> Inventory { get; private set; }
    public FocusSession? ActiveFocusSession { get; set; }

    // Active buffs
    public bool HasArcaneFocusBuff { get; set; }

    public GameState()
    {
        // 1. Current user - Helge, Lv5 Warrior
        var helgeId = Guid.NewGuid();
        CurrentUser = new CharacterProfile
        {
            Id = helgeId,
            Username = "Helge",
            Class = "Warrior",
            Level = 5,
            Xp = 65,
            XpToNext = 500,
            Hp = 42,
            MaxHp = 50,
            Mp = 20,
            MaxMp = 30,
            Gold = 350,
            Attack = 15,
            StreakDays = 7
        };

        // 2. Party members
        var ariaId = Guid.NewGuid();
        var rivenId = Guid.NewGuid();
        var mariaId = Guid.NewGuid();

        var aria = new CharacterProfile
        {
            Id = ariaId,
            Username = "Aria",
            Class = "Healer",
            Level = 4,
            Xp = 30,
            XpToNext = 400,
            Hp = 48,
            MaxHp = 50,
            Mp = 25,
            MaxMp = 30,
            Gold = 220,
            Attack = 10,
            StreakDays = 5
        };

        var riven = new CharacterProfile
        {
            Id = rivenId,
            Username = "Riven",
            Class = "Mage",
            Level = 3,
            Xp = 80,
            XpToNext = 300,
            Hp = 35,
            MaxHp = 50,
            Mp = 30,
            MaxMp = 30,
            Gold = 180,
            Attack = 12,
            StreakDays = 3
        };

        var maria = new CharacterProfile
        {
            Id = mariaId,
            Username = "Maria",
            Class = "Rogue",
            Level = 4,
            Xp = 45,
            XpToNext = 400,
            Hp = 40,
            MaxHp = 50,
            Mp = 18,
            MaxMp = 30,
            Gold = 410,
            Attack = 14,
            StreakDays = 9
        };

        // 3. Party
        var partyId = Guid.NewGuid();
        CurrentParty = new Party
        {
            Id = partyId,
            Name = "Dragon Slayers",
            InviteCode = "BOSS2024",
            LeaderId = helgeId,
            Members = new List<CharacterProfile> { CurrentUser, aria, riven, maria }
        };
        CurrentUser.PartyId = partyId;

        // 4. Bosses bestiary
        Bosses = new List<Boss>
        {
            new Boss { Name = "The Procrastination Hydra", Description = "A many-headed beast that grows stronger every hour you delay. Each head whispers 'just five more minutes...'", BaseHp = 800, Attack = 12, Tier = "Normal", XpReward = 150, GoldReward = 80 },
            new Boss { Name = "Midterm Lich", Description = "An undead sorcerer fueled by forgotten formulas and unread chapters. Its phylactery is hidden inside a blue book.", BaseHp = 1200, Attack = 18, Tier = "Elite", XpReward = 300, GoldReward = 150 },
            new Boss { Name = "Syntax Error Golem", Description = "A hulking construct of tangled code and missing semicolons. It crashes through defenses with segfaults.", BaseHp = 1000, Attack = 15, Tier = "Normal", XpReward = 200, GoldReward = 100 },
            new Boss { Name = "Deadline Dragon", Description = "A colossal wyrm whose fiery breath incinerates schedules. It lands on the night before submission day.", BaseHp = 1500, Attack = 22, Tier = "Legendary", XpReward = 500, GoldReward = 250 },
            new Boss { Name = "All-Nighter Specter", Description = "A ghostly presence that drains your energy and blurs your vision. Fueled by caffeine and desperation.", BaseHp = 600, Attack = 10, Tier = "Normal", XpReward = 100, GoldReward = 60 },
            new Boss { Name = "Thesis Titan", Description = "An ancient colossus built from citation stacks and literature reviews. Its weak point is the methodology section.", BaseHp = 2000, Attack = 25, Tier = "Legendary", XpReward = 800, GoldReward = 400 }
        };

        // 5. Active encounter - Syntax Error Golem
        var golem = Bosses[2];
        ActiveEncounter = new BossEncounter
        {
            PartyId = partyId,
            BossId = golem.Id,
            Boss = golem,
            CurrentHp = 680,
            MaxHp = 1000
        };
        CurrentParty.ActiveBossId = ActiveEncounter.Id;
        CurrentParty.ActiveBoss = ActiveEncounter;

        // 6. Tasks
        Tasks = new List<TaskItem>
        {
            // Dailies
            new TaskItem { UserId = helgeId, Title = "Daily Review", Description = "Review today's lecture notes and summarize in Obsidian", TaskType = TaskType.Daily, Difficulty = Difficulty.Easy, CourseLabel = "CS 301", StreakCount = 7 },
            new TaskItem { UserId = helgeId, Title = "Flashcard Drill", Description = "Complete 25 spaced repetition flashcards", TaskType = TaskType.Daily, Difficulty = Difficulty.Easy, CourseLabel = "MATH 240", StreakCount = 5 },
            new TaskItem { UserId = helgeId, Title = "Class Attendance", Description = "Attend all scheduled university classes on time", TaskType = TaskType.Daily, Difficulty = Difficulty.Easy, StreakCount = 12 },
            
            // Quests
            new TaskItem { UserId = helgeId, Title = "Finish Automata Problem Set", Description = "Solve questions 1-15 on DFA to NFA and regex equivalence", TaskType = TaskType.Quest, Difficulty = Difficulty.Hard, CourseLabel = "CS 301", DueDate = DateTime.UtcNow.AddDays(2) },
            new TaskItem { UserId = helgeId, Title = "Database Schema Design & ERD", Description = "Build normalized relational schema for the hospital information system", TaskType = TaskType.Quest, Difficulty = Difficulty.Medium, CourseLabel = "CS 302", DueDate = DateTime.UtcNow.AddDays(4) },
            new TaskItem { UserId = helgeId, Title = "Calculus Integration Worksheet", Description = "Solve integration by parts and trigonometric substitution drills", TaskType = TaskType.Quest, Difficulty = Difficulty.Easy, CourseLabel = "MATH 240", DueDate = DateTime.UtcNow.AddDays(1) },
            new TaskItem { UserId = helgeId, Title = "Final Project Architecture Proposal", Description = "Write complete architecture specification with component & sequence diagrams", TaskType = TaskType.Quest, Difficulty = Difficulty.Exam, CourseLabel = "CS 301", DueDate = DateTime.UtcNow.AddDays(6) }
        };

        // 7. Combat Log
        CombatLog = new List<CombatLogEntry>
        {
            new CombatLogEntry { PartyId = partyId, UserId = helgeId, Username = "Helge", EventType = "damage", Message = "Helge dealt 25 DMG with 'Database Normalization Exercise'!", Value = 25, CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new CombatLogEntry { PartyId = partyId, UserId = ariaId, Username = "Aria", EventType = "heal", Message = "Aria restored 15 HP to Riven with Healing Light!", Value = 15, CreatedAt = DateTime.UtcNow.AddHours(-2.5) },
            new CombatLogEntry { PartyId = partyId, UserId = rivenId, Username = "Riven", EventType = "critical", Message = "Riven dealt 42 CRIT DMG via Focus: 'Linear Algebra Matrix Inversion'!", Value = 42, CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new CombatLogEntry { PartyId = partyId, EventType = "boss_attack", Message = "Syntax Error Golem retaliates! Party takes 15 DMG from missed daily reviews!", Value = 15, CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new CombatLogEntry { PartyId = partyId, UserId = mariaId, Username = "Maria", EventType = "damage", Message = "Maria dealt 32 DMG with 'Data Structures Lab 4' (+8 Gold bonus)!", Value = 32, CreatedAt = DateTime.UtcNow.AddMinutes(-45) },
            new CombatLogEntry { PartyId = partyId, UserId = helgeId, Username = "Helge", EventType = "damage", Message = "Helge dealt 35 DMG with 'Finish Automata Homework'!", Value = 35, CreatedAt = DateTime.UtcNow.AddMinutes(-20) },
            new CombatLogEntry { PartyId = partyId, UserId = helgeId, Username = "Helge", EventType = "skill_use", Message = "Helge activated Power Strike! Next attack deals 1.5x damage!", Value = 0, CreatedAt = DateTime.UtcNow.AddMinutes(-5) }
        };

        // 8. Shop items
        ShopItems = new List<ShopItem>
        {
            new ShopItem { Name = "Wooden Sword", Description = "A humble training blade. Every hero starts somewhere.", Category = "weapon", GoldCost = 0, StatBonus = new Dictionary<string, int> { { "attack", 2 } }, RequiredLevel = 1 },
            new ShopItem { Name = "Iron Broadsword", Description = "Reliable steel for crunch-time cramming sessions.", Category = "weapon", GoldCost = 100, StatBonus = new Dictionary<string, int> { { "attack", 5 } }, RequiredLevel = 3 },
            new ShopItem { Name = "Scholar's Staff", Description = "Channels arcane study focus into devastating critical strikes.", Category = "weapon", GoldCost = 150, StatBonus = new Dictionary<string, int> { { "attack", 4 }, { "max_mp", 10 } }, RequiredLevel = 3 },
            new ShopItem { Name = "Leather Cap", Description = "Basic head protection against GPA drops.", Category = "helmet", GoldCost = 50, StatBonus = new Dictionary<string, int> { { "max_hp", 5 } }, RequiredLevel = 1 },
            new ShopItem { Name = "Mage's Hood", Description = "Enhances concentration and deep focus stamina.", Category = "helmet", GoldCost = 120, StatBonus = new Dictionary<string, int> { { "max_mp", 15 } }, RequiredLevel = 4 },
            new ShopItem { Name = "Chainmail Vest", Description = "Sturdy armor forged to withstand grueling exam weeks.", Category = "armor", GoldCost = 200, StatBonus = new Dictionary<string, int> { { "max_hp", 20 } }, RequiredLevel = 4 },
            new ShopItem { Name = "HP Potion (Small)", Description = "Restores 15 HP immediately. Tastes like citrus energy drink.", Category = "potion", GoldCost = 30, StatBonus = new Dictionary<string, int> { { "hp_restore", 15 } }, RequiredLevel = 1, IsConsumable = true },
            new ShopItem { Name = "HP Potion (Large)", Description = "Restores 40 HP immediately. Infused with double espresso.", Category = "potion", GoldCost = 80, StatBonus = new Dictionary<string, int> { { "hp_restore", 40 } }, RequiredLevel = 2, IsConsumable = true },
            new ShopItem { Name = "XP Scroll", Description = "Grants 50 bonus XP instantly upon reading.", Category = "potion", GoldCost = 100, StatBonus = new Dictionary<string, int> { { "xp_bonus", 50 } }, RequiredLevel = 1, IsConsumable = true },
            new ShopItem { Name = "Streak Freeze Shield", Description = "Shields your daily habit streak from breaking for one missed day.", Category = "shield", GoldCost = 150, StatBonus = new Dictionary<string, int> { { "streak_protection", 1 } }, RequiredLevel = 1, IsConsumable = true },
            new ShopItem { Name = "Golden Quill", Description = "A glittering heirloom quill marking a peerless scholar.", Category = "cosmetic", GoldCost = 300, StatBonus = new Dictionary<string, int>(), RequiredLevel = 5 },
            new ShopItem { Name = "Phoenix Feather Cape", Description = "Grants emergency auto-revive if your HP reaches 0.", Category = "shield", GoldCost = 450, StatBonus = new Dictionary<string, int> { { "auto_revive", 1 } }, RequiredLevel = 5, IsConsumable = true }
        };

        // 9. Inventory - Helge starts with Wooden Sword equipped
        Inventory = new List<InventoryItem>
        {
            new InventoryItem { UserId = helgeId, ItemId = ShopItems[0].Id, Item = ShopItems[0], IsEquipped = true, Quantity = 1 },
            new InventoryItem { UserId = helgeId, ItemId = ShopItems[6].Id, Item = ShopItems[6], IsEquipped = false, Quantity = 2 }
        };
    }

    public int CalculateDamage(Difficulty diff, bool isFocus = false)
    {
        int baseDmg = diff switch
        {
            Difficulty.Easy => 10,
            Difficulty.Medium => 20,
            Difficulty.Hard => 35,
            Difficulty.Exam => 50,
            _ => 10
        };
        baseDmg += CurrentUser.Attack / 2;

        double multiplier = 1.0;
        // Warrior bonus: +50% damage on Hard/Exam
        if (CurrentUser.Class == "Warrior" && (diff == Difficulty.Hard || diff == Difficulty.Exam))
            multiplier = 1.5;

        // Rogue bonus: +10% damage always
        if (CurrentUser.Class == "Rogue")
            multiplier *= 1.1;

        if (isFocus)
        {
            multiplier *= 1.25; // critical strike
            if (CurrentUser.Class == "Mage")
                multiplier *= 1.3;
        }

        return Math.Max(5, (int)(baseDmg * multiplier));
    }

    public int CalculateGold(Difficulty diff)
    {
        int baseGold = diff switch
        {
            Difficulty.Easy => 5,
            Difficulty.Medium => 10,
            Difficulty.Hard => 20,
            Difficulty.Exam => 35,
            _ => 5
        };
        if (CurrentUser.Class == "Rogue")
            baseGold = (int)(baseGold * 1.25);
        return baseGold;
    }

    public int CalculateXp(Difficulty diff, bool isFocus = false)
    {
        int baseXp = diff switch
        {
            Difficulty.Easy => 10,
            Difficulty.Medium => 20,
            Difficulty.Hard => 35,
            Difficulty.Exam => 50,
            _ => 10
        };
        if (CurrentUser.Class == "Mage" && isFocus)
            baseXp = (int)(baseXp * 1.3);

        if (HasArcaneFocusBuff)
        {
            baseXp *= 2;
            HasArcaneFocusBuff = false; // consumed
        }

        return baseXp;
    }

    public void CompleteTask(Guid taskId)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null || task.IsCompleted) return;

        task.IsCompleted = true;
        task.CompletedAt = DateTime.UtcNow;

        int damage = CalculateDamage(task.Difficulty);
        int xp = CalculateXp(task.Difficulty);
        int gold = CalculateGold(task.Difficulty);

        task.DamageDealt = damage;
        task.XpReward = xp;
        task.GoldReward = gold;

        CurrentUser.Xp += xp;
        CurrentUser.Gold += gold;

        if (task.TaskType == TaskType.Daily)
        {
            task.StreakCount++;
            CurrentUser.StreakDays++;
        }

        DealDamageToBoss(damage, $"{CurrentUser.Username} dealt {damage} DMG with '{task.Title}'!");

        CheckLevelUp();
        NotifyStateChanged();
    }

    public void StartFocusSession(int minutes, string subject)
    {
        ActiveFocusSession = new FocusSession
        {
            UserId = CurrentUser.Id,
            DurationMinutes = minutes,
            SubjectLabel = string.IsNullOrWhiteSpace(subject) ? "Deep Work" : subject,
            StartedAt = DateTime.UtcNow
        };
        NotifyStateChanged();
    }

    public void CompleteFocusSession()
    {
        int minutes = ActiveFocusSession?.DurationMinutes ?? 25;
        string subject = ActiveFocusSession?.SubjectLabel ?? "Deep Focus";

        int baseDmg = minutes >= 50 ? 55 : 30;
        baseDmg += CurrentUser.Attack / 3;
        double multiplier = 1.25; // critical strike
        if (CurrentUser.Class == "Mage") multiplier *= 1.3;
        int damage = (int)(baseDmg * multiplier);

        int mpRestore = minutes >= 50 ? 10 : 5;
        int xpEarn = minutes >= 50 ? 40 : 20;
        if (CurrentUser.Class == "Mage") xpEarn = (int)(xpEarn * 1.3);
        if (HasArcaneFocusBuff)
        {
            xpEarn *= 2;
            HasArcaneFocusBuff = false;
        }

        CurrentUser.Mp = Math.Min(CurrentUser.Mp + mpRestore, CurrentUser.MaxMp);
        CurrentUser.Xp += xpEarn;

        DealDamageToBoss(damage, $"⚡ {CurrentUser.Username} landed a CRITICAL STRIKE of {damage} DMG via Focus: '{subject}'! (+{mpRestore} MP)");

        ActiveFocusSession = null;
        CheckLevelUp();
        NotifyStateChanged();
    }

    public void DealDamageToBoss(int amount, string message)
    {
        if (ActiveEncounter == null || ActiveEncounter.IsDefeated) return;

        ActiveEncounter.CurrentHp = Math.Max(0, ActiveEncounter.CurrentHp - amount);

        var logEntry = new CombatLogEntry
        {
            PartyId = CurrentParty?.Id ?? Guid.Empty,
            EncounterId = ActiveEncounter.Id,
            UserId = CurrentUser.Id,
            Username = CurrentUser.Username,
            EventType = message.Contains("CRITICAL") || message.Contains("CRIT") ? "critical" : "damage",
            Message = message,
            Value = amount,
            CreatedAt = DateTime.UtcNow
        };
        CombatLog.Add(logEntry);

        if (ActiveEncounter.CurrentHp <= 0)
        {
            ActiveEncounter.IsDefeated = true;
            ActiveEncounter.DefeatedAt = DateTime.UtcNow;
            var boss = ActiveEncounter.Boss;
            if (boss != null)
            {
                int xpReward = boss.XpReward;
                int goldReward = boss.GoldReward;
                CurrentUser.Xp += xpReward;
                CurrentUser.Gold += goldReward;

                CombatLog.Add(new CombatLogEntry
                {
                    PartyId = CurrentParty?.Id ?? Guid.Empty,
                    EventType = "boss_defeat",
                    Message = $"🏆 {boss.Name} has been SLAIN! Party earned {xpReward} XP and {goldReward} Gold!",
                    Value = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }

    public void SimulateBossAttack()
    {
        if (ActiveEncounter == null || ActiveEncounter.IsDefeated) return;

        int attackPower = ActiveEncounter.Boss?.Attack ?? 15;
        CurrentUser.Hp = Math.Max(1, CurrentUser.Hp - attackPower);

        if (CurrentParty != null)
        {
            foreach (var member in CurrentParty.Members)
            {
                if (member.Id != CurrentUser.Id)
                {
                    member.Hp = Math.Max(1, member.Hp - attackPower);
                }
            }
        }

        CombatLog.Add(new CombatLogEntry
        {
            PartyId = CurrentParty?.Id ?? Guid.Empty,
            EncounterId = ActiveEncounter.Id,
            EventType = "boss_attack",
            Message = $"💥 {ActiveEncounter.Boss?.Name ?? "The Boss"} unleashes Retaliation! Everyone in the party took {attackPower} DMG!",
            Value = attackPower,
            CreatedAt = DateTime.UtcNow
        });

        NotifyStateChanged();
    }

    public void AddTask(TaskItem task)
    {
        task.UserId = CurrentUser.Id;
        Tasks.Add(task);
        NotifyStateChanged();
    }

    public void UpdateTask(TaskItem task)
    {
        var idx = Tasks.FindIndex(t => t.Id == task.Id);
        if (idx >= 0) Tasks[idx] = task;
        NotifyStateChanged();
    }

    public void DeleteTask(Guid taskId)
    {
        Tasks.RemoveAll(t => t.Id == taskId);
        NotifyStateChanged();
    }

    public void SelectBoss(Guid bossId)
    {
        var boss = Bosses.FirstOrDefault(b => b.Id == bossId);
        if (boss == null) return;

        int partySize = CurrentParty?.Members.Count ?? 1;
        int scaledHp = (int)(boss.BaseHp * (1 + 0.25 * (partySize - 1)));

        ActiveEncounter = new BossEncounter
        {
            PartyId = CurrentParty?.Id ?? Guid.Empty,
            BossId = boss.Id,
            Boss = boss,
            CurrentHp = scaledHp,
            MaxHp = scaledHp
        };

        if (CurrentParty != null)
        {
            CurrentParty.ActiveBossId = ActiveEncounter.Id;
            CurrentParty.ActiveBoss = ActiveEncounter;
        }

        CombatLog.Add(new CombatLogEntry
        {
            PartyId = CurrentParty?.Id ?? Guid.Empty,
            EncounterId = ActiveEncounter.Id,
            EventType = "damage",
            Message = $"⚔️ The party engaged {boss.Name}! ({scaledHp} HP)",
            Value = 0,
            CreatedAt = DateTime.UtcNow
        });

        NotifyStateChanged();
    }

    public void PurchaseItem(Guid itemId)
    {
        var item = ShopItems.FirstOrDefault(i => i.Id == itemId);
        if (item == null || CurrentUser.Gold < item.GoldCost) return;
        if (CurrentUser.Level < item.RequiredLevel) return;

        var existing = Inventory.FirstOrDefault(i => i.ItemId == itemId);
        if (existing != null && !item.IsConsumable) return; // already owned non-consumable

        CurrentUser.Gold -= item.GoldCost;

        if (existing != null && item.IsConsumable)
        {
            existing.Quantity++;
        }
        else
        {
            Inventory.Add(new InventoryItem
            {
                UserId = CurrentUser.Id,
                ItemId = item.Id,
                Item = item,
                Quantity = 1
            });
        }
        NotifyStateChanged();
    }

    public void EquipItem(Guid inventoryId)
    {
        var inv = Inventory.FirstOrDefault(i => i.Id == inventoryId);
        if (inv?.Item == null) return;

        if (inv.Item.IsConsumable)
        {
            // Use consumable
            foreach (var kvp in inv.Item.StatBonus)
            {
                switch (kvp.Key)
                {
                    case "hp_restore":
                        CurrentUser.Hp = Math.Min(CurrentUser.Hp + kvp.Value, CurrentUser.MaxHp);
                        CombatLog.Add(new CombatLogEntry
                        {
                            PartyId = CurrentParty?.Id ?? Guid.Empty,
                            UserId = CurrentUser.Id,
                            Username = CurrentUser.Username,
                            EventType = "heal",
                            Message = $"🧪 {CurrentUser.Username} drank {inv.Item.Name} and recovered {kvp.Value} HP!",
                            Value = kvp.Value,
                            CreatedAt = DateTime.UtcNow
                        });
                        break;
                    case "xp_bonus":
                        CurrentUser.Xp += kvp.Value;
                        CombatLog.Add(new CombatLogEntry
                        {
                            PartyId = CurrentParty?.Id ?? Guid.Empty,
                            UserId = CurrentUser.Id,
                            Username = CurrentUser.Username,
                            EventType = "revive",
                            Message = $"✨ {CurrentUser.Username} consumed {inv.Item.Name} and gained {kvp.Value} XP!",
                            Value = kvp.Value,
                            CreatedAt = DateTime.UtcNow
                        });
                        CheckLevelUp();
                        break;
                }
            }
            inv.Quantity--;
            if (inv.Quantity <= 0)
                Inventory.Remove(inv);
        }
        else
        {
            // Toggle equip
            var sameCategory = Inventory.Where(i => i.Item?.Category == inv.Item.Category && i.Id != inv.Id && i.IsEquipped);
            foreach (var other in sameCategory)
            {
                other.IsEquipped = false;
                RemoveStatBonus(other.Item);
            }

            inv.IsEquipped = !inv.IsEquipped;
            if (inv.IsEquipped)
            {
                ApplyStatBonus(inv.Item);
            }
            else
            {
                RemoveStatBonus(inv.Item);
            }
        }
        NotifyStateChanged();
    }

    private void ApplyStatBonus(ShopItem? item)
    {
        if (item == null) return;
        foreach (var stat in item.StatBonus)
        {
            if (stat.Key == "attack") CurrentUser.Attack += stat.Value;
            if (stat.Key == "max_hp") { CurrentUser.MaxHp += stat.Value; CurrentUser.Hp += stat.Value; }
            if (stat.Key == "max_mp") { CurrentUser.MaxMp += stat.Value; CurrentUser.Mp += stat.Value; }
        }
    }

    private void RemoveStatBonus(ShopItem? item)
    {
        if (item == null) return;
        foreach (var stat in item.StatBonus)
        {
            if (stat.Key == "attack") CurrentUser.Attack = Math.Max(5, CurrentUser.Attack - stat.Value);
            if (stat.Key == "max_hp") { CurrentUser.MaxHp = Math.Max(20, CurrentUser.MaxHp - stat.Value); CurrentUser.Hp = Math.Min(CurrentUser.Hp, CurrentUser.MaxHp); }
            if (stat.Key == "max_mp") { CurrentUser.MaxMp = Math.Max(10, CurrentUser.MaxMp - stat.Value); CurrentUser.Mp = Math.Min(CurrentUser.Mp, CurrentUser.MaxMp); }
        }
    }

    public void UseSkill(string skillName)
    {
        switch (skillName)
        {
            case "Power Strike" when CurrentUser.Mp >= 10:
                CurrentUser.Mp -= 10;
                int psDmg = (int)(CurrentUser.Attack * 1.5);
                DealDamageToBoss(psDmg, $"{CurrentUser.Username} executed Power Strike for {psDmg} DMG!");
                break;

            case "Arcane Focus" when CurrentUser.Mp >= 15:
                CurrentUser.Mp -= 15;
                HasArcaneFocusBuff = true;
                CombatLog.Add(new CombatLogEntry
                {
                    PartyId = CurrentParty?.Id ?? Guid.Empty,
                    UserId = CurrentUser.Id,
                    Username = CurrentUser.Username,
                    EventType = "skill_use",
                    Message = $"🔮 {CurrentUser.Username} cast Arcane Focus! Next focus session awards 2x XP!",
                    CreatedAt = DateTime.UtcNow
                });
                break;

            case "Healing Light" when CurrentUser.Mp >= 20:
                CurrentUser.Mp -= 20;
                if (CurrentParty != null && CurrentParty.Members.Any())
                {
                    var lowest = CurrentParty.Members.OrderBy(m => (double)m.Hp / m.MaxHp).First();
                    lowest.Hp = Math.Min(lowest.Hp + 20, lowest.MaxHp);
                    CombatLog.Add(new CombatLogEntry
                    {
                        PartyId = CurrentParty.Id,
                        UserId = CurrentUser.Id,
                        Username = CurrentUser.Username,
                        EventType = "heal",
                        Message = $"💚 {CurrentUser.Username} cast Healing Light on {lowest.Username}, restoring 20 HP!",
                        Value = 20,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                break;

            case "Gold Rush" when CurrentUser.Mp >= 10:
                CurrentUser.Mp -= 10;
                CurrentUser.Gold += 35;
                CombatLog.Add(new CombatLogEntry
                {
                    PartyId = CurrentParty?.Id ?? Guid.Empty,
                    UserId = CurrentUser.Id,
                    Username = CurrentUser.Username,
                    EventType = "skill_use",
                    Message = $"💰 {CurrentUser.Username} activated Gold Rush and scavenged 35 Gold!",
                    Value = 35,
                    CreatedAt = DateTime.UtcNow
                });
                break;
        }
        NotifyStateChanged();
    }

    public void CreateParty(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var newPartyId = Guid.NewGuid();
        CurrentParty = new Party
        {
            Id = newPartyId,
            Name = name,
            InviteCode = "QUEST" + new Random().Next(100, 999),
            LeaderId = CurrentUser.Id,
            Members = new List<CharacterProfile> { CurrentUser }
        };
        CurrentUser.PartyId = newPartyId;
        CombatLog.Add(new CombatLogEntry
        {
            PartyId = newPartyId,
            EventType = "revive",
            Message = $"🛡️ Party '{name}' formed by {CurrentUser.Username}!",
            CreatedAt = DateTime.UtcNow
        });
        NotifyStateChanged();
    }

    public void JoinParty(string inviteCode)
    {
        if (string.IsNullOrWhiteSpace(inviteCode)) return;
        if (CurrentParty != null && CurrentParty.InviteCode.Equals(inviteCode.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Mock joining
        var joinedPartyId = Guid.NewGuid();
        CurrentParty = new Party
        {
            Id = joinedPartyId,
            Name = "Fellowship of " + inviteCode.ToUpper(),
            InviteCode = inviteCode.ToUpper(),
            LeaderId = Guid.NewGuid(),
            Members = new List<CharacterProfile>
            {
                new CharacterProfile { Username = "PartyLeader", Class = "Mage", Level = 5, Hp = 45, MaxHp = 50 },
                CurrentUser
            }
        };
        CurrentUser.PartyId = joinedPartyId;
        NotifyStateChanged();
    }

    public void LeaveParty()
    {
        CurrentUser.PartyId = null;
        CurrentParty = null;
        ActiveEncounter = null;
        NotifyStateChanged();
    }

    private void CheckLevelUp()
    {
        while (CurrentUser.Xp >= CurrentUser.XpToNext)
        {
            CurrentUser.Xp -= CurrentUser.XpToNext;
            CurrentUser.Level++;
            CurrentUser.XpToNext = 100 * CurrentUser.Level;
            CurrentUser.MaxHp += 10;
            CurrentUser.MaxMp += 5;
            CurrentUser.Attack += 2;
            CurrentUser.Hp = CurrentUser.MaxHp;
            CurrentUser.Mp = CurrentUser.MaxMp;

            CombatLog.Add(new CombatLogEntry
            {
                PartyId = CurrentParty?.Id ?? Guid.Empty,
                UserId = CurrentUser.Id,
                Username = CurrentUser.Username,
                EventType = "revive",
                Message = $"🎉 LEVEL UP! {CurrentUser.Username} reached Level {CurrentUser.Level}! (+10 Max HP, +5 Max MP, +2 ATK)",
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    public void NotifyStateChanged() => OnStateChanged?.Invoke();
}
