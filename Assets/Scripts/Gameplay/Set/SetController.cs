using UnityEngine;

public class SetController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpellZone spellZone;
    [SerializeField] private HandLayoutManager handLayout;

    [Header("Animation")]
    [SerializeField] private float setDuration = 0.4f;

    private static SetController instance;
    public static SetController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SetController>();
            }
            return instance;
        }
    }

    private bool isSetting = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void ExecuteSet(Card spellCard)
    {
        if (!ValidateSettingSpellCard(spellCard)) return;

        int targetSlot = spellZone.FindEmptySlot();

        if (targetSlot == -1)
            { Debug.LogWarning("[SetController] No space left!"); return; }

        AnimateSet(spellCard, targetSlot);
    }


    /// <summary>
    /// Animate spell card from hand to field.
    /// </summary>
    private void AnimateSet(Card spellCard, int slotIndex)
    {
        Transform targetSlot = spellZone.GetSlotTransform(slotIndex);

        if (targetSlot == null)
        {
            Debug.LogWarning("[SetController] Target transform is null!");
            spellCard.SetState(CardState.InHand);
            isSetting = false;
            return;
        }

        if (handLayout != null)
        {
            handLayout.RemoveCard(spellCard.transform);
        }

        bool isCompleted = false;

        CardAnimator.AnimateToSlot(
            spellCard,
            targetSlot,
            setDuration,
            onComplete: () =>
            {
                FinalizeSet(spellCard, slotIndex);
                GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
                isCompleted = true;
            },
            onKill: () =>
            {
                if (!isCompleted) OnAnimationCancelled(spellCard);
            }
         );
    }

    /// <summary>
    /// Finalize set - place card on field and update state.
    /// </summary>
    private void FinalizeSet(Card spellCard, int slotIndex)
    {
        bool success = spellZone.SetSpellCard(spellCard, slotIndex);

        if (success)
        {
            spellCard.SetState(CardState.OnField);
            if (CardSelectionManager.Instance?.CurrentHandCard == spellCard)
            {
                CardSelectionManager.Instance?.ClearCurrentHandCard();
            }
            Debug.Log($"[SetController] Setted {spellCard.GetCardData()}");
        }
        else
        {
            OnSetFailed(spellCard);
        }

        isSetting = false;
    }

    /// <summary>
    /// Handle set failure - return card to hand.
    /// </summary>
    private void OnSetFailed(Card spellCard)
    {
        Debug.LogError($"[SetController] Failed to set {spellCard.GetCardData()}!");

        spellCard.SetState(CardState.InHand);

        if (handLayout != null)
        {
            handLayout.AddCard(spellCard.transform);
        }

        isSetting = false;
    }

    /// <summary>
    /// Handle animation cancellation.
    /// </summary>
    private void OnAnimationCancelled(Card spellCard)
    {
        Debug.LogWarning("[SetController] Animation cancelled!");

        if (spellCard != null && spellCard.GetState() == CardState.Playing)
        {
            spellCard.SetState(CardState.InHand);
        }
        isSetting = false;
    }

    private bool ValidateSettingSpellCard(Card monster)
    {
        if (monster == null)
        {
            Debug.LogWarning("[SetController] Card is null!");
            return false;
        }

        if (isSetting)
        {
            monster.GetComponent<CardClickHandler>()?.ForceDeselect();
            CardActionMenu.Instance?.HideMenu();
            Debug.LogWarning("[SetController] Another card is being setted!");
            return false;
        }

        if (monster.GetState() != CardState.InHand)
        {
            Debug.LogWarning("[SetController] Card must be in hand to set!");
            return false;
        }

        if (!spellZone.CanPlaceCard(monster))
        {
            return false;
        }

        return true;
    }
}
