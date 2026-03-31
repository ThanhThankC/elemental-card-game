using DG.Tweening;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DestroyMonsterEffect : ISpellEffect
{
    public bool SendToGraveyardFirst => false;

    public bool NeedsTarget => true;

    public bool CanActivate(SpellContext context)
    {
        return context.PlayerMonsterZone.GetAllCards().Count > 0;
    }

    public void Execute(SpellContext context)
    {
        Card target = context.TargetMonster;
        context.PlayerMonsterZone.RemoveCard(context.TargetMonster);

        CardSelectionManager.Instance?.NotifyCardSentToGraveyard(target);

        CardAnimator.AnimateToGraveyard(
            target.transform,
            context.GraveyardZone,
            onComplete: () => target.SetState(CardState.InGraveyard)
        );
    }
}
