-- ============================================================
-- BossQuest – Supabase PostgreSQL Schema
-- A gamified student productivity RPG
-- ============================================================

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ============================================================
-- 1. PROFILES – character sheet for every authenticated user
-- ============================================================
CREATE TABLE public.profiles (
    id              UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
    username        TEXT UNIQUE NOT NULL,
    avatar_url      TEXT DEFAULT '/images/avatars/default.png',
    class           TEXT DEFAULT 'None' CHECK (class IN ('None','Warrior','Mage','Healer','Rogue')),
    level           INT  DEFAULT 1  CHECK (level >= 1),
    xp              INT  DEFAULT 0  CHECK (xp >= 0),
    xp_to_next      INT  DEFAULT 100,
    hp              INT  DEFAULT 50 CHECK (hp >= 0),
    max_hp          INT  DEFAULT 50,
    mp              INT  DEFAULT 30 CHECK (mp >= 0),
    max_mp          INT  DEFAULT 30,
    gold            INT  DEFAULT 0  CHECK (gold >= 0),
    attack          INT  DEFAULT 10,
    streak_days     INT  DEFAULT 0,
    party_id        UUID,                -- FK added after parties table
    equipped_gear   JSONB DEFAULT '{}',  -- { "head": "...", "weapon": "...", ... }
    created_at      TIMESTAMPTZ DEFAULT now(),
    updated_at      TIMESTAMPTZ DEFAULT now()
);

-- ============================================================
-- 2. PARTIES – co-op groups (up to 6 members)
-- ============================================================
CREATE TABLE public.parties (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name            TEXT NOT NULL,
    invite_code     TEXT UNIQUE NOT NULL DEFAULT substr(md5(random()::text), 1, 8),
    leader_id       UUID NOT NULL REFERENCES public.profiles(id),
    max_members     INT  DEFAULT 6,
    active_boss_id  UUID,                -- FK added after bosses table
    created_at      TIMESTAMPTZ DEFAULT now()
);

-- Now add the FK from profiles → parties
ALTER TABLE public.profiles
    ADD CONSTRAINT fk_profiles_party
    FOREIGN KEY (party_id) REFERENCES public.parties(id) ON DELETE SET NULL;

-- ============================================================
-- 3. BOSSES – bestiary of monsters the party can fight
-- ============================================================
CREATE TABLE public.bosses (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name            TEXT NOT NULL,
    description     TEXT,
    image_url       TEXT DEFAULT '/images/bosses/default.png',
    base_hp         INT  NOT NULL DEFAULT 1000,
    attack          INT  NOT NULL DEFAULT 15,
    tier            TEXT DEFAULT 'Normal' CHECK (tier IN ('Normal','Elite','Legendary')),
    xp_reward       INT  DEFAULT 200,
    gold_reward     INT  DEFAULT 100,
    is_template     BOOLEAN DEFAULT TRUE   -- TRUE = bestiary entry; FALSE = active instance
);

-- ============================================================
-- 4. ACTIVE BOSS ENCOUNTERS – per-party boss instances
-- ============================================================
CREATE TABLE public.boss_encounters (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    party_id        UUID NOT NULL REFERENCES public.parties(id) ON DELETE CASCADE,
    boss_id         UUID NOT NULL REFERENCES public.bosses(id),
    current_hp      INT  NOT NULL,
    max_hp          INT  NOT NULL,
    is_defeated     BOOLEAN DEFAULT FALSE,
    started_at      TIMESTAMPTZ DEFAULT now(),
    defeated_at     TIMESTAMPTZ
);

-- FK from parties → boss_encounters
ALTER TABLE public.parties
    ADD CONSTRAINT fk_parties_active_boss
    FOREIGN KEY (active_boss_id) REFERENCES public.boss_encounters(id) ON DELETE SET NULL;

-- ============================================================
-- 5. TASKS – dailies and quests
-- ============================================================
CREATE TYPE task_type AS ENUM ('daily', 'quest');
CREATE TYPE difficulty AS ENUM ('easy', 'medium', 'hard', 'exam');

CREATE TABLE public.tasks (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id         UUID NOT NULL REFERENCES public.profiles(id) ON DELETE CASCADE,
    title           TEXT NOT NULL,
    description     TEXT,
    task_type       task_type NOT NULL DEFAULT 'quest',
    difficulty      difficulty NOT NULL DEFAULT 'medium',
    course_label    TEXT,              -- e.g. "CS 301", "MATH 240"
    due_date        TIMESTAMPTZ,       -- for quests
    is_completed    BOOLEAN DEFAULT FALSE,
    completed_at    TIMESTAMPTZ,
    damage_dealt    INT DEFAULT 0,     -- calculated on completion
    xp_reward       INT DEFAULT 0,
    gold_reward     INT DEFAULT 0,
    streak_count    INT DEFAULT 0,     -- for dailies
    created_at      TIMESTAMPTZ DEFAULT now()
);

