using UnityEngine;

public class SpellController : BaseCardController, ITargetableController
{
    [Header("References")]
    [SerializeField] private MonsterZone playerMonsterZone;
    [SerializeField] private SpellZone spellZone;
    [SerializeField] private HandLayoutManager handLayout;
    [SerializeField] private Transform graveyardZone;
    [SerializeField] private DeckManager deckManager;

    [Header("Animation")]
    [SerializeField] private float setDuration = 0.4f;

    private static SpellController instance;
    public static SpellController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SpellController>();
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

    public void RequestAction(Card spellCard)
    {
        if (spellCard == null) return;
        if (!ValidateCard(spellCard)) return;

        CardSelectionManager.Instance?.DeselectAll();
        CardActionMenu.Instance?.HideMenu();

        CardData data = spellCard.GetCardData();
        pendingCard = spellCard;
        pendingEffect = SpellEffectRegistry.Build(data);

        if (pendingEffect == null)
        {
            Debug.LogWarning($"[SpellController] No effect found for {data.GetCardName()}");
            return;
        }

        //TODO refactor: In future spellCard required both: NonTarget + Target ??;
        SpellContext context = BuildContext(null);
        if (!pendingEffect.CanActivate(context))
        {
            Debug.LogWarning($"[SpellController] Cannot activate: {data.GetCardName()}");
            return;
        }

        if (spellCard.GetState() == CardState.InHand)
            OnRequestFromHand();
        else if (spellCard.GetState() == CardState.OnField)
            OnRequestFromField();
    }

    protected override bool ValidateCard(Card card) => card.GetCardData().Type == CardType.Spell;

    protected override void OnRequestFromHand()
    {
        if (pendingEffect is IContinuousSpellEffect continuousEffect)
        {
            ActiveContinuous(continuousEffect, BuildContext(null));
            return;
        }

        if (pendingEffect.NeedsTarget)
        {
            EnterTargetingMode(instance, pendingEffect.TargetType);
            return;
        }

        ExecuteNormal(BuildContext(null));
    }

    protected override void OnRequestFromField() { }

    public void OnTargetSelected(Card targetCard)
    {
        if (pendingCard == null || pendingEffect == null) return;
        if (targetCard == null) return;

        SpellContext context = BuildContext(targetCard);
        if (!pendingEffect.CanActivate(context))
        {
            Debug.LogWarning($"[SpellController] Invalid target: {targetCard.GetCardData().GetCardName()}");
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

        Debug.Log("[SpellController] Targeting cancelled");
        pendingCard = null;
        pendingEffect = null;
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
        CardActionMenu.Instance?.ShowMenu(CardSelectionManager.Instance?.CurrentHandCard);
    }

    protected override SpellContext BuildContext(Card target)
    {
        return new SpellContext
        {
            SpellCard = pendingCard,
            TargetMonster = target,
            PlayerMonsterZone = playerMonsterZone,
            DeckManager = deckManager,
            GraveyardZone = graveyardZone
        };
    }

    private void ActiveContinuous(IContinuousSpellEffect continuousEffect, SpellContext context)
    {
        int slotIndex = spellZone.FindEmptySlot();
        Transform targetSlot = spellZone.GetSlotTransform(slotIndex);

        if (targetSlot == null)
        {
            Debug.LogWarning("[SpellController] Target transform is null!");
            pendingCard.SetState(CardState.InHand);
            return;
        }

        GamePhaseManager.Instance.SetPhase(GamePhase.Setting);
        CardSelectionManager.Instance?.DeselectAll();

        if (handLayout != null)
        {
            handLayout.RemoveCard(pendingCard.transform);
        }

        Card cardToPlace = pendingCard;

        CardAnimator.AnimateToField(
            pendingCard,
            targetSlot,
            setDuration,
            onComplete: () =>
            {
                spellZone.PlaceSpell(cardToPlace, slotIndex);
                cardToPlace.SetState(CardState.OnField);
                ContinuousSpellManager.Instance?.Register(cardToPlace, continuousEffect);
                continuousEffect.Execute(context);
                GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
            }
         );

        pendingCard = null;
        pendingEffect = null;
    }

    private void ExecuteNormal(SpellContext context)
    {
        if (pendingEffect.SendToGraveyardFirst)
            SendToGraveyard(pendingCard, handLayout, graveyardZone);

        pendingEffect.Execute(context);

        if (!pendingEffect.SendToGraveyardFirst)
            SendToGraveyard(pendingCard, handLayout, graveyardZone);

        pendingCard = null;
        pendingEffect = null;
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsWaitingForTarget)
        {
            CancelTargeting();
        }
    }
}
