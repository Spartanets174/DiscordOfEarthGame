using Nobi.UiRoundedCorners;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Outline),typeof(ImageWithRoundedCorners))]
public class OutlineClicableUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField]
    private UnityEngine.UI.Outline outline;
    [SerializeField]
    private ImageWithRoundedCorners roundedCorners;

    public event Action<GameObject> OnClick;
    public event Action OnHoverExit;
    public event Action OnHoverEnter;

    
    private void Start()
    {

        outline.enabled = false;
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        roundedCorners.radius = 10;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
        OnHoverExit?.Invoke();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData.pointerClick);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
        OnHoverEnter?.Invoke();
    }
}
