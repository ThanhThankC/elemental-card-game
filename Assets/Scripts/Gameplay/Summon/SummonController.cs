using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Dang luc summon thi khoa, khong cho click, drag

/// <summary>
/// Controls monster summoning with sacrifice mechanics and animations.
/// Handles direct summon (1-4 stars) and tribute summon (5+ stars).
/// </summary>
public class SummonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MonsterZone playerMonsterZone;
    [SerializeField] private HandLayoutManager handLayout;
    [SerializeField] private Transform graveyardZone;

    [Header("Animation")]
    [SerializeField] private float summonDuration = 0.4f;
    [SerializeField] private float sacrificeDuration = 0.25f;
    [SerializeField] private float sacrificeDelay = 0.1f;

    public event Action<Card> OnMonsterSummoned;

    private static SummonController instance;
    public static SummonController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SummonController>();
            }
            return instance;
        }
    }

    private bool isSummoning = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (handLayout == null)
        {
            handLayout = FindObjectOfType<HandLayoutManager>();

            if (handLayout == null)
                Debug.LogWarning("[SummonController] HandLayout not found !");
        }

        if (playerMonsterZone == null)
            Debug.LogWarning("[SummonController] playerMonsterZone not assinged!");
    }

    /// <summary>
    /// Request direct summon (no sacrifice required).
    /// For monsters with 1-4 stars.
    /// </summary>
    public void RequestSummonDirect(Card monster)
    {
        if (!ValidateSummonRequest(monster)) return;

        ExecuteSummon(monster, null);
    }

    /// <summary>
    /// Execute summon with optional sacrifices.
    /// Handles both direct summon and tribute summon.
    /// </summary>
    public void ExecuteSummon(Card monster, IReadOnlyList<Card> sacrifices)
    {
        CardActionMenu.Instance?.HideMenu();

        int targetSlot = DetermineTargetSlot(sacrifices);

        if (targetSlot == -1)
        { Debug.LogWarning("[SummonController] No space left!"); return; }

        GamePhaseManager.Instance.SetPhase(GamePhase.Summoning);

        Sequence summonSeq = DOTween.Sequence();

        if (sacrifices != null && sacrifices.Count > 0)
        {
            summonSeq.AppendCallback(() => AnimateSacrifices(sacrifices));
            summonSeq.AppendInterval((sacrificeDelay * sacrifices.Count));
        }

        summonSeq.AppendCallback(() =>
        {
            isSummoning = true;
            monster.SetState(CardState.Playing);
            AnimateSummon(monster, targetSlot);
        });
    }

    /// <summary>
    /// Determine which slot to summon to.
    /// If sacrifices exist, use first sacrifice's slot.
    /// Otherwise, find empty slot.
    /// </summary>
    private int DetermineTargetSlot(IReadOnlyList<Card> sacrifices)
    {
        int targetSlot = -1;

        if (sacrifices != null && sacrifices.Count > 0)
        {
            targetSlot = sacrifices
                         .Select(s => playerMonsterZone.GetSlotIndex(s))
                         .Where(index => index >= 0)
                         .DefaultIfEmpty(-1)
                         .Min();

            if (targetSlot == -1)
            {
                Debug.LogWarning($"[SummonController] Sacrifice card not found on field!");
                targetSlot = playerMonsterZone.FindEmptySlot();
            }
            else
                Debug.Log($"[SummonController] Will summon to slot {targetSlot} on field");
        }
        else
        {
            targetSlot = playerMonsterZone.FindEmptySlot();
        }

        return targetSlot;
    }

    /// <summary>
    /// Animate sacrifice cards to graveyard.
    /// Removes cards from field before animating.
    /// </summary>
    private void AnimateSacrifices(IReadOnlyList<Card> sacrifices)
    {
        foreach (Card sacrifice in sacrifices)
        {
            bool removed = playerMonsterZone.RemoveCard(sacrifice);

            if (!removed)
            {
                Debug.LogWarning($"[SummonController] Failed to remove sacrifice: {sacrifice.GetCardData()}");
                continue;
            }

            sacrifice.SetState(CardState.Discarding);
            CardSelectionManager.Instance?.NotifyCardSentToGraveyard(sacrifice);

            CardAnimator.AnimateToGraveyard(
                sacrifice.transform,
                graveyardZone,
                duration: sacrificeDuration,
                onComplete: () => sacrifice.SetState(CardState.InGraveyard)
            );
        }
    }

    /// <summary>
    /// Animate monster card from hand to field.
    /// </summary>
    private void AnimateSummon(Card monster, int slotIndex)
    {
        Transform targetSlot = playerMonsterZone.GetSlotTransform(slotIndex);

        if (targetSlot == null)
        {
            Debug.LogWarning("[SummonController] Target transform is null!");
            monster.SetState(CardState.InHand);
            isSummoning = false;
            GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
            return;
        }

        if (handLayout != null)
        {
            handLayout.RemoveCard(monster.transform);
        }

        bool isCompleted = false;

        CardAnimator.AnimateToField(
            monster,
            targetSlot,
            summonDuration,
            onComplete: () =>
            {
                FinalizeSummon(monster, slotIndex);
                GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
                isCompleted = true;
            },
            onKill: () =>
            {
                if (!isCompleted)
                {
                    OnAnimationCancelled(monster);
                    GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
                }
            }
         );
    }

    /// <summary>
    /// Finalize summon - place card on field and update state.
    /// </summary>
    private void FinalizeSummon(Card monster, int slotIndex)
    {
        bool success = playerMonsterZone.SummonMonster(monster, slotIndex);

        if (success)
        {
            monster.SetState(CardState.OnField);
            OnMonsterSummoned?.Invoke(monster);
            if (CardSelectionManager.Instance?.CurrentHandCard == monster)
            {
                CardSelectionManager.Instance?.ClearCurrentHandCard();
            }
            Debug.Log($"[SummonController] Summoned {monster.GetCardData()}");
        }
        else
        {
            OnSummonFailed(monster);
        }

        isSummoning = false;
    }

    /// <summary>
    /// Handle summon failure - return card to hand.
    /// </summary>
    private void OnSummonFailed(Card monster)
    {
        Debug.LogError($"[SummonController] Failed to summon {monster.GetCardData()}!");

        monster.SetState(CardState.InHand);

        if (handLayout != null)
        {
            handLayout.AddCard(monster.transform);
        }

        isSummoning = false;
    }

    /// <summary>
    /// Handle animation cancellation.
    /// </summary>
    private void OnAnimationCancelled(Card monster)
    {
        Debug.LogWarning("[SummonController] Animation cancelled!");

        if (monster != null && monster.GetState() == CardState.Playing)
        {
            monster.SetState(CardState.InHand);
        }
        isSummoning = false;
    }

    private bool ValidateSummonRequest(Card monster)
    {
        if (monster == null)
        {
            Debug.LogWarning("[SummonController] Card is null!");
            return false;
        }

        if (isSummoning)
        {
            monster.GetComponent<CardClickHandler>()?.ForceDeselect();
            CardActionMenu.Instance?.HideMenu();
            Debug.LogWarning("[SummonController] Another card is being summoned!");
            return false;
        }

        if (monster.GetState() != CardState.InHand)
        {
            Debug.LogWarning("[SummonController] Card must be in hand to summon!");
            return false;
        }

        if (!playerMonsterZone.CanPlaceCard(monster))
        {
            return false;
        }

        return true;
    }

    public bool IsSummoning => isSummoning;
    public MonsterZone GetMonsterZone() => playerMonsterZone;
    public HandLayoutManager GetHandLayout() => handLayout;
}
