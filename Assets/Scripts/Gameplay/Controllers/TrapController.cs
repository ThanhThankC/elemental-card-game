using UnityEngine;

public class TrapController : BaseCardController, ICardController
{
    [Header("References")]
    [SerializeField] private TrapZone trapZone;

    [Header("Animation")]
    [SerializeField] private float setDuration = 0.4f;
    [SerializeField] private float flipDuration = 0.4f;

    private static TrapController instance;
    public static TrapController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TrapController>();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void RequestAction(Card trapCard)
    {
        if (trapCard == null) return;
        if (!ValidateCard(trapCard)) return;

        pendingCard = trapCard;

        CardActionMenu.Instance?.HideMenu();

        if (trapCard.GetState() == CardState.InHand)
            OnRequestFromHand();
        else if (trapCard.GetState() == CardState.OnField)
            OnRequestFromField();
    }

    protected override bool ValidateCard(Card card) => card.GetCardData().Type == CardType.Trap;

    protected override void OnRequestFromHand()
    {
        CardSelectionManager.Instance?.DeselectAll();

        int slotIndex = trapZone.FindEmptySlot();
        Transform targetSlot = trapZone.GetSlotTransform(slotIndex);

        if (targetSlot == null)
        {
            Debug.LogWarning("[TrapController] Target transform is null!");
            pendingCard.SetState(CardState.InHand);
            return;
        }

        GamePhaseManager.Instance.SetPhase(GamePhase.Setting);
        if (handLayout != null)
        {
            bool removed = handLayout.RemoveCard(pendingCard.transform);
            if (!removed) return;
        }

        Card cardToPlace = pendingCard;

        CardAnimator.AnimateToField(
            cardToPlace,
            targetSlot,
            setDuration,
            flipDuration,
            onComplete: () =>
            {
                trapZone.PlaceTrap(cardToPlace, slotIndex);
                cardToPlace.SetState(CardState.OnField);
                GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
            }
         );

        pendingCard = null;
    }

    protected override void OnRequestFromField()
    {
        CardData data = pendingCard.GetCardData();
        pendingEffect = TrapEffectRegistry.Build(data);

        if (pendingEffect == null)
        {
            Debug.LogWarning($"[TrapController] No effect found for {data.GetCardName()}");
            return;
        }

        CardEffectContext context = BuildContext(null);
        if (!pendingEffect.CanActivate(context))
        {
            Debug.LogWarning($"[TrapController] Cannot activate: {data.GetCardName()}");
            return;
        }

        Card cardToPlace = pendingCard;

        CardAnimator.AnimateFlipFaceUp(cardToPlace, onComplete: () =>
        {
            if (pendingEffect.NeedsTarget)
            {
                EnterTargetingMode(instance, pendingEffect.TargetType);
                return;
            }

            ExecuteTrap(context);
        });
    }

    private void ExecuteTrap(CardEffectContext context)
    {
        Card card = pendingCard;      
        ISpellEffect effect = pendingEffect;

        pendingCard = null;             
        pendingEffect = null;

        effect.Execute(context);
        bool removed = trapZone.RemoveCard(card);
        if (!removed)
        {
            Debug.LogWarning($"[TrapController] Failed to remove: {card.GetCardData().name}");
            return;
        }
        SendToGraveyard(card, graveyardZone);
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle); 
    }

    protected override void OnAfterTargetExecute(Card card, CardEffectContext context)
    {
        bool removed = trapZone.RemoveCard(card);
        if (!removed)
        {
            Debug.LogWarning($"[TrapController] Failed to remove: {card.GetCardData().name}");
            return;
        }
        SendToGraveyard(card, graveyardZone);
    }

    public void RequestRecall(Card trapCard)
    {
        if (trapCard == null) return;
        if (!ValidateCard(trapCard)) return;

        CardSelectionManager.Instance?.DeselectAll();
        CardActionMenu.Instance?.HideMenu();

        if (!handLayout.HasEmptySlot())
        {
            Debug.LogWarning($"[TrapController] No empty slots available in hand");
            return;
        }

        GamePhaseManager.Instance.SetPhase(GamePhase.Setting);

        Card cardToPlace = trapCard;

        CardAnimator.AnimateToHand(
            cardToPlace,
            setDuration,
            flipDuration,
            onComplete: () =>
            {
                bool removed = trapZone.RemoveCard(cardToPlace);
                if (!removed)
                {
                    Debug.LogWarning("[TrapController] Failed to remove from trapZone");
                    return;
                }
                handLayout.AddCard(cardToPlace.transform);
                cardToPlace.SetState(CardState.InHand);
                GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
            }
        );
    }
}
