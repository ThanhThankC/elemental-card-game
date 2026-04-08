using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles card hover animations.
/// </summary>
public class CardClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Hover Settings")]
    [SerializeField] private float selectedOffsetY = 30f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Ease animationEase = Ease.OutBack;

    [Header("Click Protection")]
    [SerializeField] private float clickCooldown = 0.15f;

    [Header("Sorting")]
    [SerializeField] private int selectedSortingOrder = 100;

    private bool isSelected = false;
    private bool isInteractable = false;
    private float lastClickTime = 0;

    private Card card;
    private CardDragHandler dragHandler;
    private CardSelectionManager selectionManager;
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
        dragHandler = GetComponent<CardDragHandler>();
        selectionManager = CardSelectionManager.Instance;

        if (selectionManager == null)
        {
            Debug.LogWarning("$[CardClickHandler] CardSelectionManager not found!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GamePhaseManager.Instance.IsInSpellTargeting())
        {
            if (card.GetState() == CardState.OnField)
            {
                TargetingManager.Instance?.OnFeildCardClikedAsTarget(card);
            }
            return;
        }

        if (!GamePhaseManager.Instance.CanInteractWithCards()) return;
        if (!isInteractable) return;
        if (GamePhaseManager.Instance.IsInDiscardPhase() && card.GetState() != CardState.InHand) return;

        if (Time.time - lastClickTime < clickCooldown)
        {
            Debug.Log($"[Click] Cooldown active, ignoring click ({Time.time - lastClickTime:F3}s)");
            return;
        }
        lastClickTime = Time.time;

        if (dragHandler != null && dragHandler.IsDragging()) return;

        if (isSelected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    public void Select()
    {
        if (isSelected || !isInteractable) return;
        isSelected = true;

        selectionManager.OnCardSelected(card, this);
        PerformVisualSelect();
    }

    public void Deselect()
    {
        if (!isSelected) return;
        isSelected = false;

        selectionManager.OnCardDeselected(card);
        PerformVisualDeselect();
    }

    private void PerformVisualSelect()
    {
        float targetOffsetY = transformHelper.GetRestingPosition().y + selectedOffsetY;
        Quaternion targetRot = Quaternion.identity;
        Vector3 targetScale = CardVisualConfig.GetSelectedScale(card.GetState());

        transform.DOKill();
        transform.DOLocalMoveY(targetOffsetY, animationDuration).SetEase(animationEase);
        transform.DOLocalRotateQuaternion(targetRot, animationDuration).SetEase(animationEase);
        transform.DOScale(targetScale, animationDuration).SetEase(animationEase);

        transformHelper.Canvas.sortingOrder = selectedSortingOrder;
    }

    private void PerformVisualDeselect()
    {
        float targetOffsetY = transformHelper.GetRestingPosition().y;
        Quaternion targetRot = transformHelper.GetRestingRotation();
        Vector3 targetScale = transformHelper.GetRestingScale();

        transform.DOLocalMoveY(targetOffsetY, animationDuration).SetEase(animationEase);
        transform.DOLocalRotateQuaternion(targetRot, animationDuration).SetEase(animationEase);
        transform.DOScale(targetScale, animationDuration).SetEase(animationEase);

        transformHelper.Canvas.sortingOrder = transformHelper.GetRestingSortingOrder();
    }

    /// <summary>
    /// Enable/disable card interaction.
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;

        // If disabled while selected, deselect
        if (!interactable && isSelected)
        {
            Deselect();
        }
    }

    /// <summary>
    /// Forces deselection without triggering logic callbacks.
    /// </summary>
    public void ForceDeselect()
    {
        if (!isSelected) return;
        isSelected = false;

        PerformVisualDeselect();
    }

    public bool IsSelected() => isSelected;
}
