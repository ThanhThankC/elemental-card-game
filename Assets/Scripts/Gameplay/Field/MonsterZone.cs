using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterZone : FieldZone
{
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

    //public bool CanSummonDirect(Card card)

    //public (int count, MonsterTier tier) GetRequiredSacrifices(Card card)
    //{
    //    int stars = card.GetCardData().Stars;

    //    if (stars <= 4)
    //        return (0, MonsterTier.None);
    //    else if (stars <= 7)
    //        return (2, MonsterTier.Low);
    //    else
    //        return (2, MonsterTier.Mid);
    //}

    //public bool HasEnoughSacrifices(int count, MonsterTier tier)
    //{
    //    var availableMonsters = GetMonstersForSacrifice(tier);
    //    return availableMonsters.Count >= count;
    //}

    //public List<Card> GetMonstersForSacrifice(MonsterTier tier)
    //{
    //    return GetAllCards().Where(card =>
    //    {
    //        int stars = card.GetCardData().Stars;

    //        switch (tier)
    //        {
    //            case MonsterTier.Low:
    //                return stars <= 4;
    //            case MonsterTier.Mid:
    //                return stars >= 5 && stars <= 7;
    //            case MonsterTier.High:
    //                return stars >= 8;
    //            default:
    //                return true;
    //        }
    //    }).ToList();
    //}

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
