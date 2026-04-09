public class DestroyMonsterEffect : ISpellEffect
{
    public bool SendToGraveyardFirst => false;

    public TargetType TargetType => TargetType.MonsterOnField;

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
