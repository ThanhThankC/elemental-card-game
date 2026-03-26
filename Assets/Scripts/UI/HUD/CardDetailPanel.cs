using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float panelWidth = 300f;
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private Ease animEase = Ease.OutCubic;

    [Header("Preview Card")]
    [SerializeField] private Card previewCard;

    private CardSelectionManager selectionManager;
    private bool isVisible = false;
    private Card focusedCard;

    private void Start()
    {
        selectionManager = CardSelectionManager.Instance;
        if (selectionManager != null)
        {
            selectionManager.OnCardFocused += OnCardFocused;
            selectionManager.OnCardUnfocused += OnCardUnfocused;
        }

        panelRect.localScale = Vector3.zero;
    }

    private void OnDestroy()
    {
        if (selectionManager != null)
        {
            selectionManager.OnCardFocused -= OnCardFocused;
            selectionManager.OnCardUnfocused -= OnCardUnfocused;
        }
    }

    private void OnCardFocused(Card card)
    {
        if (card == null) return;

        if (focusedCard != null)
        {
            focusedCard.OnStatsChanged -= RefreshStats;
        }

        focusedCard = card;
        focusedCard.OnStatsChanged += RefreshStats;

        previewCard.CopyStatsFrom(card);
        ShowPanel();
    }

    private void OnCardUnfocused()
    {
        if (focusedCard != null)
        {
            focusedCard.OnStatsChanged -= RefreshStats;
            focusedCard = null;
        }

        HidePanel();
    }

    private void ShowPanel()
    {
        if (panelRect == null || isVisible) return;
        isVisible = true;

        panelRect.transform.DOKill();
        panelRect.DOScale(Vector3.one, animDuration).SetEase(animEase);
    }

    private void HidePanel()
    {
        if (panelRect == null || !isVisible) return;
        isVisible = false;

        panelRect.transform.DOKill();
        panelRect.DOScale(Vector3.zero, animDuration).SetEase(animEase);
    }

    private void RefreshStats()
    {
        if (focusedCard != null)
            previewCard.CopyStatsFrom(focusedCard);
    }
}
