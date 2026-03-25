using UnityEngine;

public static class SpellEffectRegistry
{
    public static ISpellEffect BuildNormal(CardData cardData)
    {
        if (cardData == null) return null;

        switch (cardData.SpellEffectID)
        {
            case SpellEffectID.BuffATKDEF:
                return new BuffATKDEFEffect(cardData.SpellBuffAmount);
            case SpellEffectID.DrawCard:
                return new DrawCardEffect();
            case SpellEffectID.DestroyMonster:
                return new DestroyMonsterEffect();
            default:
                Debug.LogWarning($"[SpellEffectRegistry] No effect ID: {cardData.SpellEffectID}");
                return null;
        }
    }

    public static IContinuousSpellEffect BuildContinous(CardData cardData)
    {
        if (cardData == null) return null;

        switch (cardData.SpellEffectID)
        {
            case SpellEffectID.BuaHoMenh:
                return new BuaHoMenhEffect(cardData.SpellBuffAmount);
            default:
                Debug.LogWarning($"[SpellEffectRegistry] No effect ID: {cardData.SpellEffectID}");
                return null;
        }
    }
}
