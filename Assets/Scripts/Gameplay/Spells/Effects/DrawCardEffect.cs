public class DrawCardEffect : ISpellEffect
{
    public bool SendToGraveyardFirst => true;

    public TargetType TargetType => TargetType.None;

    public bool CanActivate(CardEffectContext context)
    {
        return context.DeckManager.CanDrawCard();
    }

    public void Execute(CardEffectContext context)
    {
        context.DeckManager.OnDrawCard();
    }
}
