using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectCellsToAttackInRangeBehaviour : ICardSelectable
{
    public Cell clickedCell;
    public List<Character> enemiesToAttack = new();

    private BattleSystem battleSystem;
    private int range;
    private string m_selectCardTipText;
    private Character chosenCharacter;
    private List<Cell> cells = new();


    public event Action OnSelected;
    public event Action OnCancelSelection;

    public string SelectCardTipText
    {
        get
        {
            return m_selectCardTipText;
        }
    }

    public SelectCellsToAttackInRangeBehaviour(string text, BattleSystem battleSystem, Character character, int range)
    {
        m_selectCardTipText = text;
        this.battleSystem = battleSystem;
        this.range = range;
        chosenCharacter = character;
    }

    public void SelectCard()
    {
        SetEnemiesForAttack(chosenCharacter);
        OnSelectedInvoke(null);
    }

    private void SetEnemiesForAttack(Character character)
    {
        enemiesToAttack.Clear();

        SetAttackableCells(character.PositionOnField, Enums.Directions.top, character);
        SetAttackableCells(character.PositionOnField, Enums.Directions.bottom, character);
        SetAttackableCells(character.PositionOnField, Enums.Directions.right, character);
        SetAttackableCells(character.PositionOnField, Enums.Directions.left, character);
    }
    private void SetAttackableCells(Vector2 pos, Enums.Directions direction, Character character)
    {
        int newI = (int)pos.x;
        int newJ = (int)pos.y;

        for (int i = 0; i < range; i++)
        {
            switch (direction)
            {
                case Enums.Directions.top:
                    newI--;
                    break;
                case Enums.Directions.bottom:
                    newJ--;
                    break;
                case Enums.Directions.right:
                    newI++;
                    break;
                case Enums.Directions.left:
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

            Cell cell = battleSystem.FieldController.GetCell(newI, newJ);
            Character enemy;
            if (battleSystem.State is PlayerTurn)
            {
                enemy = cell.GetComponentInChildren<Enemy>();
            }
            else
            {
                enemy = cell.GetComponentInChildren<PlayerCharacter>();
            }

            KostilEnemy kostilEnemy = cell.GetComponentInChildren<KostilEnemy>();
            if (cell.transform.childCount > 0)
            {
                if (enemy != null && enemy is not KostilEnemy)
                {
                    cell.SetColor("attack");
                    enemiesToAttack.Add(enemy);
                }
                if (kostilEnemy != null)
                {
                    cell.SetColor("attack");
                    enemiesToAttack.Add(kostilEnemy.WallEnemyCharacter);
                }
                if (character.Class == Enums.Classes.Маг)
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

    public void OnSelectedInvoke(GameObject gameObject)
    {
        if (gameObject!=null)
        {
            clickedCell = gameObject.GetComponent<Cell>();
        }
        
        OnSelected?.Invoke();
    }



    public void CancelSelection()
    {
        cells.Clear();
        battleSystem.FieldController.InvokeActionOnField(x =>
        {
            x.SetCellState(true);
            x.SetColor("normal");
        });
        OnCancelSelection?.Invoke();
    }
}
