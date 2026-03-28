using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapZone : FieldZone
{
    protected override int maxSlots() => GameRules.MAX_TRAP_ON_FIELD;

    public override bool CanPlaceCard(Card card)
    {
        if (card == null) return false;

        if (card.GetCardData().Type != CardType.Trap)
        {
            Debug.LogWarning($"[trapZone] Only trap cards allowed!");
            return false;
        }

        if (!HasEmptySlot())
        {
            Debug.LogWarning($"[trapZone] Not space left!");
            return false;
        }

        return true;
    }

    public override CardType GetAllowedCardType()
    {
        return CardType.Trap;
    }

    public bool PlaceTrap(Card card, int slotIndex = -1)
    {
        if (!CanPlaceCard(card)) return false;

        if (slotIndex == -1)
        {
            slotIndex = FindEmptySlot();
        }

        if (slotIndex == -1)
        {
            Debug.LogWarning($"[trapZone] Not space left!");
            return false;
        }

        bool success = PlaceCard(card, slotIndex);

        if (success)
        {
            Debug.Log($"[trapZone] Placed {card.GetCardData().GetCardName()} face-down to slot {slotIndex}");
        }

        return success;
    }
}
