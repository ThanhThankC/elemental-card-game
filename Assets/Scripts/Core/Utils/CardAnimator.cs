using DG.Tweening;
using UnityEngine;

public static class CardAnimator
{
    public static void AnimateToSlot(Card card, Transform targetSlot, float duration, System.Action onComplete = null, System.Action onKill = null)
    {
        if (card == null || targetSlot == null) return;

        Vector3 scale = CardVisualConfig.GetRestingScale(CardState.OnField);

        Sequence seq = DOTween.Sequence();
        seq.Append(card.transform.DOMove(targetSlot.position, duration).SetEase(Ease.OutCubic));
        seq.Join(card.transform.DORotateQuaternion(Quaternion.identity, duration));
        seq.Join(card.transform.DOScale(scale, duration));
        seq.OnComplete(() => onComplete?.Invoke());
        seq.OnKill(() => onKill?.Invoke());
    }

    public static void AnimateToGraveyard(Transform card, Transform graveyard, float duration = 0.5f, System.Action onComplete = null)
    {
        if (card == null || graveyard == null) return;

        Vector3 scale = CardVisualConfig.GetRestingScale(CardState.InGraveyard);

        Sequence seq = DOTween.Sequence();
        seq.Append(card.DOMove(graveyard.position, duration).SetEase(Ease.OutCubic));
        seq.Join(card.DOLocalRotateQuaternion(Quaternion.identity, duration));
        seq.Join(card.DOScale(scale, duration));
        seq.OnComplete(() => { card.gameObject.SetActive(false); onComplete?.Invoke(); });
    }
}
