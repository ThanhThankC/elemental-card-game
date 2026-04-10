using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages active continuous spell effects, allowing them to listen for game events and apply their effects accordingly.
/// </summary>
public class ContinuousSpellManager : MonoBehaviour
{
    private static ContinuousSpellManager instance;
    public static ContinuousSpellManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ContinuousSpellManager>();
            }
            return instance;
        }
    }

    private Dictionary<Card, IContinuousSpellEffect> activeEffects = new Dictionary<Card, IContinuousSpellEffect>();
    private SummonController summonController;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        summonController = SummonController.Instance;
        if (summonController != null)
        {
            summonController.OnMonsterSummoned += OnMonsterSummoned;
        }
    }

    private void OnDestroy()
    {
        if (summonController != null)
        {
            summonController.OnMonsterSummoned -= OnMonsterSummoned;
        }
    }

    /// <summary>
    /// Register a continuous spell effect to be active and listen for game events.
    /// </summary>
    public void Register(Card spellCard, IContinuousSpellEffect effect)
    {
        if (!activeEffects.ContainsKey(spellCard))
        {
            activeEffects.Add(spellCard, effect);
        }
    }

    /// <summary>
    /// Deactivate a continuous spell effect, stop listening for game events, and remove it from active effects.
    /// </summary>
    public void Deactive(Card spellCard)
    {
        if (activeEffects.TryGetValue(spellCard, out IContinuousSpellEffect effect))
        {
            effect.OnDeactivate();
            activeEffects.Remove(spellCard);
        }
    }

    private void OnMonsterSummoned(Card card)
    {
        foreach (var e in activeEffects)
        {
            e.Value.OnMonsterSummoned(card);
        }
    }

    public bool HasActiveEffect(Card spellCard) => activeEffects.ContainsKey(spellCard);
    public int ActiveCount => activeEffects.Count;
}
