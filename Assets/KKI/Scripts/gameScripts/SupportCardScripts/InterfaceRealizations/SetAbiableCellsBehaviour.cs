using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAbiableCellsBehaviour : ICharacterSelectable
{
    private BattleSystem battleSystem;
    private string m_selectCharacterTipText;
    public List<Cell> cellsToMove = new();

    public event Action OnSelectCharacter;

    public string SelectCharacterTipText => m_selectCharacterTipText;

    public SetAbiableCellsBehaviour(string text, BattleSystem battleSystem)
    {
        m_selectCharacterTipText = text;
        this.battleSystem = battleSystem;
    }

    public void SelectCharacter(GameObject gameObject)
    {
        battleSystem.FieldController.TurnOffCells();
        foreach (var cell in cellsToMove)
        {
            if (battleSystem.State is PlayerTurn) cell.SetColor("allowed", (cell.CellIndex.x + cell.CellIndex.y) % 2 == 0);
            cell.SetCellState(true);
        }

        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            if (battleSystem.CurrentPlayerCharacter!= playerCharacter)
            {
                playerCharacter.IsChosen = false;
            }           
        }

        OnSelectCharacter?.Invoke();
    }
}
