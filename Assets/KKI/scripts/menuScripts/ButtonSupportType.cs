using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static enums;

public class ButtonSupportType : GroupButton
{
    [SerializeField]
    private typeOfSupport m_typeOfSupport;
    public typeOfSupport TypeOfSupport => m_typeOfSupport;
    public event Action<ButtonSupportType> OnButtonClick;
    private typeOfSupport chosenTypeOfSupport;
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        chosenTypeOfSupport = TypeOfSupport;
        m_button.onClick.AddListener(OnCLick);
    }
        
    protected override void OnCLick()
    {
        base.OnCLick();
        if (IsEnabled)
        {
            m_typeOfSupport = chosenTypeOfSupport;

        }
        else
        {
            m_typeOfSupport = typeOfSupport.Все;
        }
        OnButtonClick?.Invoke(this);
    }

}
