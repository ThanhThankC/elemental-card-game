using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuaHoMenhEffect : IContinuousSpellEffect
{
    private int buffAmount;

    public SpellEffectID EffectID => SpellEffectID.BuaHoMenh;

    public BuaHoMenhEffect(int buffAmount)
    {
        this.buffAmount = buffAmount;
    }

    public bool CanActivate(SpellContext context)
    {
        return context.SpellZone.HasEmptySlot();
    }

    public void OnActivate(SpellContext context)
    {
        BuffATKForMonsterOnField(context.PlayerMonsterZone.GetAllCards());

        SummonController.Instance.OnMonsterSummoned += BuffATKForSummonMonster;
    }

    public void OnDeactivate(SpellContext context)
    {
        if (SummonController.Instance != null)
        {
            SummonController.Instance.OnMonsterSummoned -= BuffATKForSummonMonster;
        }
    }

    private void BuffATKForMonsterOnField(List<Card> cards)
    {
        if (cards == null || cards.Count == 0) return;

        foreach (Card card in cards)
        {
            card?.ModifyATK(buffAmount);
        }
        Debug.Log($"[BuaHoMenh] {cards.Count} cards (+ {buffAmount} ATK)!");
    }

    private void BuffATKForSummonMonster(Card monster)
    {
        if (monster == null) return;
        monster.ModifyATK(buffAmount);
        Debug.Log($"[BuffATKDEF] {monster.GetCardData().GetCardName()} + {buffAmount} ATK/DEF");
    }
}
