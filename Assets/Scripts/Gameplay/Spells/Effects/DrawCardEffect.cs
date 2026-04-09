public class DrawCardEffect : ISpellEffect
{
    public bool SendToGraveyardFirst => true;

    public TargetType TargetType => TargetType.None;

    public bool CanActivate(SpellContext context)
    {
        return context.DeckManager.CanDrawCard();
    }

    public void Execute(SpellContext context)
    {
        context.DeckManager.OnDrawCard();
    }
}
