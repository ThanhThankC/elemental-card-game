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
        context.PlayerMonsterZone.RemoveCard(context.TargetCard);

        CardSelectionManager.Instance?.NotifyCardSentToGraveyard(target);

        CardAnimator.AnimateToGraveyard(
            target.transform,
            context.GraveyardZone,
            onComplete: () => target.SetState(CardState.InGraveyard)
        );
    }
}
