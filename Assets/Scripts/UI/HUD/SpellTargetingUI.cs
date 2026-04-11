using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpellTargetingUI : MonoBehaviour
{
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;

    private GamePhaseManager gamePhaseManager;

    private void Start()
    {
        gamePhaseManager = GamePhaseManager.Instance;
        gamePhaseManager.OnPhaseChanged += OnPhaseChanged;
        hintPanel.SetActive(false);
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
        bool isTargeting = newPhase == GamePhase.Targeting;
        hintPanel.SetActive(isTargeting);
    }
}
