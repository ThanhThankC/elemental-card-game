using UnityEngine;

/// <summary>
/// Validates selection rules without executing any actions.
/// </summary>
public class SelectionValidator
{
    public bool ShouldClearSacrifice(Card newHandCard, Card previousHandCard)
    {
        if (newHandCard == null) return false;
        if (previousHandCard == null) return true;

        MonsterTier newRequiredTier = GameRules.GetRequiredSacrificeTier(newHandCard);
        MonsterTier previousTier = GameRules.GetRequiredSacrificeTier(previousHandCard);

        return newRequiredTier != previousTier;
    }

    /// <summary>
    /// Does the field card match the required sacrifice tier?
    /// </summary>
    public bool MatchesSacrificeTier(Card fieldCard, MonsterTier requiredTier)
    {
        if (fieldCard == null) return false;

        MonsterTier fieldCardTier = GameRules.GetCardTier(fieldCard);
        return fieldCardTier == requiredTier;
    }

    /// <summary>
    /// 
    /// </summary>
    public (MonsterTier tier, int count) GetSacrificeRequirement(Card card)
    {
        if (card == null)
        {
            return (MonsterTier.None, 0);
        }

        int stars = card.GetCardData().Stars;

        if (stars <= GameRules.LOW_TIER_MAX_STARS)
        {
            return (MonsterTier.None, 0);
        }
        else if (stars <= GameRules.MID_TIER_MAX_STARS)
        {
            return (MonsterTier.Low, GameRules.MID_TIER_SACRIFICE_COUNT);
        }
        else
        {
            return (MonsterTier.Mid, GameRules.HIGH_TIER_SACRIFICE_COUNT);
        }
    }
}