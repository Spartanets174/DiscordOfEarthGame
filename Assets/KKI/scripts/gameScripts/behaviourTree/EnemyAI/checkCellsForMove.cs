using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCellsForMove : Node
{
    private BattleSystem m_battleSystem;
    private EnemyController m_EnemyBT;
    List<Cell> possibleCells;
    public CheckCellsForMove(BattleSystem battleSystem, EnemyController EnemyBT)
    {
        m_battleSystem = battleSystem;
        m_EnemyBT = EnemyBT;
    }
    public override NodeState Evaluate()
    {
        EnemyCharacter enemyCharacter = m_EnemyBT.CurrentEnemyCharacter;
        possibleCells = m_battleSystem.FieldController.GetCellsForMove(enemyCharacter, enemyCharacter.Speed);

       
        if (possibleCells.Count > 0)
        {
            SetCell(enemyCharacter);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            state = NodeState.FAILURE;
            return state;
        }
    }

    private void SetCell(EnemyCharacter enemyCharacter)
    {
        bool isCellValid=false;
        Cell currentCell = null;
        float count = 0;
        while (!isCellValid)
        {
            currentCell = possibleCells[Random.Range(0, possibleCells.Count)];
            Vector2 pos = enemyCharacter.PositionOnField;
            float numOfCells = Mathf.Abs((pos.x + pos.y) - (currentCell.CellIndex.x + currentCell.CellIndex.y));
            if (numOfCells<=m_battleSystem.PointsOfAction.Value)
            {
                isCellValid = true;
            }
            count++;
            if (count>100)
            {
                break;
            }
        }
        parent.SetData("cell", currentCell);
    }
}
