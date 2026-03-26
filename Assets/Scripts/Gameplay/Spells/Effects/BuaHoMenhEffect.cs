using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuaHoMenhEffect : IContinuousSpellEffect
{
    private readonly int buffAmount;

    public SpellEffectID EffectID => SpellEffectID.BuaHoMenh;

    public bool SendToGraveyardFirst => false;

    public bool NeedsTarget => false;

    public BuaHoMenhEffect(int buffAmount)
    {
        this.buffAmount = buffAmount;
    }

    public bool CanActivate(SpellContext context)
    {
        return true;
    }

    public void Execute(SpellContext context)
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
