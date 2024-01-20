using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteractableObject : MonoBehaviour
{
    private bool m_isEnabled;
    public bool IsEnabled
    {
        get => m_isEnabled;
        set
        {
           m_isEnabled = value;
            
        } 
        
    }

    public event Action<GameObject> OnClick;
    public event Action OnHoverEnter;
    public event Action OnHoverExit;
    public event Action OnHover;
    protected virtual void OnMouseEnter()
    {
        if (IsEnabled)
        {
            OnHoverEnterInvoke();
        }     
    }
    protected virtual void OnMouseExit()
    {
        if (IsEnabled)
        {
            OnHoverExitInvoke();
        }      
    }

    protected virtual void OnMouseDown()
    {
        if (IsEnabled)
        {
            OnClickInvoke();
        }     
    }

    protected virtual void OnMouseOver()
    {
        if (IsEnabled)
        {
            OnHoverInvoke();
        }       
    }

    public void OnHoverEnterInvoke()
    {
        OnHoverEnter?.Invoke();
    }

    public void OnHoverExitInvoke()
    {
        OnHoverExit?.Invoke();
        
    }

    public void OnClickInvoke()
    {
        OnClick?.Invoke(this.gameObject);
    }

    public void OnHoverInvoke()
    {
        OnHover?.Invoke();
    }
}