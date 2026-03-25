using System;
using UnityEngine;

public class FieldSlot
{
    [SerializeField] private Transform slotTransform;
    [SerializeField] private int slotIndex;

    private Card currentCard = null;
    private bool isFaceDown = false;

    public Transform Transform => slotTransform;
    public int Index => slotIndex;
    public Card Card => currentCard;
    public bool IsEmpty => currentCard == null;
    public bool ISFaceDown => isFaceDown; 

    public FieldSlot(Transform transform, int index)
    {
        slotTransform = transform;
        slotIndex = index;
    }

    /// <summary>
    /// [TKC] Place card in this slot.
    /// </summary>
    public void PlaceCard(Card card, bool faceDown = false)
    {
        currentCard = card;
        isFaceDown = faceDown;

        if (card != null)
        {
            card.transform.SetParent(slotTransform);
            card.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// [TKC] Return card to move.
    /// </summary>
    public void RemoveCard()
    {
        //Card removedCard = currentCard;
        currentCard = null;
        isFaceDown = false;
        //return removedCard;
    }

    /// <summary>
    /// [TKC] Flip card.
    /// </summary>
    public void Flip()
    {
        isFaceDown = !isFaceDown;
    }

    /// <summary>
    /// [TKC] Set face state.
    /// </summary>
    public void SetFaceDown(bool faceDown)
    {
        isFaceDown = faceDown;
    }
}