using UnityEngine;
using DG.Tweening;

public class CardFlip : MonoBehaviour
{
    [Header("References")]
    public GameObject cardFront;
    public GameObject cardBack;

    [Header("Settings")]
    public float flipDuration = 0.6f;
    public Ease flipEase = Ease.InOutSine;

    private bool isFaceUp = true;
    private bool isFlipping = false;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cardFront.SetActive(true);
        cardBack.SetActive(false);
    }

    /// <summary>
    /// Flip the card to face up if it's currently face down.
    /// </summary>
    public void FlipToFaceUp(float targetXScale, float duration = -1)
    {
        if (isFaceUp) return;
        Flip(targetXScale, duration);
    }

    /// <summary>
    /// Flip the card to face down if it's currently face up.
    /// </summary>
    public void FlipToFaceDown(float targetXScale, float duration = -1)
    {
        if (!isFaceUp) return;
        Flip(targetXScale, duration);
    }

    private void Flip(float targetXScale, float duration = -1)
    {
        if (isFlipping) return;
        isFlipping = true;

        float actualDuration = duration <= 0 ? this.flipDuration : duration;

        transform.DOScaleX(0f, actualDuration / 2f)
            .SetEase(flipEase)
            .OnComplete(() =>
            {
                isFaceUp = !isFaceUp;
                cardFront.SetActive(isFaceUp);
                cardBack.SetActive(!isFaceUp);

                transform.DOScaleX(targetXScale, actualDuration / 2f)
                    .SetEase(flipEase)
                    .OnComplete(() => isFlipping = false); 
            });
    }

    public void SetFaceDown()
    {
        isFaceUp = false;
        cardFront.SetActive(false);
        cardBack.SetActive(true);
        rectTransform.localScale = Vector3.one;
    }

    public void SetFaceUp()
    {
        isFaceUp = true;
        cardFront.SetActive(true);
        cardBack.SetActive(false);
        rectTransform.localScale = Vector3.one;
    }
}