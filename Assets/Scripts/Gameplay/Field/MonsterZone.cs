using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterZone : FieldZone
{
    protected override int maxSlots() => GameRules.MAX_MONSTER_ON_FIELD;

    public override bool CanPlaceCard(Card card)
    {
        if (card == null) return false;

        if (card.GetCardData().Type != CardType.Monster)
        {
            Debug.LogWarning($"[MonsterZone] Only monsters allowed!");
            return false;
        }

        if (!HasEmptySlot())
        {
            Debug.LogWarning($"[MonsterZone] Not space left!");
            return false;
        }

        return true;
    }

    public override CardType GetAllowedCardType()
    {
        return CardType.Monster;
    }

    public bool SummonMonster(Card card, int slotIndex = -1)
    {
        if (!CanPlaceCard(card)) return false;

        if (slotIndex == -1)
        {
            slotIndex = FindEmptySlot();
        }

        if (slotIndex == -1)
        {
            Debug.LogWarning($"[MonsterZone] Not space left!");
            return false;
        }

        bool success = PlaceCard(card, slotIndex);

        if (success)
        {
            Debug.Log($"[MonsterZone] Summoned {card.GetCardData().GetCardName()} to slot {slotIndex}");
        }

        return success;
    }
}
