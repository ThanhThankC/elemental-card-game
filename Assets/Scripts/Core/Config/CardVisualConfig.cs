using UnityEngine;
/// <summary>
/// Central configuration for all card visual properties.
/// Modify GlobalScaleUnit to zoom all cards simultaneously.
/// </summary>
public static class CardVisualConfig
{
    /// <summary>
    /// The base unit size for a card. 
    /// Modify this to zoom in/out the entire card system globally.
    /// </summary>
    public static float GlobalScaleUnit = 1.0f;

    /// <summary>
    /// Global multiplier applied when any card is selected/hovered.
    /// </summary>
    public static float GlobalSelectedMultiplier = 1f;

    public static float DeckScale => 0.5f * GlobalScaleUnit;
    public static float HandScale => 1.0f * GlobalScaleUnit;
    public static float FieldScale => 0.8f * GlobalScaleUnit;
    public static float GraveyardScale => 0.6f * GlobalScaleUnit;

    public static float HandSelectedMultiplier => 1.2f * GlobalSelectedMultiplier;
    public static float FieldSelectedMultiplier => 1.3f * GlobalSelectedMultiplier;

    /// <summary>
    /// Get resting scale for a card state.
    /// </summary>
    public static Vector3 GetRestingScale(CardState state)
    {
        switch (state)
        {
            case CardState.InDeck: return Vector3.one * DeckScale;
            case CardState.InHand: return Vector3.one * HandScale;
            case CardState.OnField: return Vector3.one * FieldScale;
            case CardState.InGraveyard: return Vector3.one * GraveyardScale;
            default: return Vector3.one * HandScale;
        }
    }

    /// <summary>
    /// Get selected scale for a card state.
    /// </summary>
    public static Vector3 GetSelectedScale(CardState state)
    {
        return GetRestingScale(state) * GetSelectedMultiplier(state);
    }

    private static float GetSelectedMultiplier(CardState state)
    {
        switch (state)
        {
            case CardState.InHand: return HandSelectedMultiplier;
            case CardState.OnField: return FieldSelectedMultiplier;
            default: return HandSelectedMultiplier;
        }
    }

}
