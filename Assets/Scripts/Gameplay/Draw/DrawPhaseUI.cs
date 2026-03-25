using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawPhaseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button drawButton;
    [SerializeField] private TextMeshProUGUI drawButtonText;
    [SerializeField] private Button discardButton;
    [SerializeField] private TextMeshProUGUI discardButtonText;

    [Header("Animation")]
    [SerializeField] private float scaleAnimationDuration = 0.2f;
    [SerializeField] private Ease scaleEase = Ease.OutCubic;
    [SerializeField] private float normalScale = 0.8f;
    [SerializeField] private float largeScale = 1.1f;

    [Header("Visual Effect")]
    [SerializeField] private Image discardModeOverlay;

    private DiscardManager discardManager;
    private DrawPhaseController drawController;
    private GamePhaseManager gamePhaseManager;

    private void Start()
    {
        discardManager = DiscardManager.Instance;
        drawController = DrawPhaseController.Instance;
        gamePhaseManager = GamePhaseManager.Instance;

        if (discardManager == null || drawController == null || gamePhaseManager == null)
        {
            Debug.LogError("$[DrawPhaseUI] Instance not found!");
            return;
        }

        if (drawButton != null)
            drawButton.onClick.AddListener(OnDrawButtonClicked);

        if (discardButton != null)
            discardButton.onClick.AddListener(OnDiscardButtonCliked);

        discardManager.OnDiscardRequirementChanged += UpdateButtonVisuals;
        gamePhaseManager.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDestroy()
    {
        if (discardManager != null)
        {
            discardManager.OnDiscardRequirementChanged -= UpdateButtonVisuals;
        }

        if (gamePhaseManager != null)
        {
            gamePhaseManager.OnPhaseChanged -= OnPhaseChanged;
        }

        drawButton?.transform.DOKill();
        discardButton?.transform.DOKill();
    }

    private void OnDrawButtonClicked()
    {
        drawController.ExcecuteDraw();
    }

    private void OnDiscardButtonCliked()
    {
        bool isDiscardPhase = gamePhaseManager.IsInDiscardPhase();

        if (!isDiscardPhase)
        {
            CardSelectionManager.Instance?.DeselectAll();
            gamePhaseManager.SetPhase(GamePhase.DiscardSelection);
        }
        else
        {
            CardSelectionManager.Instance?.ClearDiscardVisuals();
            CardSelectionManager.Instance?.ClearAllDiscard();
            gamePhaseManager.SetPhase(GamePhase.Idle);
        }
    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        bool isDiscardPhase = newPhase == GamePhase.DiscardSelection;

        if (discardModeOverlay != null)
        {
            discardModeOverlay.gameObject.SetActive(isDiscardPhase);
        }
        UpdateButtonVisuals(discardManager.SelectedDiscardCount, discardManager.RequiredDiscardCount);
    }

    private void UpdateButtonVisuals(int selected, int required)
    {
        bool isDiscardPhase = gamePhaseManager.IsInDiscardPhase();
        bool hasRequirement = required > 0;
        bool meetsRequirement = selected == required;

        drawButton.interactable = !hasRequirement || meetsRequirement;
        discardButton.interactable = hasRequirement;
        discardModeOverlay.gameObject.SetActive(isDiscardPhase);

        if (!hasRequirement)
        {
            UpdateButtons("Draw", "Discard", largeScale, normalScale);
            return;
        }

        if (!isDiscardPhase)
        {
            UpdateButtons("", $"Discard ({required})", normalScale, normalScale);
        }
        else
        {
            string drawText = meetsRequirement ? "Draw" : $"Required ({selected}/{required})";
            float drawScale = meetsRequirement ? largeScale : normalScale;
            float discardScale = meetsRequirement ? normalScale : largeScale;

            UpdateButtons(drawText, "Cancel", drawScale, discardScale);
        }
    }

    private void UpdateButtons(string drawTxt, string discardTxt, float drawScl, float discardScl)
    {
        drawButtonText.text = drawTxt;
        discardButtonText.text = discardTxt;
        SetButtonScale(drawButton, drawScl);
        SetButtonScale(discardButton, discardScl);
    }

    private void SetButtonScale(Button button, float targetScale)
    {
        if (button == null) return;

        button.transform.DOKill();
        button.transform.DOScale(targetScale, scaleAnimationDuration).SetEase(scaleEase);
    }
}
