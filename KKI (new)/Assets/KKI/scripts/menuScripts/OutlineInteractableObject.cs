using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineInteractableObject : InteractableObject
{
    private Outline outline;

    private BoxCollider boxCollider;
    public BoxCollider BoxCollider => boxCollider;
    // Start is called before the first frame update
    void Start()
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

    private void EnableOutline()
    {
        outline.enabled = true;
    }
    private void DisableOutline()
    {
        outline.enabled = false;
    }
}
