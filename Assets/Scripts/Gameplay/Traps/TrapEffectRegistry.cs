using UnityEngine;

public static class TrapEffectRegistry
{
    public static ITrapEffect Build(CardData cardData)
    {
        switch (cardData.TrapEffectID)
        {
            default:
                Debug.LogWarning($"[TrapEffectRegistry] No effect ID: {cardData.TrapEffectID}");
                return null;
        }
    }
}