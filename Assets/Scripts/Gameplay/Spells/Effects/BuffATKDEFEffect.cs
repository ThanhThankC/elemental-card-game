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

    public bool CanActivate(SpellContext context)
    {
        return context.PlayerMonsterZone.GetAllCards().Count > 0;
    }

    public void Execute(SpellContext context)
    {
        Card target = context.TargetMonster;
        target.ModifyATK(buffAmount);
        target.ModifyDEF(buffAmount);

        Debug.Log($"[BuffATKDEF] {target.GetCardData().GetCardName()} + {buffAmount} ATK/DEF");
    }
}
