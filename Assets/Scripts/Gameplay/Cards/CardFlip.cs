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

    public void Flip(float targetXScale)
    {
        if (isFlipping) return;
        isFlipping = true;

        rectTransform.DOScaleX(0f, flipDuration / 2f)
            .SetEase(flipEase)
            .OnComplete(() =>
            {
                isFaceUp = !isFaceUp;
                cardFront.SetActive(isFaceUp);
                cardBack.SetActive(!isFaceUp);

                rectTransform.DOScaleX(targetXScale, flipDuration / 2f)
                    .SetEase(flipEase)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                    });
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