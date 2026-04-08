using System.Collections.Generic;
using UnityEngine;

public class MadCyclone : ITrapEffect
{
    public bool SendToGraveyardFirst => false;

    public bool NeedsTarget => false;

    public bool CanActivate(SpellContext context)
    {
        TrapContext ctx = context as TrapContext;
        if (ctx == null) return false;

        return ctx.PlayerMonsterZone.GetAllCards().Count > 0;
    }

    public void Execute(SpellContext context)
    {
        TrapContext ctx = context as TrapContext;
        if (ctx == null) return;

        List<Card> monsters = new List<Card>(ctx.PlayerMonsterZone.GetAllCards());

        foreach (Card monster in monsters)
        {
            bool removed = ctx.PlayerMonsterZone.RemoveCard(monster);
            if (!removed)
            {
                Debug.LogWarning($"[MadCyclone] Failed to remove: {monster.GetCardData().name}");
                continue;
            }

            monster.SetState(CardState.Discarding);
            CardSelectionManager.Instance?.NotifyCardSentToGraveyard(monster);
            CardAnimator.AnimateToGraveyard(
                monster.transform,
                ctx.GraveyardZone,
                onComplete: () => monster.SetState(CardState.InGraveyard)
            );
        }
    }
}