-- ============================================================
-- 6. FOCUS SESSIONS – pomodoro study timer records
-- ============================================================
CREATE TABLE public.focus_sessions (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id         UUID NOT NULL REFERENCES public.profiles(id) ON DELETE CASCADE,
    duration_minutes INT NOT NULL DEFAULT 25,
    subject_label   TEXT,
    started_at      TIMESTAMPTZ DEFAULT now(),
    ended_at        TIMESTAMPTZ,
    is_completed    BOOLEAN DEFAULT FALSE,
    damage_dealt    INT DEFAULT 0,     -- critical strike damage
    xp_earned       INT DEFAULT 0,
    mp_restored     INT DEFAULT 0,
    created_at      TIMESTAMPTZ DEFAULT now()
);

-- ============================================================
-- 7. COMBAT LOG – real-time party combat feed
-- ============================================================
CREATE TABLE public.combat_logs (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    party_id        UUID NOT NULL REFERENCES public.parties(id) ON DELETE CASCADE,
    encounter_id    UUID REFERENCES public.boss_encounters(id) ON DELETE CASCADE,
    user_id         UUID REFERENCES public.profiles(id) ON DELETE SET NULL,
    event_type      TEXT NOT NULL CHECK (event_type IN (
        'damage',       -- player deals damage
        'boss_attack',  -- boss retaliates (daily miss)
        'heal',         -- healer restores HP
        'critical',     -- focus session critical strike
        'boss_defeat',  -- boss killed
        'player_down',  -- player reached 0 HP
        'revive',       -- player revived
        'skill_use'     -- class skill used
    )),
    message         TEXT NOT NULL,
    value           INT DEFAULT 0,     -- damage/heal amount
    created_at      TIMESTAMPTZ DEFAULT now()
);

-- ============================================================
-- 8. SHOP ITEMS – gear and consumables
-- ============================================================
CREATE TABLE public.shop_items (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name            TEXT NOT NULL,
    description     TEXT,
    category        TEXT NOT NULL CHECK (category IN ('weapon','armor','helmet','potion','shield','cosmetic')),
    image_url       TEXT DEFAULT '/images/items/default.png',
    gold_cost       INT NOT NULL DEFAULT 50,
    stat_bonus      JSONB DEFAULT '{}',   -- { "attack": 5, "max_hp": 10, ... }
    required_level  INT DEFAULT 1,
    required_class  TEXT DEFAULT 'Any',
    is_consumable   BOOLEAN DEFAULT FALSE
);

-- ============================================================
-- 9. INVENTORY – items owned by users
-- ============================================================
CREATE TABLE public.inventory (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id         UUID NOT NULL REFERENCES public.profiles(id) ON DELETE CASCADE,
    item_id         UUID NOT NULL REFERENCES public.shop_items(id) ON DELETE CASCADE,
    quantity        INT DEFAULT 1 CHECK (quantity >= 0),
    is_equipped     BOOLEAN DEFAULT FALSE,
    purchased_at    TIMESTAMPTZ DEFAULT now(),
    UNIQUE(user_id, item_id)
);

-- ============================================================
-- 10. ROW LEVEL SECURITY (RLS)
-- ============================================================

-- Enable RLS on all tables
ALTER TABLE public.profiles        ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.parties         ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.bosses          ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.boss_encounters ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.tasks           ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.focus_sessions  ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.combat_logs     ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.shop_items      ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.inventory       ENABLE ROW LEVEL SECURITY;

-- ── PROFILES ────────────────────────────────────────────────
CREATE POLICY "Users can view all profiles"
    ON public.profiles FOR SELECT USING (true);

CREATE POLICY "Users can update own profile"
    ON public.profiles FOR UPDATE USING (auth.uid() = id);

CREATE POLICY "Users can insert own profile"
    ON public.profiles FOR INSERT WITH CHECK (auth.uid() = id);

-- ── PARTIES ─────────────────────────────────────────────────
CREATE POLICY "Party members can view their party"
    ON public.parties FOR SELECT USING (
        id IN (SELECT party_id FROM public.profiles WHERE id = auth.uid())
        OR leader_id = auth.uid()
    );

