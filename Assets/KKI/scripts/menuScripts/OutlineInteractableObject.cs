using HighlightPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HighlightEffect))]
public class OutlineInteractableObject : InteractableObject
{
    private HighlightEffect highlightEffect;

    private Collider m_collider;
    public Collider Collider => m_collider;
    // Start is called before the first frame update
    void Awake()
    {
        highlightEffect = GetComponent<HighlightEffect>();
        m_collider = GetComponent<Collider>();
        OnHoverEnter += EnableOutline;
        OnHoverExit += DisableOutline;
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
        highlightEffect.highlighted = true;
    }
    protected void DisableOutline(GameObject gameObject)
    {
        highlightEffect.highlighted = false;
    }
}
