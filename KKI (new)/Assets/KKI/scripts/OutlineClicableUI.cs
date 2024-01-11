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
    [SerializeField]
    private GameObject blocker;

    public event Action<GameObject> OnClick;
    public event Action OnHoverExit;
    public event Action OnHoverEnter;

    private bool m_isEnabled;

    public bool IsEnabled
    {
        get
        {
            return m_isEnabled;
        }
        set {
            m_isEnabled = value;
            SetBlockerState(!m_isEnabled);
        }
    }

    public void SetBlockerState(bool state)
    {
        if (blocker != null)
        {
            blocker.SetActive(state);
        }
    }
    
    private void Start()
    {
        IsEnabled = true;
        outline.enabled = false;
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        roundedCorners.radius = 10;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_isEnabled)
        {
            outline.enabled = false;
            OnHoverExit?.Invoke();
        }      
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (m_isEnabled)
        {
            OnClick?.Invoke(eventData.pointerClick);
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (m_isEnabled)
        {
            outline.enabled = true;
            OnHoverEnter?.Invoke();
        }
    }
}
