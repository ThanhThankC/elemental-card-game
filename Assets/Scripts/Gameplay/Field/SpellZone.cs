using UnityEngine;

public class SpellZone : FieldZone
{
    public override bool CanPlaceCard(Card card)
    {
        if (card == null) return false;

        if (card.GetCardData().Type != CardType.Spell || card.GetCardData().SpellType != SpellType.Continuous)
        {
            Debug.LogWarning($"[SpellZone] Only continuous spell card allowed!");
            return false;
        }

        if (!HasEmptySlot())
        {
            Debug.LogWarning($"[SpellZone] Not space left!");
            return false;
        }

        return true;
    }

    public override CardType GetAllowedCardType()
    {
        return CardType.Spell;
    }

    public override bool RemoveCard(Card card)
    {
        //TODO 
        return base.RemoveCard(card);
    }

    public bool SetSpellCard(Card card, int slotIndex = -1)
    {
        if (!CanPlaceCard(card)) return false;

        if (slotIndex == -1)
        {
            slotIndex = FindEmptySlot();
        }

        if (slotIndex == -1)
        {
            Debug.LogWarning($"[SpellZone] Not space left!");
            return false;
        }

        bool success = PlaceCard(card, slotIndex);

        if (success)
        {
            Debug.Log($"[SpellZone] Setted {card.GetCardData().GetCardName()} to slot {slotIndex}");
        }

        return success;
    }
}
