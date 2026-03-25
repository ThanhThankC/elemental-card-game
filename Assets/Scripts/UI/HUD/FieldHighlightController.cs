using UnityEngine;

public class FieldHighlightController : MonoBehaviour
{
    [SerializeField] private MonsterZone playerMonsterZone;

    private GamePhaseManager gamePhaseManager;

    private void Awake()
    {
        if (playerMonsterZone == null)
        {
            playerMonsterZone = FindObjectOfType<MonsterZone>();
            if (playerMonsterZone == null)
            {
                Debug.LogWarning($"[FieldHighlightController] MonsterZone not found!");
            }
        }
    }

    private void Start()
    {
        gamePhaseManager = GamePhaseManager.Instance;
        gamePhaseManager.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDestroy()
    {
        if (gamePhaseManager != null)
        {
            gamePhaseManager.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        bool active = newPhase == GamePhase.SpellTargeting;

        var cards = playerMonsterZone.GetAllCards();
        foreach (Card card in cards)
        {
            if (card != null)
            {
                CardHighlighter highlighter = card.GetComponent<CardHighlighter>();
                if (highlighter != null)
                {
                    highlighter.SetHighlight(active);
                }
            }
        }
    }
}
