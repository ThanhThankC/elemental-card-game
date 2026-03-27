using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
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

    public void RequestSet(Card trapCard)
    {
        if (trapCard == null) return;

        CardSelectionManager.Instance?.DeselectAll();
        CardActionMenu.Instance?.HideMenu();

        if (trapCard.GetCardData().Type != CardType.Trap)
        {
            Debug.LogWarning($"[TrapController] Only trap cards allowed");
            return;
        }

        if (trapCard.GetState() == CardState.InHand)
        {
            SetTrapCard(trapCard);
        }
        else if (trapCard.GetState() == CardState.OnField)
        {
            ActivateTrapCard();
        }
    }

    public void RequestRecall(Card trapCard)
    {
        if (trapCard == null) return;


        CardSelectionManager.Instance?.DeselectAll();
        CardActionMenu.Instance?.HideMenu();

        if (trapCard.GetCardData().Type != CardType.Trap)
        {
            Debug.LogWarning($"[TrapController] Only trap cards allowed");
            return;
        }

        //if (handLayout.)
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

    private void SetTrapCard(Card trapCard)
    {
        int slotIndex = trapZone.FindEmptySlot();
        Transform targetSlot = trapZone.GetSlotTransform(slotIndex);

        if (targetSlot == null)
        {
            Debug.LogWarning("[TrapController] Target transform is null!");
            trapCard.SetState(CardState.InHand);
            return;
        }

        GamePhaseManager.Instance.SetPhase(GamePhase.Setting);
        CardSelectionManager.Instance?.DeselectAll();

        if (handLayout != null)
        {
            handLayout.RemoveCard(trapCard.transform);
        }

        Card cardToPlace = trapCard;

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
    }

    private void ActivateTrapCard()
    {
        Debug.Log("Hello");
    }
}
