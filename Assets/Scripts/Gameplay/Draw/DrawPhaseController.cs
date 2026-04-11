using System.Threading;
using UnityEngine;

public class DrawPhaseController : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private HandLayoutManager handLayout;
    [SerializeField] private Transform graveyardZone;

    private static DrawPhaseController instance;
    public static DrawPhaseController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DrawPhaseController>();
            }
            return instance;
        }
    }
    private DiscardManager discardManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        discardManager = DiscardManager.Instance;

        if (discardManager == null)
        {
            Debug.LogWarning("[DrawPhaseController] DiscardManager not found!");
        }
    }

    /// <summary>
    /// Excecute draw with discard if need.
    /// </summary>
    public void ExcecuteDraw()
    {
        CardSelectionManager.Instance?.DeselectAll();

        DiscardSelectedCards();

        CardSelectionManager.Instance?.ClearAllDiscard();

        GamePhaseManager.Instance.SetPhase(GamePhase.Drawing);

        deckManager.OnDrawCard();

        discardManager.ResetState();
    }
     
    private void DiscardSelectedCards()
    {
        foreach (var card in CardSelectionManager.Instance.SelectedDiscards)
        {
            if (card != null)
            {
                if (handLayout != null)
                {
                    bool removed = handLayout.RemoveCard(card.transform);
                    if (!removed) return;
                }
                CardSelectionManager.Instance?.NotifyCardSentToGraveyard(card);

                CardAnimator.AnimateToGraveyard(
                    card.transform,
                    graveyardZone,
                    onComplete: () => card.SetState(CardState.InGraveyard)
                );
            }
        }
    }
}