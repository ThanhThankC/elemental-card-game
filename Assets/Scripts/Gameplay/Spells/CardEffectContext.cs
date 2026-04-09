using UnityEngine;

/// <summary>
/// Context object to pass necessary information for card effect execution.
/// </summary>
public class CardEffectContext
{
    public Card SourceCard { get; set; }
    public Card TargetCard { get; set; }
    public MonsterZone PlayerMonsterZone { get; set; }
    public DeckManager DeckManager { get; set; }
    public Transform GraveyardZone { get; set; }
}
