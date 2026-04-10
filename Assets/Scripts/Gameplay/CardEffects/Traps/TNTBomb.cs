using UnityEngine;

public class TNTBomb : ITrapEffect
{
    public bool SendToGraveyardFirst => false;

    public TargetType TargetType => TargetType.MonsterOnField;

    public bool CanActivate(CardEffectContext context)
    {
        return context.PlayerMonsterZone.GetAllCards().Count > 0;
    }

    public void Execute(CardEffectContext context)
    {
        Card monster = context.TargetCard;
        if (monster == null) return;

        bool removed = context.PlayerMonsterZone.RemoveCard(monster);
        if (!removed)
        {
            Debug.LogWarning($"[TNTBomb] Failed to remove: {monster.GetCardData().name}");
            return;
        }

        monster.SetState(CardState.Discarding);
        CardSelectionManager.Instance?.NotifyCardSentToGraveyard(monster);
        CardAnimator.AnimateToGraveyard(
            monster.transform,
            context.GraveyardZone,
            onComplete: () => monster.SetState(CardState.InGraveyard)
        );
    }
}
