using System.Collections.Generic;

/// <summary>
/// Holds the current selection state.
/// No logic, just data. This makes state easy to track and debug.
/// </summary>
public class SelectionContext
{
    private Card currentHandCard;
    private CardClickHandler currentHandCardHandler;
    private Card currentFieldCard;
    private CardClickHandler currentFieldCardHandler;

    public Card CurrentHandCard => currentHandCard;
    public CardClickHandler CurrentHandCardHandler => currentHandCardHandler;
    public Card CurrentFieldCard => currentFieldCard;
    public CardClickHandler CurrentFieldCardHandler => currentFieldCardHandler;

    public void SetHandCard(Card card, CardClickHandler clickHandler)
    {
        currentHandCard = card;
        currentHandCardHandler = clickHandler;
    }

    public void ClearHandCard()
    {
        currentHandCard = null;
        currentHandCardHandler = null;
    }

    public void SetFieldCard(Card card, CardClickHandler clickHandler)
    {
        currentFieldCard = card;
        currentFieldCardHandler = clickHandler;
    }

    public void ClearFieldCard()
    {
        currentFieldCard = null;
        currentFieldCardHandler = null;
    }

    public bool HasHandCard() => currentHandCard != null;
    public bool HasFieldCard() => currentFieldCard != null;



    //Sacrifice Mode

    private List<Card> selectedSacrifices = new List<Card>();
    private MonsterTier requiredSacrificeTier;
    private int requiredSacrificeCount;

    public IReadOnlyList<Card> SelectedSacrifices => selectedSacrifices.AsReadOnly();
    public MonsterTier RequiredSacrificeTier => requiredSacrificeTier;
    public int RequiredSacrificeCount => requiredSacrificeCount;

    public void SetSacrificeRequirements(MonsterTier tier, int count)
    {
        requiredSacrificeTier = tier;
        requiredSacrificeCount = count;
    }

    public void AddSacrifice(Card card)
    {
        if (!selectedSacrifices.Contains(card))
        {
            selectedSacrifices.Add(card);
        }
    }

    public void RemoveSacrifice(Card card)
    {
        if (selectedSacrifices.Contains(card))
        {
            selectedSacrifices.Remove(card);
        }
    }

    public void ClearSacrifices()
    {
        selectedSacrifices.Clear();
    }

    public bool IsInSacrificeMode()
    {
        return currentHandCard != null && requiredSacrificeCount > 0;
    }

    public void ClearAll()
    {
        ClearHandCard();
        ClearFieldCard();
        ClearSacrifices();
        requiredSacrificeTier = MonsterTier.None;
        requiredSacrificeCount = 0;
    }

    public bool IsSacrificeComplete() => selectedSacrifices.Count == requiredSacrificeCount;
    public bool HasSacrifices() => selectedSacrifices.Count > 0;


    //New
    //Discard Mode

    private List<Card> selectedDiscardCards = new List<Card>();

    public void AddDiscard(Card card)
    {
        if (!selectedDiscardCards.Contains(card))
        {
            selectedDiscardCards.Add(card);
        }
    }

    public void RemoveDiscard(Card card)
    {
        if (selectedDiscardCards.Contains(card))
        {
            selectedDiscardCards.Remove(card);
        }
    }

    public void ClearDiscards()
    {
        selectedDiscardCards.Clear();
    }

    public int SelectedDiscardCount => selectedDiscardCards.Count;
    public IReadOnlyList<Card> SelectedDiscards => selectedDiscardCards.AsReadOnly();
}