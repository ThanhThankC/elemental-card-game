using UnityEngine;

public class DrawCardEffect : ISpellEffect
{
    public SpellEffectID EffectID => SpellEffectID.DrawCard;

    public bool SendToGraveyardFirst => true;

    public bool NeedsTarget => false;

    public bool CanActivate(SpellContext context)
    {
        return context.DeckManager.CanDrawCard();
    }

    public void Execute(SpellContext context)
    {
        context.DeckManager.OnDrawCard();
    }
}
