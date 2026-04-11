using UnityEngine;
using System;

public enum GamePhase
{
    Idle,           
    Drawing,
    DiscardSelection,
    Summoning,
    Setting,
    Targeting,
    Battle,
    End
}

public class GamePhaseManager : MonoBehaviour
{
    public event Action<GamePhase> OnPhaseChanged;

    private static GamePhaseManager instance;
    public static GamePhaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GamePhaseManager");
                instance = go.AddComponent<GamePhaseManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private GamePhase currentPhase = GamePhase.Idle;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SetPhase(GamePhase newState)
    {
        if (currentPhase == newState) return;

        currentPhase = newState;

        OnPhaseChanged?.Invoke(newState);
    }

    public bool CanInteractWithCards() => currentPhase == GamePhase.Idle || currentPhase == GamePhase.DiscardSelection;
    public bool CanDragCards() => currentPhase == GamePhase.Idle;
    public bool IsInDiscardPhase() => currentPhase == GamePhase.DiscardSelection;
    public bool IsInTargeting() => currentPhase == GamePhase.Targeting; 
    public GamePhase CurrentState => currentPhase;
}
