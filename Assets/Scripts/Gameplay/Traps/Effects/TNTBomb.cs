using UnityEngine;

public class TNTBomb : ITrapEffect
{
    public bool SendToGraveyardFirst => false;

    public TargetType TargetType => TargetType.MonsterOnField;

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

        Card monster = context.TargetMonster;
        if (monster == null) return;

        bool removed = context.PlayerMonsterZone.RemoveCard(monster);
        if (!removed)
        {
            Debug.LogWarning($"[MadCyclone] Failed to remove: {monster.GetCardData().name}");
            return;
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
