using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages card selection in hand (only one card can be selected at a time).
/// </summary>
public class CardSelectionManager : MonoBehaviour
{
    private static CardSelectionManager instance;

    public static CardSelectionManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CardSelectionManager");
                instance = go.AddComponent<CardSelectionManager>();
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }

    private SelectionContext context;
    private SelectionValidator validator;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        context = new SelectionContext();
        validator = new SelectionValidator();
    }

    public void OnCardSelected(Card card, CardClickHandler clickHandler)
    {
        if (card == null) return;

        if (card.GetState() == CardState.InHand)
            HandleSelectInHand(card, clickHandler);
        else if (card.GetState() == CardState.OnField)
            HandleSelectOnField(card, clickHandler);
    }

    public void OnCardDeselected(Card card)
    {
        if (card == null) return;

        if (card.GetState() == CardState.InHand)
            HandleDeselectInHand(card);
        else if (card.GetState() == CardState.OnField)
            HandleDeselectOnField(card);
    }

    //New
    private void HandleSelectInHand(Card card, CardClickHandler clickHandler)
    {
        if (GamePhaseManager.Instance.IsInDiscardPhase())
        {
            context.AddDiscard(card); //TODO: handling work of Discard...
            DiscardManager.Instance?.UpdateSelectedDiscardCount(context.SelectedDiscardCount);
            return;
        }

        if (context.HasSacrifices())
        {
            bool shouldClear = validator.ShouldClearSacrifice(card, context.CurrentHandCard);

            if (shouldClear)
            {
                ClearSacrificeVisuals();
                context.ClearSacrifices();
                UpdateSacrificeButton();
            }
        }

        if (context.HasFieldCard())
        {
            DeselectFieldCardVisual();
            context.ClearFieldCard();
        }

        if (context.HasHandCard() && context.CurrentHandCardHandler != clickHandler)
        {
            DeselectHandCardVisual();
        }
        context.SetHandCard(card, clickHandler);

        var (tier, count) = validator.GetSacrificeRequirement(card);
        context.SetSacrificeRequirements(tier, count);

        if (context.IsInSacrificeMode())
        {
            UpdateSacrificeButton();
        }

        CardActionMenu.Instance?.ShowMenu(card);
    }

    //New
    private void HandleSelectOnField(Card card, CardClickHandler clickHandler)
    {
        if (GamePhaseManager.Instance.IsInDiscardPhase()) return;

        if (!context.HasHandCard())
        {
            if (context.HasSacrifices())
            {
                ClearSacrificeVisuals();
                context.ClearSacrifices();
                UpdateSacrificeButton();
            }

            if (context.HasFieldCard() && context.CurrentFieldCardHandler != clickHandler)
            {
                DeselectFieldCardVisual();
            }
            context.SetFieldCard(card, clickHandler);
            return;
        }

        if (!validator.MatchesSacrificeTier(card, context.RequiredSacrificeTier))
        {
            DeselectHandCardVisual();
            ClearSacrificeVisuals();
            context.ClearAll();
            CardActionMenu.Instance?.HideMenu();

            context.SetFieldCard(card, clickHandler);
            return;
        }

        context.AddSacrifice(card);
        UpdateSacrificeButton();
    }

    //New
    private void HandleDeselectInHand(Card card)
    {
        if (GamePhaseManager.Instance.IsInDiscardPhase())
        {
            context.RemoveDiscard(card);
            DiscardManager.Instance?.UpdateSelectedDiscardCount(context.SelectedDiscardCount);
            return;
        }

        CardActionMenu.Instance?.HideMenu();
        context.ClearHandCard();

        if (context.HasSacrifices())
        {
            ClearSacrificeVisuals();
            context.ClearSacrifices();
            UpdateSacrificeButton();
        }
    }

    private void HandleDeselectOnField(Card card)
    {
        if (!context.HasHandCard()) return;

        context.RemoveSacrifice(card);
        UpdateSacrificeButton();
    }

    public void DeselectAll()
    {
        DeselectHandCardVisual();
        DeselectFieldCardVisual();
        ClearSacrificeVisuals();

        context.ClearAll();
        CardActionMenu.Instance?.HideMenu();
    }

    private void DeselectHandCardVisual()
    {
        if (context.CurrentHandCardHandler != null)
        {
            context.CurrentHandCardHandler.ForceDeselect();
        }
    }

    private void DeselectFieldCardVisual()
    {
        if (context.CurrentFieldCardHandler != null)
        {
            context.CurrentFieldCardHandler.ForceDeselect();
        }
    }

    private void ClearSacrificeVisuals()
    {
        foreach (Card sacrifice in context.SelectedSacrifices)
        {
            CardClickHandler clickHandler = sacrifice?.GetComponent<CardClickHandler>();
            if (clickHandler != null && clickHandler.IsSelected())
            {
                clickHandler.ForceDeselect();
            }
        }
    }

    //New
    public void ClearDiscardVisuals()
    {
        foreach (Card discard in context.SelectedDiscards)
        {
            CardClickHandler clickHandler = discard?.GetComponent<CardClickHandler>();
            if (clickHandler != null && clickHandler.IsSelected())
            {
                clickHandler.ForceDeselect();
            }
        }
    }

    //New
    private void UpdateSacrificeButton()
    {
        CardActionMenu.Instance?.UpdateSacrificeButton(
            context.SelectedSacrifices.Count,
            context.RequiredSacrificeCount
        );
    }


    //New
    public void ClearAllDiscard()
    {
        context.ClearDiscards(); 
        DiscardManager.Instance?.UpdateSelectedDiscardCount(0); 
    }

    public Card CurrentHandCard => context.CurrentHandCard;
    public IReadOnlyList<Card> SelectedSacrifices => context.SelectedSacrifices;
    public void ClearCurrentHandCard() => context.ClearHandCard();
    public bool IsSacrificeComplete() => context.IsSacrificeComplete();
    //New
    public int GetSelectedDiscardCount() => context.SelectedDiscardCount;
    public IReadOnlyList<Card> SelectedDiscards => context.SelectedDiscards;
}

