using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public static class CardAnimator
{
    public static void AnimateFlipFaceUp(Card card, float flipDuration = -1, System.Action onComplete = null)
    {
        if (card == null) return;

        CardFlip flip = card.GetComponent<CardFlip>();
        if (flip == null) return;

        Vector3 scale = CardVisualConfig.GetSelectedScale(CardState.OnField);
        flip.FlipToFaceUp(scale.x, flipDuration, onComplete);
    }

    public static void AnimateToField(Card card, Transform targetSlot, float moveDuration, float flipDuration = -1, System.Action onComplete = null, System.Action onKill = null)
    {
        if (card == null || targetSlot == null) return;

        Vector3 scale = CardVisualConfig.GetRestingScale(CardState.OnField);

        Sequence seq = DOTween.Sequence();
        seq.Append(card.transform.DOMove(targetSlot.position, moveDuration).SetEase(Ease.OutCubic));
        seq.Join(card.transform.DORotateQuaternion(Quaternion.identity, moveDuration));
        seq.Join(card.transform.DOScale(scale, moveDuration));
        seq.InsertCallback(moveDuration * 0.5f, () =>
        {
            if (flipDuration > 0)
                card.GetComponent<CardFlip>().FlipToFaceDown(scale.x, flipDuration);
        });
        seq.OnComplete(() => onComplete?.Invoke());
        seq.OnKill(() => onKill?.Invoke());
    }

    public static void AnimateToHand(Card card, float moveDuration, float flipDuration = -1, System.Action onComplete = null)
    {
        if (card == null) return;
        Vector3 scale = CardVisualConfig.GetRestingScale(CardState.InHand);
        Sequence seq = DOTween.Sequence();
        seq.Append(card.transform.DOScale(scale, moveDuration));
        seq.InsertCallback(moveDuration * 0.5f, () =>
        {
            if (flipDuration > 0)
                card.GetComponent<CardFlip>().FlipToFaceUp(scale.x, flipDuration);
        });
        seq.OnComplete(() => onComplete?.Invoke());
    }

    public static void AnimateToGraveyard(Transform card, Transform graveyard, float duration = 0.5f, System.Action onComplete = null)
    {
        if (card == null || graveyard == null) return;

        Vector3 scale = CardVisualConfig.GetRestingScale(CardState.InGraveyard);

        Sequence seq = DOTween.Sequence();
        seq.Append(card.DOMove(graveyard.position, duration).SetEase(Ease.OutCubic));
        seq.Join(card.DORotateQuaternion(Quaternion.identity, duration));
        seq.Join(card.DOScale(scale, duration));
        seq.OnComplete(() => { card.gameObject.SetActive(false); onComplete?.Invoke(); });
    }
}
