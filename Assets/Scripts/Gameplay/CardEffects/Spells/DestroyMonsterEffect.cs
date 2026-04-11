using UnityEngine;

public class DestroyMonsterEffect : ISpellEffect
{
    public bool SendToGraveyardFirst => false;

    public TargetType TargetType => TargetType.MonsterOnField;

    public bool CanActivate(CardEffectContext context)
    {
        return context.PlayerMonsterZone.GetAllCards().Count > 0;
    }

    public void Execute(CardEffectContext context)
    {
        Card target = context.TargetCard;
        bool removed = context.PlayerMonsterZone.RemoveCard(target);
        if (!removed)
        {
            Debug.LogWarning($"[DestroyMonsterEffect] Failed to remove: {target.GetCardData().name}");
            return;
        }

        CardSelectionManager.Instance?.NotifyCardSentToGraveyard(target);

        CardAnimator.AnimateToGraveyard(
            target.transform,
            context.GraveyardZone,
            onComplete: () => target.SetState(CardState.InGraveyard)
        );
    }
}
