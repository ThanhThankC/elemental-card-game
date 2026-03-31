/// <summary>
/// Spell effect interface defining the structure for all spell effects in the game.
/// </summary>
public interface ISpellEffect
{
    /// <summary>
    /// Determines if the spell card should be sent to the graveyard before executing the effect.
    /// This is important for effects that might depend on the card being in the graveyard (e.g., "Send this card to the graveyard, then draw 2 cards").
    /// </summary>
    bool SendToGraveyardFirst { get; }

    bool NeedsTarget { get; }

    bool CanActivate(SpellContext context);

    void Execute(SpellContext context);
}
