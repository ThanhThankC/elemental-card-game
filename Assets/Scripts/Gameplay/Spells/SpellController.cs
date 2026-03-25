using UnityEngine;

public class SpellController : MonoBehaviour
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
                instance = FindAnyObjectByType<SpellController>();
            return instance;
        }
    }

    private Card spellCard;
    private ISpellEffect effect;

    public bool IsWaitingForTarget => spellCard != null && effect != null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void RequestActivate(Card spellCard)
    {
        if (spellCard == null) return;

        CardData data = spellCard.GetCardData();
        this.spellCard = spellCard;
        effect = SpellEffectRegistry.Build(data);

        if (effect == null)
        {
            Debug.LogWarning($"[SpellController] No effect found for {data.GetCardName()}");
            return;
        }

        //TODO refactor: In future spellCard required both: NonTarget + Target ??;
        SpellContext context = BuildContext(null);
        if (!effect.CanActivate(context))
        {
            Debug.LogWarning($"[SpellController] Cannot activate: {data.GetCardName()}");
            return;
        }

        if (effect is IContinuousSpellEffect continuousEffect)
        {
            ActiveContinuous(continuousEffect, context);
            return;
        }

        if (effect.NeedsTarget)
        {
            EnterTargetingMode();
            return;
        }

        ExecuteNormal(context);
    }

    public void OnFieldCardClickedAsTarget(Card targetCard)
    {
        if (spellCard == null || effect == null) return;
        if (targetCard == null) return;

        SpellContext context = BuildContext(targetCard);

        if (!effect.CanActivate(context))
        {
            Debug.LogWarning($"[SpellController] Invalid target: {targetCard.GetCardData().GetCardName()}");
            return;
        }

        effect.Execute(context);
        SendToGraveyard(spellCard);

        spellCard = null;
        effect = null;
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
    }

    private void ActiveContinuous(IContinuousSpellEffect continuousEffect, SpellContext context)
    {
        int slotIndex = spellZone.FindEmptySlot();
        Transform targetSlot = spellZone.GetSlotTransform(slotIndex);

        if (targetSlot == null)
        {
            Debug.LogWarning("[SpellController] Target transform is null!");
            spellCard.SetState(CardState.InHand);
            return;
        }

        GamePhaseManager.Instance.SetPhase(GamePhase.Setting);

        if (handLayout != null)
        {
            handLayout.RemoveCard(spellCard.transform);
        }

        Card cardToPlace = spellCard;

        CardAnimator.AnimateToSlot(
            spellCard,
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

        spellCard = null;
        effect = null;
    }

    private void ExecuteNormal(SpellContext context)
    {
        if (effect.SendToGraveyardFirst)
            SendToGraveyard(spellCard);

        effect.Execute(context);

        if (!effect.SendToGraveyardFirst)
            SendToGraveyard(spellCard);

        spellCard = null;
        effect = null;
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
    }

    public void CancelTargeting()
    {
        if (spellCard == null || effect == null) return;

        Debug.Log("[SpellController] Targeting cancelled");
        spellCard = null;
        effect = null;
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
        CardActionMenu.Instance?.ShowMenu(CardSelectionManager.Instance?.CurrentHandCard);
    }

    private void EnterTargetingMode()
    {
        //CardActionMenu.Instance?.HideMenu();
        CardSelectionManager.Instance?.DeselectAll();
        GamePhaseManager.Instance.SetPhase(GamePhase.SpellTargeting);
    }

    private void SendToGraveyard(Card spellCard)
    {
        handLayout.RemoveCard(spellCard.transform);
        spellCard.SetState(CardState.InGraveyard);
        spellCard.transform.position = graveyardZone.position;
        spellCard.gameObject.SetActive(false);

        CardSelectionManager.Instance?.DeselectAll();
        CardActionMenu.Instance?.HideMenu();
    }

    private SpellContext BuildContext(Card target)
    {
        return new SpellContext
        {
            SpellCard = spellCard,
            TargetMonster = target,
            PlayerMonsterZone = playerMonsterZone,
            DeckManager = deckManager,
            GraveyardZone = graveyardZone
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsWaitingForTarget)
        {
            CancelTargeting();
        }
    }
}
