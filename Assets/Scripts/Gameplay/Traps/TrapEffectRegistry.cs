using UnityEngine;

public static class TrapEffectRegistry
{
    public static ITrapEffect Build(CardData cardData)
    {
        switch (cardData.TrapEffectID)
        {
            case TrapEffectID.MadCyclone:
                return new MadCyclone();
            case TrapEffectID.TNTBomb:
                return new TNTBomb();
            default:
                Debug.LogWarning($"[TrapEffectRegistry] No effect ID: {cardData.TrapEffectID}");
                return null;
        }
    }
}