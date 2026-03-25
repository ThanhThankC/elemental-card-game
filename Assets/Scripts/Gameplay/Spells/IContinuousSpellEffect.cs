using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContinuousSpellEffect
{
    SpellEffectID EffectID { get; }

    bool CanActivate(SpellContext context);

    void OnActivate(SpellContext context);

    void OnDeactivate(SpellContext context);
}
