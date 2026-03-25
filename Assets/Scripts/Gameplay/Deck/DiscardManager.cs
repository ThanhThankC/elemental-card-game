using System;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    private static DiscardManager instance;
    public static DiscardManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DiscardManager");
                instance = go.AddComponent<DiscardManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public event Action<int, int> OnDiscardRequirementChanged;

    private int selectedDiscardCount;
    private int requiredDiscardCount;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void UpdateAvailableSpace(int spaceSlot)
    {
        requiredDiscardCount = GameRules.MAXDRAW_PER_TURN - spaceSlot;
        if (requiredDiscardCount < 0) selectedDiscardCount = 0;

        NotifyRequirementChanged();
    }

    public void UpdateSelectedDiscardCount(int count)
    {
        selectedDiscardCount = count;
        NotifyRequirementChanged();
    }

    public void ResetState()
    {
        selectedDiscardCount = 0;
        requiredDiscardCount = 0;
        NotifyRequirementChanged();
    }

    private void NotifyRequirementChanged()
    {
        OnDiscardRequirementChanged?.Invoke(selectedDiscardCount, requiredDiscardCount);
    }

    public int SelectedDiscardCount => selectedDiscardCount;
    public int RequiredDiscardCount => requiredDiscardCount;
}
