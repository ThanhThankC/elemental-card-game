using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class FieldZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    [SerializeField] protected Transform[] slotTransforms;

    protected FieldSlot[] slots;

    protected virtual void Awake()
    {
        InitializeSlots();
    }

    protected abstract int maxSlots();

    public abstract bool CanPlaceCard(Card card);

    public abstract CardType GetAllowedCardType();

    protected virtual void InitializeSlots()
    {
        if (slotTransforms == null || slotTransforms.Length == 0)
            { Debug.LogError($"[{GetType().Name}] Slot transform not assigned!"); return; }

        int actualCount = Mathf.Min(maxSlots(), slotTransforms.Length);

        slots = new FieldSlot[actualCount];

        for (int i = 0; i < actualCount; i++)
        {
            slots[i] = new FieldSlot(slotTransforms[i], i);
        }

        Debug.Log($"[{GetType().Name}] Initialized {slots.Length} slots");
    }

    public virtual bool PlaceCard(Card card, int slotIndex, bool faceDown = false)
    {
        if (!IsValidSlotIndex(slotIndex))
            { Debug.LogWarning($"[{GetType().Name}] Invalid slot index: {slotIndex}"); return false; };

        if (!slots[slotIndex].IsEmpty)
            { Debug.LogWarning($"[{GetType().Name}] Slot {slotIndex} is occupied"); return false; }

        slots[slotIndex].PlaceCard(card, faceDown);

        return true;
    }

    public virtual bool RemoveCard(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex))
            { Debug.LogWarning($"[{GetType().Name}] Invalid slot index: {slotIndex}"); return false; }

        if (slots[slotIndex].IsEmpty)
            { Debug.LogWarning($"[{GetType().Name}] Slot {slotIndex} is already empty"); return false; }

        return slots[slotIndex].RemoveCard();
    }

    public virtual bool RemoveCard(Card card)
    {
        if (card == null) return false;

        int index = GetSlotIndex(card);
        if (index == -1)
        {
            Debug.LogWarning($"[{GetType().Name}] Card not found in any slot");
            return false;
        }

        return RemoveCard(index);
    }

    public int GetSlotIndex(Card card)
    {
        if (card == null) return -1;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Card == card)
            {
                return i;
            }
        }

        return -1;
    }
    public virtual int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty) return i;
        }
        return -1;
    }

    public virtual bool HasEmptySlot()
    {
        return FindEmptySlot() != -1;
    }

    public virtual Card GetCardAt(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex)) return null;
        return slots[slotIndex].Card;
    }

    public virtual List<Card> GetAllCards()
    {
        return slots.Where(slot => !slot.IsEmpty).Select(slot => slot.Card).ToList();
    }

    protected bool IsValidSlotIndex(int slotIndex)
    {
        return (slotIndex >= 0 && slotIndex < slots.Length);
    }

    public Transform GetSlotTransform(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex)) return null;
        return slots[slotIndex].Transform;
    }
}
