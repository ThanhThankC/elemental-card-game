using UnityEngine;

public class BuffATKDEFEffect : ISpellEffect
{
    private readonly int buffAmount;

    public bool SendToGraveyardFirst => false;

    public TargetType TargetType => TargetType.MonsterOnField;

    public BuffATKDEFEffect(int buffAmount)
    {
        this.buffAmount = buffAmount;
    }

    public bool CanActivate(CardEffectContext context)
    {
        return context.PlayerMonsterZone.GetAllCards().Count > 0;
    }

    public void Execute(CardEffectContext context)
    {
        Card target = context.TargetCard;
        target.ModifyATK(buffAmount);
        target.ModifyDEF(buffAmount);

        Debug.Log($"[BuffATKDEF] {target.GetCardData().GetCardName()} + {buffAmount} ATK/DEF");
    }
}
