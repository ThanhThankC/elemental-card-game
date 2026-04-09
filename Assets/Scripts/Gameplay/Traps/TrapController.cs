using UnityEngine;

public class TrapController : BaseCardController, ITargetableController
{
    [Header("References")]
    [SerializeField] private MonsterZone playerMonsterZone;
    [SerializeField] private TrapZone trapZone;
    [SerializeField] private HandLayoutManager handLayout;
    [SerializeField] private Transform graveyardZone;
    [SerializeField] private DeckManager deckManager;

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

    public void OnTargetSelected(Card targetCard)
    {
        if (pendingCard == null || pendingEffect == null) return;
        if (targetCard == null) return;

        SpellContext context = BuildContext(targetCard);
        if (!pendingEffect.CanActivate(context))
        {
            Debug.LogWarning($"[TrapController] Invalid target: {targetCard.GetCardData().GetCardName()}");
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
        throw new System.NotImplementedException();
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
        handLayout?.RemoveCard(pendingCard.transform);

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

        TrapContext context = BuildContext(null) as TrapContext;
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

    private void ExecuteTrap(TrapContext context)
    {
        Card card = pendingCard;      
        ISpellEffect effect = pendingEffect;

        pendingCard = null;             
        pendingEffect = null;

        effect.Execute(context);
        trapZone.RemoveCard(card);
        SendToGraveyard(card, handLayout, graveyardZone);
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle); 
    }

    protected override SpellContext BuildContext(Card targetCard)
    {
        return new TrapContext
        {
            SpellCard = pendingCard,
            TargetMonster = targetCard,
            PlayerMonsterZone = playerMonsterZone,
            GraveyardZone = graveyardZone
        };
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
                handLayout.AddCard(cardToPlace.transform);
                trapZone.RemoveCard(cardToPlace);
                cardToPlace.SetState(CardState.InHand);
                GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
            }
        );
    }
}
