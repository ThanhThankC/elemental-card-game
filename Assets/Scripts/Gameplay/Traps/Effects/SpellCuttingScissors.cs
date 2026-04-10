using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCuttingScissors : ITrapEffect
{
    public bool SendToGraveyardFirst => false;

    public TargetType TargetType => TargetType.CardInHand;

    public bool CanActivate(CardEffectContext context)
    {
        return context.HandLayout.GetCardCount() > 0;
    }

    public void Execute(CardEffectContext context)
    {
        Card cardInHand = context.TargetCard;
        if (cardInHand == null) return;

        context.HandLayout.RemoveCard(cardInHand.transform);

        cardInHand.SetState(CardState.Discarding);
        CardSelectionManager.Instance?.NotifyCardSentToGraveyard(cardInHand);
        CardAnimator.AnimateToGraveyard(
            cardInHand.transform,
            context.GraveyardZone,
            onComplete: () => cardInHand.SetState(CardState.InGraveyard)
        );
    }
}
