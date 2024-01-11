using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteractableObject : MonoBehaviour
{
    private bool m_enabled;
    public bool Enabled
    {
        get => m_enabled;
        set => m_enabled = value;
    }

    public event Action<GameObject> OnClick;
    public event Action OnHoverEnter;
    public event Action OnHoverExit;
    public event Action OnHover;
    private void OnMouseEnter()
    {
        if (Enabled)
        {
            OnHoverEnter?.Invoke();
        }     
    }
    private void OnMouseExit()
    {
        if (Enabled)
        {
            OnHoverExit?.Invoke();
        }      
    }

    private void OnMouseDown()
    {
        if (Enabled)
        {
            OnClick?.Invoke(gameObject);
        }     
    }

    private void OnMouseOver()
    {
        if (Enabled)
        {
            OnHover?.Invoke();
        }       
    }
}