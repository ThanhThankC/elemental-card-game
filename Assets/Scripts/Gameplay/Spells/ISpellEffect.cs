/// <summary>
/// 
/// </summary>
public interface ISpellEffect
{
    SpellEffectID EffectID { get; }

    bool SendToGraveyardFirst { get; }

    bool NeedsTarget { get; }

    bool CanActivate(SpellContext context);

    void Execute(SpellContext context);
}
