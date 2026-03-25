using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    [SerializeField] private MonsterZone playerMonsterZone;
    [SerializeField] private SpellZone spellZone;
    [SerializeField] private HandLayoutManager handLayout;
    [SerializeField] private Transform graveyardZone;
    [SerializeField] private DeckManager deckManager;

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
    //private IContinuousSpellEffect continuousEffect;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsWaitingForTarget)
        {
            CancelTargeting();
        }
    }

    public void RequestSet(Card spellCard)
    {
        if (spellCard == null) return;

        CardData data = spellCard.GetCardData();
        //this.spellCard = spellCard;
        IContinuousSpellEffect continuousEffect = SpellEffectRegistry.BuildContinous(data);

        if (continuousEffect == null)
        {
            Debug.LogWarning($"[SpellController] No continuos effect found for {data.GetCardName()}");
            return;
        }

        SpellContext context = new SpellContext
        {
            SpellCard = spellCard,
            SpellZone = spellZone,
            PlayerMonsterZone = playerMonsterZone,
            DeckManager = deckManager,
            GraveyardZone = graveyardZone
        };
        if (!continuousEffect.CanActivate(context)) return;

        continuousEffect.OnActivate(context);

        //TODO spellCard to SpellZone
        if (SetController.Instance != null)
        {
            SetController.Instance.ExecuteSet(spellCard);
        }
    }

    public void RequestActivate(Card spellCard)
    {
        if (spellCard == null) return;

        CardData data = spellCard.GetCardData();
        this.spellCard = spellCard;
        effect = SpellEffectRegistry.BuildNormal(data);

        if (effect == null)
        {
            Debug.LogWarning($"[SpellController] No effect found for {data.GetCardName()}");
            return;
        }

        //TODO refactor: In future spellCard required both: NonTarget + Target ??;
        SpellContext context = new SpellContext
        {
            SpellCard = spellCard,
            PlayerMonsterZone = playerMonsterZone,
            DeckManager = deckManager,
        };
        if (!effect.CanActivate(context)) return;

        if (effect.NeedsTarget)
        {
            EnterTargetingMode();
            return;
        }

        if (effect.SendToGraveyardFirst)
            SendToGraveyard(spellCard);

        effect.Execute(context);

        if (!effect.SendToGraveyardFirst)
            SendToGraveyard(spellCard);
    }

    public void OnFieldCardClickedAsTarget(Card targetCard)
    {
        if (spellCard == null || effect == null) return;
        if (targetCard == null) return;

        SpellContext context = new SpellContext
        {
            SpellCard = spellCard,
            TargetMonster = targetCard,
            PlayerMonsterZone = playerMonsterZone,
            DeckManager = deckManager,
            GraveyardZone = graveyardZone
        };

        if (!effect.CanActivate(context))
        {
            Debug.LogWarning($"[SpellController] Invalid target: {targetCard.GetCardData().GetCardName()}");
        }

        effect.Execute(context);
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
        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
    }
}
