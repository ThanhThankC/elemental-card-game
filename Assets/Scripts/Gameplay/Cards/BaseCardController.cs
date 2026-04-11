using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public abstract class BaseCardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected MonsterZone playerMonsterZone;
    [SerializeField] protected HandLayoutManager handLayout;
    [SerializeField] protected Transform graveyardZone;
    [SerializeField] protected DeckManager deckManager;

    protected Card pendingCard;
    protected ISpellEffect pendingEffect;

    public void OnTargetSelected(Card targetCard)
    {
        if (pendingCard == null || pendingEffect == null) return;
        if (targetCard == null) return;

        CardEffectContext context = BuildContext(targetCard);
        if (!pendingEffect.CanActivate(context))
        {
            Debug.LogWarning($"[{GetType().Name}] Invalid target: {targetCard.GetCardData().GetCardName()}");
            return;
        }

        pendingEffect.Execute(context);
        SendToGraveyard(pendingCard, handLayout, graveyardZone);

        pendingCard = null;
        pendingEffect = null;
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
    }

    public void CancelTargeting()
    {
        if (pendingCard == null || pendingEffect == null) return;

        Debug.Log($"[{GetType().Name}] Targeting cancelled");
        pendingCard = null;
        pendingEffect = null;
        TargetingManager.Instance?.UnRegister();
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
        CardActionMenu.Instance?.ShowMenu(CardSelectionManager.Instance?.CurrentHandCard);
    }

    protected void EnterTargetingMode(BaseCardController controller, TargetType targetType)
    {
        TargetingManager.Instance?.Register(controller, targetType);
        GamePhaseManager.Instance?.SetPhase(GamePhase.SpellTargeting);
    }

    protected void SendToGraveyard(Card card, HandLayoutManager handLayout, Transform graveyardZone)
    {
        bool removed = handLayout.RemoveCard(card.transform);
        if (!removed)
        {
            Debug.LogWarning($"[BaseCardController] Failed to remove: {card.GetCardData().name}");
            return;
        }
        //TODO: Add disappear effect
        card.SetState(CardState.InGraveyard);
        card.transform.position = graveyardZone.position;
        card.gameObject.SetActive(false);
        CardSelectionManager.Instance?.NotifyCardSentToGraveyard(card);    
    }
    protected CardEffectContext BuildContext(Card target)
    {
        return new CardEffectContext
        {
            SourceCard = pendingCard,
            TargetCard = target,
            PlayerMonsterZone = playerMonsterZone,
            DeckManager = deckManager,
            GraveyardZone = graveyardZone,
            HandLayout = handLayout
        };
    }

    protected abstract bool ValidateCard(Card card);
    protected abstract void OnRequestFromHand();
    protected abstract void OnRequestFromField();
}
