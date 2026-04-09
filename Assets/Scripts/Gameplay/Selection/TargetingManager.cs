using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    private static TargetingManager instance;
    public static TargetingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TargetingManager>();
            }
            return instance;
        }
    }

    private ITargetableController activeController;
    private TargetType currentTargetType;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Register(ITargetableController controller, TargetType targetType)
    {
        if (controller != null && activeController != controller)
        {
            activeController = controller;
            currentTargetType = targetType;
        }
    }

    public void UnRegister()
    {
        activeController = null;
    }

    public void OnTargetCardClicked(Card targetCard)
    {
        activeController.OnTargetSelected(targetCard);
    }

    public bool IsValidTarget(Card targetCard)
    {
        switch (currentTargetType)
        {
            case TargetType.MonsterOnField:
                return targetCard.GetCardData().Type == CardType.Monster && targetCard.GetState() == CardState.OnField;
            case TargetType.SpellOnField:
                return targetCard.GetCardData().Type == CardType.Spell && targetCard.GetState() == CardState.OnField;
            case TargetType.TrapOnField:
                return targetCard.GetCardData().Type == CardType.Trap && targetCard.GetState() == CardState.OnField;
            case TargetType.CardInHand:
                return targetCard.GetState() == CardState.InHand;
            case TargetType.AnyCardOnField:
                return targetCard.GetState() == CardState.OnField;
            case TargetType.AnyCard:
                return true;
            default:
                return false;
        }
    }
}
