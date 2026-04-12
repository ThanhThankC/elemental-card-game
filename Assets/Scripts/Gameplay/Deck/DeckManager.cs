using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages deck, hand, graveyard.
/// </summary>
public class DeckManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private Card cardPrefab;

    [Header("Zones")]
    [SerializeField] private HandLayoutManager handLayout;
    [SerializeField] private DiscardManager discardManager;
    [SerializeField] private Transform deckZone;
    [SerializeField] private Transform poolParent;

    [Header("Pool Settings")]
    [SerializeField] private int poolInitialSize = 15;
    [SerializeField] private int poolMaxSize = 50;

    [Header("Deck Configuration")]
    [SerializeField] private List<CardData> deckCards = new List<CardData>();

    [Header("Draw Settings")]
    [SerializeField] private float drawDuration = 0.5f;
    [SerializeField] private float drawDelay = 0.15f;

    [Header("Shuffle Animation")]
    [SerializeField] private GameObject deckCardBack;
    [SerializeField] private float shuffleDuration = 1f;
    [SerializeField] private ParticleSystem shuffleVFX;

    [Header("UI Settings")]
    [SerializeField] private Button resetButton;

    private ObjectPool<Card> cardPool;
    private List<CardData> deck = new List<CardData>();

    private void Awake()
    {
        InitializePool();

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetGame);

        discardManager = DiscardManager.Instance;

        if (handLayout != null)
        {
            handLayout.OnHandCountChanged += OnHandCountChanged;
        }
    }

    private void Start()
    {
        InitializeDeck();
        StartCoroutine(ShuffleAndDrawSequence());
    }

    private void InitializePool()
    {
        if (poolParent == null)
        {
            GameObject poolObj = new GameObject("CardPool");
            poolParent = poolObj.transform;
            //poolParent.SetParent(deckZone);
            poolParent.localScale = Vector3.one;
        }

        cardPool = new ObjectPool<Card>(cardPrefab, poolParent, poolInitialSize, poolMaxSize);
        Debug.Log($"[DeckManager] Card pool intialized");
    }

    private void InitializeDeck()
    {
        deck.Clear();

        foreach (CardData cardData in deckCards)
        {
            if (cardData != null)
            {
                deck.Add(cardData);
            }
        }

        Debug.Log($"[DeckManager] {deck.Count} cards created in deck");
    }

    private IEnumerator ShuffleAndDrawSequence()
    {
        yield return StartCoroutine(PlayShuffleAnimation());

        ShuffleDeck();

        yield return StartCoroutine(DrawCards(GameRules.STARTING_HAND_SIZE));
    }

    private IEnumerator PlayShuffleAnimation()
    {
        if (deckCardBack == null)
        {
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        Debug.Log("[DeckManager] Shuffling deck...");

        Sequence shuffleSeq = DOTween.Sequence();

        shuffleSeq.Append(deckCardBack.transform.DORotate(new Vector3(0, 0, 360), shuffleDuration, RotateMode.FastBeyond360)).SetEase(Ease.OutCubic);

        shuffleSeq.Join(deckCardBack.transform.DOShakePosition(shuffleDuration, strength: 10f, vibrato: 20));

        if (shuffleVFX != null)
        {
            shuffleVFX.Play();
        }

        yield return shuffleSeq.WaitForCompletion();
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            CardData temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        Debug.Log($"[DeckManager] Shuffled!");
    }

    private IEnumerator DrawCards(int count = 0)
    {
        int actualDrawCount = Mathf.Min(count, GetDrawableCount());

        GamePhaseManager.Instance.SetPhase(GamePhase.Drawing);

        for (int i = 0; i < actualDrawCount; i++)
        {
            DrawSingleCard();
            yield return new WaitForSeconds(drawDelay);
        }

        GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
    }

    private void DrawSingleCard()
    {
        if (deck.Count == 0 || !handLayout.HasEmptySlot()) return;

        CardSelectionManager.Instance?.DeselectAll();

        CardData cardData = deck[0];
        deck.RemoveAt(0);

        Card card = cardPool.Get(deckZone.position);

        CardClickHandler hoverEffect = card.GetComponent<CardClickHandler>();
        if (hoverEffect == null)
        {
            hoverEffect = card.gameObject.AddComponent<CardClickHandler>();
        }

        CardDragHandler dragHandler = card.GetComponent<CardDragHandler>();
        if (dragHandler == null)
        {
            dragHandler = card.gameObject.AddComponent<CardDragHandler>();
        }

        if (card != null)
        {
            card.Initialize(cardData);
            card.SetState(CardState.Drawing);
            card.transform.SetParent(deckZone);

        }
        AnimateCardDraw(card);
    }

    private void AnimateCardDraw(Card card)
    {
        card.transform.localScale = CardVisualConfig.GetRestingScale(CardState.InDeck);

        CardAnimator.AnimateToHand(
            card,
            drawDuration,
            onComplete: () =>
            {
                handLayout.AddCard(card.transform);
                GamePhaseManager.Instance.SetPhase(GamePhase.Idle);
            }
        );
    }

    private int GetDrawableCount()
    {
        int spaceInHand = GameRules.MAX_HAND_SIZE - handLayout.GetCardCount();
        int cardsInDeck = deck.Count;

        return Mathf.Min(spaceInHand, cardsInDeck);
    }

    private void OnHandCountChanged()
    {
        if (discardManager != null)
        {
            int spaceAvailable = GetDrawableCount();
            discardManager.UpdateAvailableSpace(spaceAvailable);
        }
    }

    private void OnDestroy()
    {
        if (handLayout != null)
        {
            handLayout.OnHandCountChanged -= OnHandCountChanged;
        }
    }

    private void Update()
    {
    #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDrawCard();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetGameSpeed(1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetGameSpeed(0.1f);
        }
    #endif
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnDrawCard()
    {
        StartCoroutine(DrawCards(2));
    }

    void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public int GetDeckCount() => deck.Count;
    public bool CanDrawCard() => GetDrawableCount() > 0;
}