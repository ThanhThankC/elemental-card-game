using UnityEngine;

public static class TrapEffectRegistry
{
    public static ITrapEffect Build(CardData cardData)
    {
        switch (cardData.TrapEffectID)
        {
            case TrapEffectID.MadCyclone:
                return new MadCyclone();
            default:
                Debug.LogWarning($"[TrapEffectRegistry] No effect ID: {cardData.TrapEffectID}");
                return null;
        }
    }
}