using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class InteractableObject : MonoBehaviour
{
    public event Action OnClick;
    public event Action OnHoverEnter;
    public event Action OnHoverExit;
    public event Action OnHover;
    private void OnMouseEnter()
    {
        OnHoverEnter?.Invoke();
    }
    private void OnMouseExit()
    {
        OnHoverExit?.Invoke();
    }

    private void OnMouseDown()
    {       
        OnClick?.Invoke();
    }

    private void OnMouseOver()
    {  
        OnHover?.Invoke();
    }
}