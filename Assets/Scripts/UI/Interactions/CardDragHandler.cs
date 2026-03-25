using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// [TKC] Handles card dragging.
/// </summary>
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Settings")]
    [SerializeField] private float returnDuration = 0.3f;
    [SerializeField] private Ease returnEase = Ease.OutBack;

    [Header("Drag Restrictions")]
    [SerializeField] private bool canDragOnlyInHand = true;

    [Header("Swap Settings")]
    [SerializeField] private float swapDetectionRadius = 150f;

    [Header("Sorting")]
    [SerializeField] private int dragSortingOrder = 200;

    private static CardDragHandler currentItemDragging;

    private Vector3 dragOffset;
    private bool isDragging = false;

    private Card card;
    private HandLayoutManager handLayout;
    private CardTransformHelper transformHelper;

    private void Awake()
    {
        transformHelper = GetComponent<CardTransformHelper>();
        if (transformHelper == null)
        {
            transformHelper = gameObject.AddComponent<CardTransformHelper>();
        }
    }

    private void Start()
    {
        card = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!GamePhaseManager.Instance.CanDragCards())
        {
            eventData.pointerDrag = null;
            return;
        }

        if (currentItemDragging != null)
        {
            Debug.Log($"[Drag] Another card is dragging, blocking");
            eventData.pointerDrag = null;
            return;
        }

        if (!CanDrag() || isDragging)
        {
            eventData.pointerDrag = null;
            return;
        }

        currentItemDragging = this;
        isDragging = true;

        handLayout = GetComponentInParent<HandLayoutManager>();

        transformHelper.Canvas.sortingOrder = dragSortingOrder;
        dragOffset = transform.position - GetCursorPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        transform.position = GetCursorPosition(eventData) + dragOffset;

        CheckWapCandidate(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        CardSelectionManager.Instance?.DeselectAll();

        Card targetCard = FindSwapTarget(eventData.position);

        if (targetCard != null && targetCard != card)
        {
            SwapPosition(targetCard);
        }
        else
        {
            ReturnToOriginalPosition();
        }

        if (currentItemDragging == this)
        {
            currentItemDragging = null;
        }

        Debug.Log($"[Drag] End: {targetCard?.GetCardData()}");
    }

    private Card FindSwapTarget(Vector2 screenPos)
    {
        if (handLayout == null) return null;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = screenPos;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        foreach (var result in results)
        {
            Card otherCard = result.gameObject.GetComponent<Card>();

            if (otherCard != null && otherCard != card && otherCard.GetState() == CardState.InHand)
            {
                float distance = Vector2.Distance(transform.position, otherCard.transform.position);

                if (distance < swapDetectionRadius)
                {
                    return otherCard;
                }
            }
        }

        return null;
    }

    private void SwapPosition(Card targetCard)
    {
        if (handLayout == null || targetCard == null) return;

        handLayout.SwapCards(card, targetCard);
        handLayout.ArrangeCards(true);

        Vector3 targetScale = transformHelper.GetRestingScale();
        transform.DOScale(targetScale, returnDuration).SetEase(returnEase);

        Debug.Log($"[Swap] {card.GetCardData()} <-> {targetCard.GetCardData()}");
    }

    /// <summary>
    /// Visual feedback when dragging over swappable card.
    /// </summary>
    private void CheckWapCandidate(Vector2 screenPos)
    {
        Card targetCard = FindSwapTarget(screenPos);

        if (targetCard != null)
        {
            // TODO: Visual feedback (highlight target card)
            // Will implement in next iteration
        }
    }

    private bool CanDrag()
    {
        if (card != null && canDragOnlyInHand)
        {
            return card.CanInteract();
        }
        return false;
    }

    private Vector3 GetCursorPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
        transform.parent as RectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out Vector3 worldPos
        );
        return worldPos;
    }

    private void ReturnToOriginalPosition()
    {
        transform.DOKill();

        Sequence returnSeq = DOTween.Sequence();

        Vector3 targetScale = transformHelper.GetRestingScale();
        Vector3 targetPosition = transformHelper.GetRestingPosition();
        Quaternion targetRotation = transformHelper.GetRestingRotation();
        int targetSortingOrder = transformHelper.GetRestingSortingOrder();

        returnSeq.Append(transform.DOLocalMove(targetPosition, returnDuration).SetEase(returnEase));
        returnSeq.Join(transform.DOLocalRotateQuaternion(targetRotation, returnDuration).SetEase(returnEase));
        returnSeq.Join(transform.DOScale(targetScale, returnDuration).SetEase(returnEase));
        returnSeq.JoinCallback(() => transformHelper.Canvas.sortingOrder = targetSortingOrder);
    }
    public bool IsDragging() => isDragging;
}
