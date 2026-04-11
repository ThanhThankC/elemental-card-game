using UnityEngine;

public class SpellController : BaseCardController, ICardController
{
    [Header("References")]
    [SerializeField] private SpellZone spellZone;

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
        CardEffectContext context = BuildContext(null);
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

    private void ActiveContinuous(IContinuousSpellEffect continuousEffect, CardEffectContext context)
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
            bool removed = handLayout.RemoveCard(pendingCard.transform);
            if (!removed) return;
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

    private void ExecuteNormal(CardEffectContext context)
    {
        if (pendingEffect.SendToGraveyardFirst)
        {
            handLayout.RemoveCard(pendingCard.transform);
            SendToGraveyard(pendingCard, graveyardZone);
        }

        pendingEffect.Execute(context);

        if (!pendingEffect.SendToGraveyardFirst)
        {
            handLayout.RemoveCard(pendingCard.transform);
            SendToGraveyard(pendingCard, graveyardZone);
        }

        pendingCard = null;
        pendingEffect = null;
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
    }

    protected override void OnAfterTargetExecute(Card card, CardEffectContext context)
    {
        bool removed = handLayout.RemoveCard(card.transform);
        if (!removed)
        {
            Debug.LogWarning($"[SpellController] Failed to remove: {card.GetCardData().name}");
            return;
        }
        SendToGraveyard(card, graveyardZone);
    }
}
