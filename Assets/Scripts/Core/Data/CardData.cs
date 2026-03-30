using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string cardID;
    [SerializeField] private string cardNameVN;
    [SerializeField] private string cardNameEN;
    [SerializeField] private CardType cardType;
    [SerializeField] private Sprite cardArtwork;

    [Header("Monster Stats (Monster only)")]
    [SerializeField] private int stars = 1;
    [SerializeField] private int baseATK = 0;
    [SerializeField] private int baseDEF = 0;

    [Header("Elements")]
    [SerializeField] private List<ElementType> elements = new List<ElementType>();

    [Header("Buff System")]
    [Range(0, 10)]
    [SerializeField] private int rubyCount = 0;
    [Range(0, 10)]
    [SerializeField] private int lotusCount = 0;

    [Header("Spell Info")]
    [SerializeField] private SpellType spellType = SpellType.Normal;
    [SerializeField] private SpellEffectID spellEffectID = SpellEffectID.None;
    [SerializeField] private int spellBuffAmount = 0;

    [Header("Trap Info")]
    [SerializeField] private TrapEffectID trapEffectID = TrapEffectID.None;

    [Header("Descriptions")]
    [TextArea(3, 6)]
    [SerializeField] private string normalEffectDescVN;
    [TextArea(3, 6)]
    [SerializeField] private string normalEffectDescEN;
    [TextArea(3, 6)]
    [SerializeField] private string specialEffectDescVN;
    [TextArea(3, 6)]
    [SerializeField] private string specialEffectDescEN;

    public string CardID => cardID;
    public CardType Type => cardType;
    public Sprite Artwork => cardArtwork;
    public int Stars => stars;
    public int BaseATK => baseATK;
    public int BaseDEF => baseDEF;
    public List<ElementType> Elements => elements;
    public int RubyCount => rubyCount;
    public int LotusCount => lotusCount;
    public SpellType SpellType => spellType;
    public SpellEffectID SpellEffectID => spellEffectID;
    public int SpellBuffAmount => spellBuffAmount;
    public TrapEffectID TrapEffectID => trapEffectID;
    public string GetCardName(bool isVietnamese = true)
    {
        return isVietnamese ? cardNameVN : cardNameEN;
    }

    public string GetNormalEffectDescription(bool isVietNamese = true)
    {
        return isVietNamese ? normalEffectDescVN : normalEffectDescEN;
    }

    public string GetSpecialEffectDescription(bool isVietNamese = true)
    {
        return isVietNamese ? specialEffectDescVN : specialEffectDescEN;
    }

    public bool IsMonster()
    {
        return cardType == CardType.Monster;
    }

    public bool HasSpecialBuff() => !string.IsNullOrEmpty(specialEffectDescVN);

    /// <summary>
    /// [TKC] Calculate total ATK/DEF buffs (Lotus + Ruby).
    /// </summary>
    public int GetTotalBuffValue()
    {
        return (rubyCount + lotusCount) * 100;
    }

    public string GetTier()
    {
        if (!IsMonster()) return "N/A";

        if (stars <= 4) return "Low";
        if (stars <= 7) return "Mid";
        return "High";
    }
}
