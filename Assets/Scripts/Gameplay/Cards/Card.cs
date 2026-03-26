using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Card : MonoBehaviour
{
    [Header("Card Data Reference")]
    [SerializeField] private CardData cardData;

    [Header("Card State")]
    [SerializeField] private CardState currentState;

    [Header("Basic UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Image cardArtImage;

    [Header("Combat Stats")]
    [SerializeField] private GameObject combatStats;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI defText;

    [Header("Attribute Group")]
    [SerializeField] private GameObject attributeGroup;

    [Header("AttributeGroup/Elements")]
    [SerializeField] private Transform elementPanel;
    [SerializeField] private GameObject elementIconPrefab;

    [Header("AttributeGroup/Buffs")]
    [SerializeField] private GameObject buffPanel;
    [SerializeField] private GameObject buffRubyGroup;
    [SerializeField] private GameObject buffLotusGroup;
    [SerializeField] private TextMeshProUGUI buffRubyText;
    [SerializeField] private TextMeshProUGUI buffLotusText;
    [SerializeField] private Image buffRubyIcon = null;
    [SerializeField] private Image buffLotusIcon = null;

    [Header("Description")]
    [SerializeField] private TextMeshProUGUI normalDescText;

    [SerializeField] private GameObject specialDescPanel;
    [SerializeField] private TextMeshProUGUI specialDescText;

    [Header("Background Color")]
    [SerializeField] private Image cardBackground;
    [SerializeField] private Color monsterColor = Color.yellow;
    [SerializeField] private Color spellColor = Color.green;

    public event Action OnStatsChanged;

    private int currentATK;
    private int currentDEF;
    private bool isVietnamese = true;

    private void Start()
    {
        if (cardData != null)
        {
            Initialize(cardData);
        }
    }

    public void Initialize(CardData data)
    {
        cardData = data;

        if (cardData.IsMonster())
        {
            currentATK = data.BaseATK;
            currentDEF = data.BaseDEF;
        }

        UpdateVisualCard();
    }

    private void UpdateVisualCard()
    {
        if (cardData == null) return;

        UpdateBasicInfo();

        bool isMonster = cardData.IsMonster();

        if (isMonster)
        {
            UpdateCombatStats();
            UpdateElementIcons();
            UpdateBuffPanel();
        }

        SetGroupVisibility(combatStats, isMonster);
        SetGroupVisibility(attributeGroup, isMonster);
        SetGroupVisibility(specialDescPanel, isMonster);
        UpdateDescription();
    }

    private void UpdateBasicInfo()
    {
        if (nameText != null)
        {
            nameText.text = cardData.GetCardName(isVietnamese);
        }

        if (cardArtImage != null && cardData.Artwork != null)
        {
            cardArtImage.sprite = cardData.Artwork;
            cardArtImage.GetComponent<Image>().SetNativeSize();
        }

        if (typeText != null)
        {
            typeText.text = cardData.IsMonster() ? GetStarsDisplay() : GetTypeText();
        }

        switch (cardData.Type)
        {
            case CardType.Monster:
                cardBackground.color = monsterColor;
                break;
            case CardType.Spell:
                cardBackground.color = spellColor;
                break;
        }
    }

    private void UpdateCombatStats()
    {
        if (atkText != null)
        {
            atkText.text = $"ATK: {currentATK}";
        }

        if (defText != null)
        {
            defText.text = $"DEF: {currentDEF}";
        }
    }

    private void UpdateElementIcons()
    {
        if (elementPanel == null) return;

        foreach (Transform child in elementPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (ElementType element in cardData.Elements)
        {
            GameObject iconObj = Instantiate(elementIconPrefab, elementPanel);
            Image iconImage = iconObj.GetComponent<Image>();

            if (iconImage != null)
            {
                iconImage.sprite = IconDatabase.Instance?.GetElementIcon(element);
            }
        }
    }

    private void UpdateBuffPanel()
    {
        if (buffPanel == null) return;

        bool hasLotus = cardData.LotusCount > 0;
        bool hasRuby = cardData.RubyCount > 0;

        bool hasBuff = hasLotus || hasRuby;
        SetGroupVisibility(buffPanel, hasBuff);

        if (!hasBuff) return;

        if (buffLotusGroup != null)
        {
            SetGroupVisibility(buffLotusGroup, hasLotus);
            if (hasLotus)
            {
                if (buffLotusText != null)
                    buffLotusText.text = cardData.LotusCount.ToString();

                if (buffLotusIcon != null)
                    buffLotusIcon.sprite = IconDatabase.Instance?.GetBuffIcon(BuffType.Lotus);
            }
        }

        if (buffRubyGroup != null)
        {
            SetGroupVisibility(buffRubyGroup, hasRuby);
            if (hasRuby)
            {
                if (buffRubyText != null)
                    buffRubyText.text = cardData.RubyCount.ToString();

                if (buffRubyIcon != null)
                    buffRubyIcon.sprite = IconDatabase.Instance?.GetBuffIcon(BuffType.Ruby);
            }
        }
    }

    private void UpdateDescription()
    {
        if (normalDescText != null)
        {
            normalDescText.text = cardData.GetNormalEffectDescription(isVietnamese);
        }

        bool hasSpecialEffect = cardData.HasSpecialBuff();
        SetGroupVisibility(specialDescPanel, hasSpecialEffect);

        if (hasSpecialEffect && specialDescText != null)
        {
            specialDescText.text = cardData.GetSpecialEffectDescription(isVietnamese);
        }
    }

    private void SetGroupVisibility(GameObject group, bool visibile)
    {
        if (group != null)
        {
            group.SetActive(visibile);
        }
    }

    private string GetTypeText()
    {
        switch (cardData.Type)
        {
            case CardType.Monster:
                return "Quai thu";
            case CardType.Spell:
                return "P";
            case CardType.Trap:
                return "B";
            case CardType.Special:
                return "SS";
            default:
                return "";
        }
    }

    private string GetStarsDisplay()
    {
        return $"{cardData.Stars}*";
    }

    public void ModifyATK(int amount)
    {
        currentATK += amount;
        UpdateCombatStats();
        OnStatsChanged?.Invoke();
    }

    public void ModifyDEF(int amount)
    {
        currentDEF += amount;
        if (currentDEF < 0) currentDEF = 0;
        UpdateCombatStats();
        OnStatsChanged?.Invoke();
    }

    public void CopyStatsFrom(Card source)
    {
        Initialize(source.GetCardData());

        int diffATK = source.GetCurrentATK() - source.GetCardData().BaseATK;
        int diffDEF = source.GetCurrentDEF() - source.GetCardData().BaseDEF;

        if (diffATK != 0) ModifyATK(diffATK);
        if (diffDEF != 0) ModifyDEF(diffDEF);
    }

    public void ApplySpecialCardBuff()
    {
        if (cardData == null) return;

        int buffAmount = cardData.GetTotalBuffValue();

        if (buffAmount > 0)
        {
            ModifyATK(buffAmount);
            ModifyDEF(buffAmount);

            Debug.Log($"[Card] {cardData.GetCardName(isVietnamese)} receives +{buffAmount} ATK/DEF buff");
        }
    }

    public void ResetStas()
    {
        if (cardData.IsMonster())
        {
            currentATK = cardData.BaseATK;
            currentDEF = cardData.BaseDEF;
            UpdateCombatStats();
        }
    }

    public CardData GetCardData() => cardData;
    public int GetCurrentATK() => currentATK;
    public int GetCurrentDEF() => currentDEF;
    public string GetCardID() => cardData.CardID;
    public CardState GetState() => currentState;

    /// <summary>
    /// [TKC] Set card state and update interaction.
    /// </summary>
    public void SetState(CardState newState)
    {
        currentState = newState;
        UpdateInteractability();
    }

    /// <summary>
    /// [TKC] Check if card can be interacted with.
    /// </summary>
    public bool CanInteract()
    {
        return currentState == CardState.InHand || currentState == CardState.OnField;
    }

    /// <summary>
    /// [TKC] Update whether card can be clicked/hovered.
    /// </summary>
    private void UpdateInteractability()
    {
        var selectedHandler = GetComponent<CardClickHandler>();
        if (selectedHandler != null)
        {
            selectedHandler.SetInteractable(CanInteract());
        }
    }
}
