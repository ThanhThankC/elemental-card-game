using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadCyclone : ITrapEffect
{
    public bool SendToGraveyardFirst => false;

    public bool NeedsTarget => false;

    public bool CanActivate(SpellContext context)
    {
        return context.PlayerMonsterZone.GetAllCards().Count > 0;
    }

    public void Execute(SpellContext context)
    {
        foreach (Card card in context.PlayerMonsterZone.GetAllCards())
        {
            card.ModifyATK(-500);
        }
    }
}
