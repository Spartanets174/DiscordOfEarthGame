using HighlightPlus;
using UnityEngine;
public class ChildOutlineInteractableObject : ChildInteractableObject
{
    private HighlightEffect highlightEffect;

    // Start is called before the first frame update
   protected override void Awake()
    {
        base.Awake();
        highlightEffect = GetComponentInChildren<HighlightEffect>();

        OnHoverEnter += EnableOutline;
        OnHoverExit += DisableOutline;
        OnEnableChanged += x => { if (!x) DisableOutline(null); };
    }

    private void OnDestroy()
    {
        OnHoverEnter -= EnableOutline;
        OnHoverExit -= DisableOutline;
    }

    protected void SetOutlineState(bool state)
    {
        highlightEffect.highlighted = state;
    }

    protected void EnableOutline(GameObject gameObject)
    {
        SetOutlineState(true);
    }
    protected void DisableOutline(GameObject gameObject)
    {
        SetOutlineState(false);
    }
}
