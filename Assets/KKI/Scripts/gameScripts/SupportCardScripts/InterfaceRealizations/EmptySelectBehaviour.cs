using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySelectBehaviour : ICardSelectable
{
    
    public event Action OnSelected;
    private string m_selectCardTipText;

    public string SelectCardTipText
    {
        get
        {
            return m_selectCardTipText;
        }
    }
    public EmptySelectBehaviour(string text)
    {
        m_selectCardTipText = text;
    }

    public void SelectCard()
    {
        OnSelected?.Invoke();
    }
}
