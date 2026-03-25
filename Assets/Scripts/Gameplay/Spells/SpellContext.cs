using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SpellContext
{
    public Card SpellCard { get; set; }
    public Card TargetMonster { get; set; }
    public MonsterZone PlayerMonsterZone { get; set; }
    public SpellZone SpellZone { get; set; }
    public DeckManager DeckManager { get; set; }
    public Transform GraveyardZone { get; set; }

    public bool HasTarget() => TargetMonster != null;
}
