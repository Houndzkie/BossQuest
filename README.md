# ⚔️ BossQuest — Gamified Student Productivity & Co-op RPG

A gamified student productivity web application inspired by Habitica, tailored specifically for college students managing study sessions, assignments, and exams with a party of friends.

---

## 🚀 Quick Start

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)

### Run Locally
```bash
cd BossQuest
dotnet run
```
Then navigate to the displayed localhost URL (typically `http://localhost:5000` or `https://localhost:5001`).

---

## 🎮 Core Gameplay Mechanics

### 1. Character Attributes & Progression
* **HP (Health Points):** Starts at 50/50. Depleted when the party misses daily habits or when a boss counter-attacks.
* **MP (Mana Points):** Spent to cast unique class abilities (Power Strike, Arcane Focus, Healing Light, Gold Rush). Restored via Pomodoro Focus Raids and potions.
* **XP (Experience Points):** Earned by finishing tasks and Pomodoro sessions. Leveling up increases Max HP (+10), Max MP (+5), Attack (+2), and restores full health.
* **Gold:** Earned from homework quests and class perks; spent in the Tavern & Armory.

### 2. Three Task Categories
1. **Dailies (Recurring Habits):** Daily review, flashcards, or class attendance. Missing a daily triggers a Boss Retaliation strike against the entire party.
2. **Quests (Assignments & Deadlines):** Homework and course projects with difficulty tiers:
   - **Easy:** 10 base DMG • 10 XP • 5 Gold
   - **Medium:** 20 base DMG • 20 XP • 10 Gold
   - **Hard:** 35 base DMG • 35 XP • 20 Gold
   - **Exam-Level:** 50 base DMG • 50 XP • 35 Gold
3. **Focus Raids (Pomodoro Study Timer):** Built-in 25/5 or 50/10 study timer. Completing uninterrupted focus triggers a **Critical Strike** dealing ~35–65 DMG directly to the Boss and replenishes MP.

### 3. Class System (Unlocked at Level 3)
* ⚔️ **Warrior (Crammer):** Deals **+50% bonus damage** on Hard and Exam-level assignments.
* 🔮 **Mage (Scholar):** Generates **+30% bonus XP and MP** from Pomodoro focus blocks. Has the *Arcane Focus* skill (doubles next focus XP).
* 💚 **Healer (Support):** Can spend 20 MP on *Healing Light* to heal the lowest-HP party member before the Boss strikes.
* 🗡️ **Rogue (Sprinter):** Receives **+25% bonus Gold drops** and boosted streak multipliers. Has the *Gold Rush* skill.

### 4. Boss Encounters & Bestiary
* **The Procrastination Hydra** (800 HP, 12 ATK) — *Normal*
* **Midterm Lich** (1,200 HP, 18 ATK) — *Elite*
* **Syntax Error Golem** (1,000 HP, 15 ATK) — *Normal*
* **Deadline Dragon** (1,500 HP, 22 ATK) — *Legendary*
* **All-Nighter Specter** (600 HP, 10 ATK) — *Normal*
* **Thesis Titan** (2,000 HP, 25 ATK) — *Legendary*
* *Dynamic Scaling:* Boss HP automatically scales by **+25% per additional party member**.

---

## 🗄️ Supabase PostgreSQL Backend & RLS

The database schema is provided in `supabase_schema.sql`. It includes:
1. `profiles` — Player stats, level, class, streak, and gear.
2. `parties` — Co-op groups and invite codes.
3. `bosses` & `boss_encounters` — Bestiary and active boss instances.
4. `tasks` — Dailies and quests with difficulty ratings.
5. `focus_sessions` — Pomodoro logs with damage and MP recovery metrics.
6. `combat_logs` — Live combat events stream.
7. `shop_items` & `inventory` — Gear, potions, and equip state.
8. Full Row Level Security (RLS) policies and Realtime publication configuration.

---

## 🧪 Explicit Testing IDs Reference

| Component | Element ID | Purpose |
|---|---|---|
| **Character Card** | `textviewHp`, `textviewMp`, `textviewXp`, `textviewGold` | Stat counters |
| **Class Skills** | `buttonSkillPowerStrike`, `buttonSkillArcaneFocus`, `buttonSkillHealingLight`, `buttonSkillGoldRush` | Class skill buttons |
| **Pomodoro Timer** | `textviewTimer`, `edittextFocusSubject`, `buttonTimer25`, `buttonTimer50` | Timer controls |
| **Timer Actions** | `buttonStartFocus`, `buttonPauseFocus`, `buttonResumeFocus`, `buttonStopFocus` | Playback triggers |
| **Task Board** | `tabDailies`, `tabQuests`, `buttonAddTask` | Navigation & creation |
| **Task Items** | `buttonCompleteTask_{id}`, `buttonEditTask_{id}`, `buttonDeleteTask_{id}` | Task row actions |
| **Task Modal** | `edittextTaskTitle`, `edittextTaskDescription`, `buttonTypeDaily`, `buttonTypeQuest`, `buttonDiffEasy`... | Form inputs |
| **Boss Arena** | `textviewBossHp`, `buttonOpenParty`, `buttonOpenBestiary`, `listCombatLog` | Boss battle views |
| **Party Lobby** | `buttonCreateParty`, `buttonJoinParty`, `buttonLeaveParty`, `buttonCopyInviteCode` | Co-op lobby |
| **Shop & Tavern** | `textviewShopGold`, `tabShopAll`..., `buttonBuyItem_{name}`, `buttonEquipItem_{name}` | Shop & Inventory |
| **Auth Page** | `tabLogin`, `tabSignUp`, `edittextEmail`, `edittextPassword`, `buttonLogin`, `buttonSignUp` | Authentication |
