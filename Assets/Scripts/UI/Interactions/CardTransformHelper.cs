using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages card transform states (position, rotation, sorting) across different zones.
/// Supports both HandLayout and FieldZone positioning.
/// </summary>
public class CardTransformHelper : MonoBehaviour, IPointerDownHandler
{
    private Vector3 cachedPosition;
    private Quaternion cachedRotation;
    private int cachedSortingOrder;

    private Canvas canvas;
    private Card card;
    private HandLayoutManager handLayout;
    private FieldZone currentFieldZone;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.overrideSorting = true;
    }

    private void Start()
    {
        card = GetComponent<Card>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GamePhaseManager.Instance.CanInteractWithCards()) return;
        CacheCurrentState();
        UpdateZoneContext();
    }

    /// <summary>
    /// Cache current transform state.
    /// </summary>
    public void CacheCurrentState()
    {
        cachedPosition = transform.localPosition;
        cachedRotation = transform.localRotation;
        cachedSortingOrder = canvas.sortingOrder;
    }

    /// <summary>
    /// Update the parent zone reference.
    /// </summary>
    public void UpdateZoneContext()
    {
        if (card == null) return;

        CardState state = card.GetState();

        switch (state)
        {
            case CardState.InHand:
                handLayout = GetComponentInParent<HandLayoutManager>();
                currentFieldZone = null;
                break;

            case CardState.OnField:
                handLayout = null;
                currentFieldZone = GetComponentInParent<FieldZone>();
                break;

            default:
                handLayout = null;
                currentFieldZone = null;
                break;
        }
    }

    /// <summary>
    /// Get resting position for current zone.
    /// </summary>
    public Vector3 GetRestingPosition()
    {
        if (handLayout != null)
        {
            return handLayout.GetTargetPosition(transform);
        }

        if (currentFieldZone != null)
        {
            return Vector3.zero;
        }

        return cachedPosition;
    }

    /// <summary>
    /// Get resting rotation for current zone.
    /// </summary>
    public Quaternion GetRestingRotation()
    {
        if (handLayout != null)
        {
            return handLayout.GetTargetRotation(transform);
        }

        return cachedRotation;
    }

    /// <summary>
    /// Get resting sorting order for current zone.
    /// </summary>
    public int GetRestingSortingOrder()
    {
        if (handLayout != null)
        {
            return handLayout.GetSortingOrder(transform);
        }
        if (currentFieldZone != null)
        {
            int slotIndex = currentFieldZone.GetSlotIndex(card);
            return slotIndex >= 0 ? slotIndex : cachedSortingOrder;
        }

        return cachedSortingOrder;
    }

    /// <summary>
    /// Get resting scale for current card state.
    /// </summary>
    public Vector3 GetRestingScale()
    {
        if (card == null) return Vector3.one;

        return CardVisualConfig.GetRestingScale(card.GetState());
    }

    public Canvas Canvas => canvas;
    public Vector3 CachedPosition => cachedPosition;
    public Quaternion CachedRotation => cachedRotation;
    public int CachedSortingOrder => cachedSortingOrder;
}