using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [TKC] Action menu popup when card is selected.
/// </summary>
public class CardActionMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button summonButton; // Summon Monster
    [SerializeField] private TextMeshProUGUI summonButtonText;
    [SerializeField] private Button setButton; // Set Spell or Trap or Special
    [SerializeField] private Button activateButton; // Activate Spell
    [SerializeField] private Button cancelButton;

    [Header("Localization Text")]
    [SerializeField] private string summonTextVN;
    [SerializeField] private string summonTextEN;
    [SerializeField] private string sacrificeTextVN;
    [SerializeField] private string sacrificeTextEN;

    [Header("Game References")]
    [SerializeField] private MonsterZone playerMonsterZone;
    [SerializeField] private SpellZone spellZone;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Transform graveyardZone;

    [Header("Animation")]
    [SerializeField] private float showDuration = 0.2f;
    [SerializeField] private Ease showEase = Ease.OutBack;

    private static CardActionMenu instance;
    public static CardActionMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<CardActionMenu>();
            }
            return instance;
        }
    }

    private Card currentCard;
    private Tween scaleTween;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (playerMonsterZone == null)
        {
            playerMonsterZone = FindObjectOfType<MonsterZone>();
            if (playerMonsterZone == null)
                Debug.LogWarning($"[CardActionMenu] MonsterZone not found!");
        }

        if (spellZone == null)
        {
            spellZone = FindObjectOfType<SpellZone>();
            if (spellZone == null)
                Debug.LogWarning($"[SpellZone] MonsterZone not found!");
        }

        if (deckManager == null)
        {
            deckManager = FindObjectOfType<DeckManager>();
            if (deckManager == null)
                Debug.LogWarning($"[CardActionMenu] DeckManager not found!");
        }

        if (summonButton != null)
            summonButton.onClick.AddListener(OnSummonClicked);

        if (setButton != null)
            setButton.onClick.AddListener(OnSetClicked);

        if (activateButton != null)
            activateButton.onClick.AddListener(OnActivateClicked);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);

        menuPanel.SetActive(false);
    }

    /// <summary>
    /// [TKC] Show action menu for a card.
    /// </summary>
    public void ShowMenu(Card card)
    {
        if (card == null) return;
        if (menuPanel == null) return;

        currentCard = card;

        UpdateButtonStates();

        UpdateVisualSummonButton(card, true);

        scaleTween?.Kill();

        if (!menuPanel.activeSelf)
        {
            menuPanel.SetActive(true);
            menuPanel.transform.localScale = Vector3.zero;
        }

        scaleTween = menuPanel.transform.DOScale(Vector3.one, showDuration).SetEase(showEase);
    }


    /// <summary>
    /// [TKC] Hidden action menu.
    /// </summary>
    public void HideMenu()
    {
        if (menuPanel == null || !menuPanel.activeSelf) return;

        scaleTween?.Kill();

        scaleTween = menuPanel.transform.DOScale(Vector3.zero, showDuration).SetEase(showEase).OnComplete(() => menuPanel.SetActive(false));

        currentCard = null;
    }

    public void UpdateSacrificeButton(int selectedCount, int requiredCount)
    {
        bool isVietnamese = true;
            
        if (summonButton == null || summonButtonText == null) return;

        summonButtonText.text = isVietnamese ? $"{sacrificeTextVN} {selectedCount}/{requiredCount}" : $"{sacrificeTextEN} {selectedCount}/{requiredCount}";
        summonButton.interactable = (selectedCount == requiredCount);
    }

    private void OnSummonClicked()
    {
        if (currentCard == null) return;

        int stars = currentCard.GetCardData().Stars;

        if (stars <= 4)
        {
            SummonController.Instance?.RequestSummonDirect(currentCard);
        }
        else
        {
            CardSelectionManager selectionManager = CardSelectionManager.Instance;
            if (selectionManager == null || !selectionManager.IsSacrificeComplete()) return;

            SummonController summonController = SummonController.Instance;
            if (summonController != null)
            {
                summonController.ExecuteSummon(selectionManager.CurrentHandCard, selectionManager.SelectedSacrifices);
            }

            selectionManager.ClearCurrentHandCard();
        }
    }

    private void OnSetClicked()
    {
        if (currentCard == null) return;
        SpellController.Instance?.RequestSet(currentCard);
    }

    private void OnActivateClicked()
    {
        if (currentCard == null) return;
        SpellController.Instance?.RequestActivate(currentCard);
    }

    private void OnCancelClicked()
    {
        CardSelectionManager.Instance?.DeselectAll();
        //HideMenu();
    }

    private void UpdateVisualSummonButton(Card card, bool isVietnamese)
    {
        if (summonButton == null || summonButtonText == null) return;

        int stars = card.GetCardData().Stars;

        if (stars <= GameRules.LOW_TIER_MAX_STARS)
        {
            summonButtonText.text = isVietnamese ? summonTextVN : summonTextEN;
            summonButton.interactable = true;
        }
    }

    private void UpdateButtonStates()
    {
        if (currentCard == null) return;

        CardData cardData = currentCard.GetCardData();
        CardType type = cardData.Type;

        switch (type)
        {
            case CardType.Monster:
                summonButton.gameObject.SetActive(true);
                setButton.gameObject.SetActive(false);
                activateButton.gameObject.SetActive(false);
                break;

            case CardType.Spell:
                summonButton.gameObject.SetActive(false);

                if (cardData.SpellType == SpellType.Normal)
                {
                    setButton.gameObject.SetActive(false);
                    activateButton.gameObject.SetActive(true);

                    ISpellEffect effect = SpellEffectRegistry.BuildNormal(cardData);
                    if (effect == null) { activateButton.interactable = false; break; }
                    SpellContext context = new SpellContext
                    {
                        SpellCard = currentCard,
                        PlayerMonsterZone = playerMonsterZone,
                        DeckManager = deckManager,
                    };
                    activateButton.interactable = effect.CanActivate(context);
                }
                else
                {
                    setButton.gameObject.SetActive(true);
                    activateButton.gameObject.SetActive(false);

                    IContinuousSpellEffect effect = SpellEffectRegistry.BuildContinous(cardData);
                    if (effect == null) { setButton.interactable = false; break; }
                    SpellContext context = new SpellContext
                    {
                        SpellCard = currentCard, 
                        SpellZone = spellZone,
                    };
                    setButton.interactable = effect.CanActivate(context);
                }
                break;

            case CardType.Trap:
                summonButton.gameObject.SetActive(false);
                setButton.gameObject.SetActive(true);
                activateButton.gameObject.SetActive(false);
                break;

            case CardType.Special:
                summonButton.gameObject.SetActive(false);
                setButton.gameObject.SetActive(true);
                activateButton.gameObject.SetActive(false);
                break;
        }
    }
}
