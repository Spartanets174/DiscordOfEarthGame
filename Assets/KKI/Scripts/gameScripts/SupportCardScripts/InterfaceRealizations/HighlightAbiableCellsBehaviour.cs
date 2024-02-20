using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightAbiableCellsBehaviour : ICharacterSelectable
{
    private BattleSystem battleSystem;
    private string m_selectCharacterTipText;

    public event Action OnSelectCharacter;

    public string SelectCharacterTipText => m_selectCharacterTipText;

    public HighlightAbiableCellsBehaviour(string text, BattleSystem battleSystem)
    {
        m_selectCharacterTipText = text;
        this.battleSystem = battleSystem;
    }

    public void SelectCharacter(GameObject gameObject)
    {
        if (battleSystem.State is PlayerTurn)
        {
            battleSystem.FieldController.InvokeActionOnField(SelectCell);
        }
        
        OnSelectCharacter?.Invoke();
    }

    private void SelectCell(Cell cell)
    {

        if (cell.transform.childCount==0)
        {
            cell.SetCellState(true);
            cell.SetCellMovable();
        }
        else
        {
            cell.SetCellState(false);
        }
    }
}
