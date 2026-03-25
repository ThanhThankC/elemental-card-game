using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconDatabase", menuName = "Elemental Card Game/Icon Database")]
public class IconDatabase : ScriptableObject
{
    [Header("Element Icons")]
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite plantIcon;
    [SerializeField] private Sprite earthIcon;
    [SerializeField] private Sprite windIcon;
    [SerializeField] private Sprite poisonIcon;
    [SerializeField] private Sprite iceIcon;
    [SerializeField] private Sprite metalIcon;
    [SerializeField] private Sprite thunderIcon;
    [SerializeField] private Sprite lightIcon;
    [SerializeField] private Sprite darkIcon;
    [SerializeField] private Sprite soundIcon;

    [Header("Buff Icons")]
    [SerializeField] private Sprite rubyIcon;
    [SerializeField] private Sprite lotusIcon;

    private static IconDatabase instance;

    public static IconDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<IconDatabase>("IconDatabase");
                if (instance == null)
                {
                    Debug.LogError("[IconDatabase] IconDatabase not found in Resources!");
                }
            }
            return instance;
        }
    }

    public Sprite GetElementIcon(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return fireIcon;
            case ElementType.Water: return waterIcon;
            case ElementType.Plant: return plantIcon;
            case ElementType.Earth: return earthIcon;
            case ElementType.Wind: return windIcon;
            case ElementType.Ice: return iceIcon;
            case ElementType.Thunder: return thunderIcon;
            case ElementType.Poison: return poisonIcon;
            case ElementType.Metal: return metalIcon;
            case ElementType.Sound: return soundIcon;
            case ElementType.Light: return lightIcon;
            case ElementType.Dark: return darkIcon;
            default: return null;
        }
    }

    public Sprite GetBuffIcon(BuffType buff)
    {
        switch (buff)
        {
            case BuffType.Lotus: return lotusIcon;
            case BuffType.Ruby: return rubyIcon;
            default: return null;
        }
    }
}
