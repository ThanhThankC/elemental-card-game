/// <summary>
/// Central game rules cofiguration.
/// </summary>
public static class GameRules
{
    public const int LOW_TIER_MAX_STARS = 4;
    public const int MID_TIER_MAX_STARS = 7;

    public const int MID_TIER_SACRIFICE_COUNT = 2;
    public const int HIGH_TIER_SACRIFICE_COUNT = 2;

    public const int MAX_HAND_SIZE = 7;
    public const int MAX_DRAW_PER_TURN = 2;

    public const int MAX_MONSTER_ON_FIELD = 7;
    public const int MAX_SPELL_TRAP_ON_FIELD = 5;

    public const int MAXSUMMON_PER_TURN = 2;

    /// <summary>
    /// Get tier of card (Low/Mid/High).
    /// </summary>
    public static MonsterTier GetCardTier(Card card)
    {
        int stars = card.GetCardData().Stars; 

        if (stars <= LOW_TIER_MAX_STARS)
            return MonsterTier.Low;
        else if (stars <= MID_TIER_MAX_STARS)
            return MonsterTier.Mid;
        else
            return MonsterTier.High;
    }
    
    /// <summary>
    /// Gets required sacrifice tier for a monster.
    /// </summary>
    public static MonsterTier GetRequiredSacrificeTier(Card card)
    {
        int stars = card.GetCardData().Stars;

        if (stars <= LOW_TIER_MAX_STARS)
            return MonsterTier.None;
        else if (stars <= MID_TIER_MAX_STARS)
            return MonsterTier.Low;
        else
            return MonsterTier.Mid;
    }

    /// <summary>
    /// Get required sacrifice count.
    /// </summary>
    public static int GetRequiredSacrificeCount(int stars)
    {
        if (stars <= LOW_TIER_MAX_STARS)
            return 0;
        else if (stars <= MID_TIER_MAX_STARS)
            return MID_TIER_SACRIFICE_COUNT;
        else
            return HIGH_TIER_SACRIFICE_COUNT;
    }


}