CREATE POLICY "Authenticated users can create parties"
    ON public.parties FOR INSERT WITH CHECK (auth.uid() = leader_id);

CREATE POLICY "Party leader can update party"
    ON public.parties FOR UPDATE USING (leader_id = auth.uid());

-- ── BOSSES (read-only bestiary for all) ─────────────────────
CREATE POLICY "Anyone can view bosses"
    ON public.bosses FOR SELECT USING (true);

-- ── BOSS ENCOUNTERS ─────────────────────────────────────────
CREATE POLICY "Party members can view encounters"
    ON public.boss_encounters FOR SELECT USING (
        party_id IN (SELECT party_id FROM public.profiles WHERE id = auth.uid())
    );

CREATE POLICY "Party members can update encounters"
    ON public.boss_encounters FOR UPDATE USING (
        party_id IN (SELECT party_id FROM public.profiles WHERE id = auth.uid())
    );

CREATE POLICY "Party leader can create encounters"
    ON public.boss_encounters FOR INSERT WITH CHECK (
        party_id IN (SELECT id FROM public.parties WHERE leader_id = auth.uid())
    );

-- ── TASKS ───────────────────────────────────────────────────
CREATE POLICY "Users can view own tasks"
    ON public.tasks FOR SELECT USING (user_id = auth.uid());

CREATE POLICY "Users can insert own tasks"
    ON public.tasks FOR INSERT WITH CHECK (user_id = auth.uid());

CREATE POLICY "Users can update own tasks"
    ON public.tasks FOR UPDATE USING (user_id = auth.uid());

CREATE POLICY "Users can delete own tasks"
    ON public.tasks FOR DELETE USING (user_id = auth.uid());

-- ── FOCUS SESSIONS ──────────────────────────────────────────
CREATE POLICY "Users can view own sessions"
    ON public.focus_sessions FOR SELECT USING (user_id = auth.uid());

CREATE POLICY "Users can insert own sessions"
    ON public.focus_sessions FOR INSERT WITH CHECK (user_id = auth.uid());

CREATE POLICY "Users can update own sessions"
    ON public.focus_sessions FOR UPDATE USING (user_id = auth.uid());

-- ── COMBAT LOGS ─────────────────────────────────────────────
CREATE POLICY "Party members can view combat logs"
    ON public.combat_logs FOR SELECT USING (
        party_id IN (SELECT party_id FROM public.profiles WHERE id = auth.uid())
    );

CREATE POLICY "Party members can insert combat logs"
    ON public.combat_logs FOR INSERT WITH CHECK (
        party_id IN (SELECT party_id FROM public.profiles WHERE id = auth.uid())
    );

-- ── SHOP ITEMS (read-only for all authenticated users) ──────
CREATE POLICY "Authenticated users can view shop items"
    ON public.shop_items FOR SELECT USING (auth.role() = 'authenticated');

-- ── INVENTORY ───────────────────────────────────────────────
CREATE POLICY "Users can view own inventory"
    ON public.inventory FOR SELECT USING (user_id = auth.uid());

CREATE POLICY "Users can insert own inventory"
    ON public.inventory FOR INSERT WITH CHECK (user_id = auth.uid());

CREATE POLICY "Users can update own inventory"
    ON public.inventory FOR UPDATE USING (user_id = auth.uid());

-- ============================================================
-- 11. HELPER FUNCTIONS
-- ============================================================

-- Auto-create profile on sign-up
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO public.profiles (id, username)
    VALUES (NEW.id, COALESCE(NEW.raw_user_meta_data->>'username', 'Adventurer_' || substr(NEW.id::text, 1, 6)));
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();

-- Calculate damage based on difficulty and player stats
CREATE OR REPLACE FUNCTION public.calculate_damage(
    p_difficulty difficulty,
    p_attack INT,
    p_class TEXT,
    p_is_focus BOOLEAN DEFAULT FALSE
) RETURNS INT AS $$
DECLARE
    base_dmg INT;
    multiplier FLOAT := 1.0;
