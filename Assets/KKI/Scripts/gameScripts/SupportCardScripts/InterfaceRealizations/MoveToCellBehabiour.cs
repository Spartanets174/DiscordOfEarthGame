using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCellBehaviour : ICardUsable
{
    private BattleSystem battleSystem;

    public event Action OnCardUse;

    public MoveToCellBehaviour(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
    }

    public void UseAbility(GameObject gameObject)
    {
        Cell currentCell = gameObject.GetComponent<Cell>();
        Character character = battleSystem.State is PlayerTurn?battleSystem.CurrentPlayerCharacter: battleSystem.EnemyController.CurrentEnemyCharacter;
        Vector2 pos = character.PositionOnField;
        float numOfCells = Mathf.Abs((pos.x + pos.y) - (currentCell.CellIndex.x + currentCell.CellIndex.y));

        Vector3 currentCellPos = currentCell.transform.position;
        character.transform.DOMove(new Vector3(currentCellPos.x, character.transform.position.y, currentCellPos.z), 0.5f).OnComplete(() =>
        {
            character.transform.SetParent(currentCell.transform);
            character.transform.localPosition = new Vector3(0, 1, 0);
            foreach (var staticEnemy in battleSystem.EnemyController.StaticEnemyCharObjects)
            {
                if (battleSystem.State is PlayerTurn)
                {
                    staticEnemy.AttackPlayerCharacter(battleSystem, (PlayerCharacter)character);
                }
                else
                {
                    staticEnemy.AttackEnemyCharacter(battleSystem, (EnemyCharacter)character);
                }
            }
        });

        OnCardUse?.Invoke();
    }

}
