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

    private BaseCardController activeController;
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

    public void Register(BaseCardController controller, TargetType targetType)
    {
        if (controller != null && activeController != controller)
        {
            activeController = controller;
        }
        currentTargetType = targetType;
    }

    public void UnRegister()
    {
        activeController = null;
    }

    public void OnTargetCardClicked(Card targetCard)
    {
        if (activeController == null) return;
        if (!IsValidTarget(targetCard)) return;
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

    private void Update()
    {
        //TODO
        //if (Input.GetKeyDown(KeyCode.Escape) && GamePhaseManager.Instance.IsInSpellTargeting())
        //    activeController?.CancelTargeting();
    }
}
