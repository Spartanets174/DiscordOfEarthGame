using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineInteractableObject : InteractableObject
{
    private Outline outline;

    private BoxCollider boxCollider;
    public BoxCollider BoxCollider => boxCollider;
    // Start is called before the first frame update
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        outline = GetComponent<Outline>();
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
        outline.enabled = state;
    }

    protected void EnableOutline()
    {
        outline.enabled = true;
    }
    protected void DisableOutline()
    {
        outline.enabled = false;
    }
}