BEGIN
    -- Base damage by difficulty
    base_dmg := CASE p_difficulty
        WHEN 'easy'  THEN 10
        WHEN 'medium' THEN 20
        WHEN 'hard'  THEN 35
        WHEN 'exam'  THEN 50
    END;

    -- Add player attack bonus
    base_dmg := base_dmg + (p_attack / 2);

    -- Class bonuses
    IF p_class = 'Warrior' AND p_difficulty IN ('hard', 'exam') THEN
        multiplier := 1.5;  -- Warrior bonus on hard/exam
    ELSIF p_class = 'Mage' AND p_is_focus THEN
        multiplier := 1.3;  -- Mage bonus on focus sessions
    ELSIF p_class = 'Rogue' THEN
        multiplier := 1.1;  -- Rogue slight bonus always
    END IF;

    -- Focus sessions get critical strike bonus
    IF p_is_focus THEN
        multiplier := multiplier * 1.25;
    END IF;

    RETURN FLOOR(base_dmg * multiplier);
END;
$$ LANGUAGE plpgsql IMMUTABLE;

-- ============================================================
-- 12. SEED DATA – Bosses (Bestiary)
-- ============================================================
INSERT INTO public.bosses (name, description, image_url, base_hp, attack, tier, xp_reward, gold_reward) VALUES
('The Procrastination Hydra', 'A many-headed beast that grows stronger every hour you delay. Each head whispers "just five more minutes..."', '/images/bosses/hydra.png', 800, 12, 'Normal', 150, 80),
('Midterm Lich', 'An undead sorcerer fueled by forgotten formulas and unread chapters. Its phylactery is hidden inside a blue book.', '/images/bosses/lich.png', 1200, 18, 'Elite', 300, 150),
('Syntax Error Golem', 'A hulking construct of tangled code and missing semicolons. It crashes through your defenses with segfaults.', '/images/bosses/golem.png', 1000, 15, 'Normal', 200, 100),
('Deadline Dragon', 'A colossal wyrm whose fiery breath incinerates schedules. It lands on the night before submission day.', '/images/bosses/dragon.png', 1500, 22, 'Legendary', 500, 250),
('All-Nighter Specter', 'A ghostly presence that drains your energy and blurs your vision. Fueled by caffeine and desperation.', '/images/bosses/specter.png', 600, 10, 'Normal', 100, 60),
('Thesis Titan', 'An ancient colossus built from citation stacks and literature reviews. Its weak point is the methodology section.', '/images/bosses/titan.png', 2000, 25, 'Legendary', 800, 400);

-- ============================================================
-- 13. SEED DATA – Shop Items
-- ============================================================
INSERT INTO public.shop_items (name, description, category, gold_cost, stat_bonus, required_level, is_consumable) VALUES
('Wooden Sword', 'A humble training blade. Every hero starts somewhere.', 'weapon', 0, '{"attack": 2}', 1, FALSE),
('Iron Broadsword', 'Reliable steel for reliable students.', 'weapon', 100, '{"attack": 5}', 3, FALSE),
('Scholar''s Staff', 'Channels arcane focus energy into raw power.', 'weapon', 150, '{"attack": 4, "max_mp": 10}', 3, FALSE),
('Leather Cap', 'Basic head protection against falling GPAs.', 'helmet', 50, '{"max_hp": 5}', 1, FALSE),
('Mage''s Hood', 'A hood that enhances concentration.', 'helmet', 120, '{"max_mp": 15}', 5, FALSE),
('Chainmail Vest', 'Sturdy armor for the academic battlefield.', 'armor', 200, '{"max_hp": 20}', 5, FALSE),
('HP Potion (Small)', 'Restores 15 HP. Tastes like energy drinks.', 'potion', 30, '{"hp_restore": 15}', 1, TRUE),
('HP Potion (Large)', 'Restores 40 HP. Contains double espresso.', 'potion', 80, '{"hp_restore": 40}', 1, TRUE),
('XP Scroll', 'Grants 50 bonus XP. Smells like old textbooks.', 'potion', 100, '{"xp_bonus": 50}', 1, TRUE),
('Streak Freeze Shield', 'Protects your daily streak for one day. Perfect for exam week.', 'shield', 150, '{"streak_protection": 1}', 1, TRUE),
('Golden Quill (Cosmetic)', 'A shimmering quill that marks you as a dedicated scholar.', 'cosmetic', 300, '{}', 10, FALSE),
('Phoenix Feather Cape', 'Auto-revive once when you hit 0 HP. Consumed on use.', 'shield', 500, '{"auto_revive": 1}', 8, TRUE);

-- ============================================================
-- 14. REALTIME – Enable realtime for combat & encounters
-- ============================================================
ALTER PUBLICATION supabase_realtime ADD TABLE public.combat_logs;
ALTER PUBLICATION supabase_realtime ADD TABLE public.boss_encounters;
ALTER PUBLICATION supabase_realtime ADD TABLE public.profiles;
