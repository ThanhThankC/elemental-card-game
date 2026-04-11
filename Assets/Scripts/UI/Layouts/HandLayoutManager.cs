using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// [TKC] Arranges cards in hand with arc layout.
/// </summary>
public class HandLayoutManager : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] private float cardSpacing = 80f;
    [SerializeField] private float arcHeight = 50f;
    [SerializeField] private float cardAngle = 5f;

    [Header("Animation Settings")]
    [SerializeField] private float rearrangeDuration = 0.3f;
    [SerializeField] private Ease rearrangeEase = Ease.OutCubic;

    public event Action OnHandCountChanged;

    private List<Transform> cards = new List<Transform>();

    public void AddCard(Transform card)
    {
        if (card == null) return;

        if (!cards.Contains(card))
        {
            cards.Add(card);
            card.SetParent(transform);
            ArrangeCards(true);
        }

        OnHandCountChanged?.Invoke();
    }

    public bool RemoveCard(Transform card)
    {
        if (card == null) return false;

        bool removed = cards.Remove(card);
        if (removed)
        {
            ArrangeCards(true);
            OnHandCountChanged?.Invoke();
        }
        return removed;
    }

    public void SwapCards(Card a, Card b)
    {
        if (a == null || b == null) return;

        int indexA = cards.IndexOf(a.transform);
        int indexB = cards.IndexOf(b.transform);

        if (indexA == -1 || indexB == -1 || indexA == indexB) return;

        (cards[indexA], cards[indexB]) = (cards[indexB], cards[indexA]);
    }

    /// <summary>
    /// Arrange all cards in arc layout.
    /// </summary>
    /// <param name="aminate">Use DOTween animation or instant</param>
    public void ArrangeCards(bool aminate = false)
    {
        int cardCount = cards.Count;
        if (cardCount == 0) return;

        for (int i = 0; i < cardCount; i++)
        {
            if (cards[i] == null) continue;

            Vector3 targetPos = CalculatePosition(i);
            Quaternion targetRot = CalculateRotation(i);

            Canvas canvas = cards[i].GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = i;
            }

            Transform cardTransform = cards[i];
            Card cardComp = cardTransform.GetComponent<Card>();

            if (aminate)
            {
                Sequence seq = DOTween.Sequence();

                seq.Join(cardTransform.DOLocalMove(targetPos, rearrangeDuration).SetEase(rearrangeEase));
                seq.Join(cardTransform.DOLocalRotateQuaternion(targetRot, rearrangeDuration).SetEase(rearrangeEase));
                seq.OnComplete(() =>
                {
                    if (cardComp != null)
                    {
                        cardComp.SetState(CardState.InHand);
                    }
                });
            }
            else
            {
                cardTransform.localPosition = targetPos;
                cardTransform.localRotation = targetRot;

                if (cardComp != null)
                {
                    cardComp.SetState(CardState.InHand);
                }
            }

            transform.SetAsFirstSibling();
        }
    }

    /// <summary>
    /// Get target position for a specific card.
    /// </summary>
    public Vector3 GetTargetPosition(Transform card)
    {
        int index = cards.IndexOf(card);
        if (index == -1)
        {
            return card.localPosition;
        }
        return CalculatePosition(index);
    }

    /// <summary>
    /// Get target rotation for a specific card.
    /// </summary>
    public Quaternion GetTargetRotation(Transform card)
    {
        int index = cards.IndexOf(card);
        if (index == -1)
        {
            return Quaternion.identity;
        }

        return CalculateRotation(index);
    }

    /// <summary>
    /// Get target sorting order for a specifice card.
    /// </summary>
    public int GetSortingOrder(Transform card)
    {
        return cards.IndexOf(card);
    }

    private Vector3 CalculatePosition(int index)
    {
        int cardCount = cards.Count;
        float totalWidth = (cardCount - 1) * cardSpacing;
        float startX = -totalWidth / 2f;
        float xPos = startX + (index * cardSpacing);

        float normalizedX = cardCount > 1 ? (float)index / (cardCount - 1) : 0.5f;
        float centerOffset = normalizedX - 0.5f;
        float yPos = -arcHeight * 4f * centerOffset * centerOffset + arcHeight;

        return new Vector3(xPos, yPos, 0f);
    }

    private Quaternion CalculateRotation(int index)
    {
        int cardCount = cards.Count;

        float normalizedX = cardCount > 1 ? (float)index / (cardCount - 1) : 0.5f;
        float angle = (normalizedX - 0.5f) * cardAngle * (cardCount - 1);

        return Quaternion.Euler(0, 0, angle);
    }

    public bool HasEmptySlot() => cards.Count < GameRules.MAX_HAND_SIZE;
    public int GetCardCount() => cards.Count;
    public void Clear() => cards.Clear();
    public bool Contains(Transform card) => cards.Contains(card);
    public int GetCardIndex(Transform card) => cards.IndexOf(card);
}
