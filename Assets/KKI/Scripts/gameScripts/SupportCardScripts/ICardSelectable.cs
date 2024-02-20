using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSelectable 
{
    public event Action OnSelected;
    public string SelectCardTipText { get;  }
    public void SelectCard();
}
