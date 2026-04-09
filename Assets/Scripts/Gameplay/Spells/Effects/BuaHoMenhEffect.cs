using UnityEngine;

public class BuaHoMenhEffect : IContinuousSpellEffect
{
    private readonly int buffAmount;

    public bool SendToGraveyardFirst => false;

    public TargetType TargetType => TargetType.None;

    public BuaHoMenhEffect(int buffAmount)
    {
        this.buffAmount = buffAmount;
    }

    public bool CanActivate(CardEffectContext context)
    {
        return true;
    }

    public void Execute(CardEffectContext context)
    {
        foreach (Card card in context.PlayerMonsterZone.GetAllCards())
        {
            card.ModifyATK(buffAmount);
        }
    }

    public void OnMonsterSummoned(Card monster)
    {
        if (monster == null) return;
        monster.ModifyATK(buffAmount);
        Debug.Log($"[BuffATKDEF] {monster.GetCardData().GetCardName()} + {buffAmount} ATK/DEF");
    }

    public void OnDeactivate() { }

}
