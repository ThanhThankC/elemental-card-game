using UnityEngine;

public abstract class BaseCardController : MonoBehaviour
{
    protected Card pendingCard;
    protected ISpellEffect pendingEffect;
    public bool IsWaitingForTarget => pendingCard != null && pendingEffect != null;

    protected void EnterTargetingMode()
    {
        GamePhaseManager.Instance?.SetPhase(GamePhase.SpellTargeting);
    }

    protected void SendToGraveyard(Card card, HandLayoutManager handLayout, Transform graveyardZone)
    {
        handLayout.RemoveCard(card.transform);
        //TODO: Add disappear effect
        card.SetState(CardState.InGraveyard);
        card.transform.position = graveyardZone.position;
        card.gameObject.SetActive(false);
        CardSelectionManager.Instance?.NotifyCardSentToGraveyard(card);    
    }

    protected abstract bool ValidateCard(Card card);
    protected abstract void OnRequestFromHand();
    protected abstract void OnRequestFromField();
    protected abstract SpellContext BuildContext(Card card);
}
