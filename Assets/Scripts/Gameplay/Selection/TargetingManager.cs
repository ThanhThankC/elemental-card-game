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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Register(ITargetableController controller)
    {
        if (controller != null && activeController != controller)
        {
            activeController = controller;
        }
    }

    public void UnRegister()
    {
        activeController = null;
    }

    public void OnFeildCardClikedAsTarget(Card card)
    {
        activeController.OnFieldCardClickedAsTarget(card);
    }
}
