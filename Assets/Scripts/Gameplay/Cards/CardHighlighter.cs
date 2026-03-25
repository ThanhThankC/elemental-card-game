using UnityEngine;
using UnityEngine.UI;

public class CardHighlighter : MonoBehaviour
{
    [SerializeField] private Outline outline;

    private void Awake()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }

        SetHighlight(false);
    }

    public void SetHighlight(bool active)
    {
        if (outline != null)
        {
            outline.enabled = active;
        }
    }
}
