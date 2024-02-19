using BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCellsForAttack : Node
{
    private List<Character> enemiesToAttack = new List<Character>();
    private BattleSystem m_battleSystem;
    private EnemyController m_EnemyBT;
    public CheckCellsForAttack(BattleSystem battleSystem, EnemyController EnemyBT)
    {
        m_battleSystem = battleSystem;
        m_EnemyBT = EnemyBT;
    }


    public override NodeState Evaluate()
    {
        EnemyCharacter enemyCharacter = m_EnemyBT.CurrentEnemyCharacter;
        SetEnemiesForAttack(enemyCharacter);

        if (enemyCharacter.IsAttackedOnTheMove)
        {
            state = NodeState.FAILURE;
            return state;
        }
        else
        {
            Debug.Log($"{enemiesToAttack.Count}" );
            if (enemiesToAttack.Count > 0)
            {
                parent.parent.SetData("enemy", enemiesToAttack[UnityEngine.Random.Range(0, enemiesToAttack.Count)]);
                state = NodeState.SUCCESS;
                Debug.Log($"{state}");
                return state;
            }
            else
            {
                state = NodeState.FAILURE;
                Debug.Log($"{state}");
                return state;
            }
        }
    }

    public void SetEnemiesForAttack(Character character)
    {
        enemiesToAttack.Clear();
        SetAttackableCells(character.PositionOnField, enums.Directions.top, character);
        SetAttackableCells(character.PositionOnField, enums.Directions.bottom, character);
        SetAttackableCells(character.PositionOnField, enums.Directions.right, character);
        SetAttackableCells(character.PositionOnField, enums.Directions.left, character);
    }
    private void SetAttackableCells(Vector2 pos, enums.Directions direction, Character character)
    {
        int newI = (int)pos.x;
        int newJ = (int)pos.y;

        for (int i = 0; i < character.Range; i++)
        {
            switch (direction)
            {
                case enums.Directions.top:
                    newI--;
                    break;
                case enums.Directions.bottom:
                    newJ--;
                    break;
                case enums.Directions.right:
                    newI++;
                    break;
                case enums.Directions.left:
                    newJ++;
                    break;
            }

            if (newI >= 7 || newI < 0)
            {
                break;
            }
            if (newJ >= 11 || newJ < 0)
            {
                break;
            }

            Cell cell = m_battleSystem.FieldController.GetCell(newI, newJ);
            PlayerCharacter enemy = cell.GetComponentInChildren<PlayerCharacter>();
            StaticEnemyCharacter staticEnemy = cell.GetComponentInChildren<StaticEnemyCharacter>();
            if (cell.transform.childCount > 0)
            {
                if (enemy != null)
                {
                    enemiesToAttack.Add(enemy);
                }
                if (staticEnemy != null)
                {
                    enemiesToAttack.Add(staticEnemy);
                }
                if (m_EnemyBT.CurrentEnemyCharacter.Class == enums.Classes.Маг)
                {
                    continue;
                }
                else
                {
                    break;
                }

            }

        }
    }
}

