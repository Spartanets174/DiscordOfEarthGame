using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
[Serializable]
public class PlayerTurn : State
{
    private List<Cell> cellsToMove = new();
    private List<Enemy> enemiesToAttack = new();

    public PlayerTurn(BattleSystem battleSystem) : base(battleSystem)
    {
    }

    public override IEnumerator Start()
    {
        /*Логика при выборе старте*/
        new WaitForSecondsRealtime(1f);

        BattleSystem.GameUIPresenter.AddMessageToGameLog("Ваш ход.");
        BattleSystem.GameUIPresenter.OnPlayerTurnStart();
        BattleSystem.FieldController.TurnOnCells();
        BattleSystem.PointsOfAction = 20;

        OnStepStarted += OnPlayerTurnStarted;
        OnStepCompleted += OnPlayerTurnCompleted;

        foreach (var playerCharacter in BattleSystem.PlayerCharactersObjects)
        {
            playerCharacter.ResetCharacter();     
        }

        OnStepStartedInvoke();
        yield break;
    }

    private void OnPlayerTurnStarted()
    {
        foreach (var playerCharacter in BattleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick += BattleSystem.OnChooseCharacterButton;
        }
    }

    private void OnPlayerTurnCompleted()
    {
        foreach (var playerCharacter in BattleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= BattleSystem.OnChooseCharacterButton;
        }    
    }

  

    //При выборе персонажа
    public override IEnumerator ChooseCharacter(GameObject character)
    {
        PlayerCharacter playerCharacter = character.GetComponent<PlayerCharacter>();

        foreach (var item in cellsToMove)
        {
            item.OnClick -=BattleSystem.OnMoveButton;
        }
        foreach (var item in enemiesToAttack)
        {
            item.OnClick -= BattleSystem.OnAttackButton;
        }

        BattleSystem.FieldController.TurnOffCells();
        cellsToMove = BattleSystem.GetCellsForMove(playerCharacter);
        SetEnemiesForAttack(playerCharacter);

        foreach (var item in cellsToMove)
        {
            item.OnClick += BattleSystem.OnMoveButton;
        }
        foreach (var item in enemiesToAttack)
        {
            item.OnClick += BattleSystem.OnAttackButton;
        }
        yield break;
    }
    public void SetEnemiesForAttack(Character character)
    {
        enemiesToAttack.Clear();
        SetAttackableCells(character.PositionOnField, enums.Directions.top, character);
        SetAttackableCells(character.PositionOnField, enums.Directions.bottom, character);
        SetAttackableCells(character.PositionOnField, enums.Directions.right, character);
        SetAttackableCells(character.PositionOnField, enums.Directions.left, character);
        Debug.Log(enemiesToAttack.Count);
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

            Cell cell = BattleSystem.FieldController.GetCell(newI, newJ);
            Enemy enemy = cell.GetComponentInChildren<Enemy>();
            if (cell.transform.childCount>0)
            {
                if (enemy != null)
                {
                    cell.SetColor("attack", (cell.CellIndex.y + cell.CellIndex.x) % 2 == 0);
                    enemiesToAttack.Add(enemy);
                }
                if (BattleSystem.CurrentPlayerCharacter.Class == enums.Classes.Маг)
                {
                    continue;                 
                }
                else
                {
                    break;
                }
               
            }
            
        }

        /*if (cell == null) return;

        Enemy enemy = cell.GetComponentInChildren<Enemy>();
        if (isAllowed && enemy != null)
        {
            cell.SetColor("attack", (cell.CellIndex.y + cell.CellIndex.x) % 2 == 0);
            enemiesToAttack.Add(enemy);
        }*/
    }
    /* public List<Enemy> SetEnemiesForAttack(Character character)
     {
         List<Enemy> enemiesToAttack = new();

         bool top = true;
         bool bottom = true;
         bool left = true;
         bool rigth = true;

         for (int i = 1; i <= character.Range; i++)
         {
             Cell topCell = GetCellForAttack(-i, 0, character.PositionOnField);
             Cell bottomCell = GetCellForAttack(0, -i, character.PositionOnField);
             Cell leftCell = GetCellForAttack(i, 0, character.PositionOnField);
             Cell rigtCell = GetCellForAttack(0, i, character.PositionOnField);

             top = topCell != null && top && !character.IsAttackedOnTheMove;
             bottom = bottomCell != null && bottom && !character.IsAttackedOnTheMove;
             rigth = rigtCell != null && rigth && !character.IsAttackedOnTheMove;
             left = leftCell != null && left && !character.IsAttackedOnTheMove;

             SetAttackableCell(topCell, top, enemiesToAttack);
             SetAttackableCell(bottomCell, bottom, enemiesToAttack);
             SetAttackableCell(leftCell, rigth, enemiesToAttack);
             SetAttackableCell(rigtCell, left, enemiesToAttack);
         }
         return enemiesToAttack;
     }*/

    /* private void SetAttackableCell(Cell cell, bool isAllowed, List<Enemy> enemiesToAttack)
     {
         if (cell == null) return;

         Enemy enemy = cell.GetComponentInChildren<Enemy>();
         if (isAllowed && enemy != null)
         {
             cell.SetColor("attack", (cell.CellIndex.y + cell.CellIndex.x) % 2 == 0);
             enemiesToAttack.Add(enemy);
         }
     }

     private Cell GetCellForAttack(int i, int j, Vector2 pos)
     {
         float newI = pos.x + i;
         float newJ = pos.y + j;
         if (newI >= 7 || newI < 0)
         {
             return null;
         }
         if (newJ >= 11 || newJ < 0)
         {
             return null;
         }

         Cell cell = BattleSystem.FieldController.GetCell((int)newI, (int)newJ);
         Enemy enemy = cell.GetComponentInChildren<Enemy>();
         if (cell.transform.childCount > 0)
         {
             if (enemy == null && BattleSystem.CurrentPlayerCharacter.Class != enums.Classes.Маг)
             {
                 return null;
             }
         }

         return cell;
     }*/

    public override IEnumerator Move(GameObject cell)
    {
        Cell currentCell = cell.GetComponent<Cell>();
        PlayerCharacter playerCharacter = BattleSystem.CurrentPlayerCharacter;
        Vector2 pos = playerCharacter.PositionOnField;
        float numOfCells = Mathf.Abs((pos.x+pos.y) - (currentCell.CellIndex.x + currentCell.CellIndex.y));

        if (numOfCells <= BattleSystem.PointsOfAction)
        {
            BattleSystem.PointsOfAction -= numOfCells;
            playerCharacter.Speed -= Convert.ToInt32(numOfCells);

            Vector3 currentCellPos = currentCell.transform.position;
            playerCharacter.transform.DOMove(new Vector3(currentCellPos.x, playerCharacter.transform.position.y, currentCellPos.z),0.5f).OnComplete(() =>
            {
                playerCharacter.transform.SetParent(currentCell.transform);
                playerCharacter.transform.localPosition = new Vector3(0, 1, 0);
            }) ;
            

            if (BattleSystem.PointsOfAction == 0)
            {
                BattleSystem.SetEnemyTurn();
            }
                
        }
        foreach (var item in cellsToMove)
        {
            item.OnClick -= BattleSystem.OnMoveButton;
        }
        foreach (var item in enemiesToAttack)
        {
            item.OnClick -= BattleSystem.OnAttackButton;
            playerCharacter.ParentCell.OnClick -= BattleSystem.OnAttackButton;
        }
        yield break;
    }
    public override IEnumerator Attack(GameObject target)
    {
        if (2 <= BattleSystem.PointsOfAction)
        {
            PlayerCharacter playerCharacter = BattleSystem.CurrentPlayerCharacter;
            Enemy currentTarget = target.GetComponent<Enemy>();

            playerCharacter.IsAttackedOnTheMove = true;

            float finalDamage = currentTarget.Damage(playerCharacter);
            bool isDeath = currentTarget.Health == 0;
            BattleSystem.GameUIPresenter.AddMessageToGameLog($"{playerCharacter.CharacterName} наносит  юниту {currentTarget.CharacterName} {finalDamage * 100:00.00} урона");

            if (isDeath)
            {
                if (currentTarget is StaticEnemyCharacter)
                {
                    BattleSystem.EnemyController.StaticEnemyCharObjects.Remove((StaticEnemyCharacter)currentTarget);
                }
                else
                {
                    BattleSystem.EnemyController.EnemyCharObjects.Remove((EnemyCharacter)currentTarget);
                }
                GameObject.Destroy(currentTarget.gameObject);
                BattleSystem.GameUIPresenter.AddMessageToGameLog($"Вражеский юнит {target.name} убит");
            }

            foreach (var item in cellsToMove)
            {
                item.OnClick -= BattleSystem.OnMoveButton;
            }
            foreach (var item in enemiesToAttack)
            {
                item.OnClick -= BattleSystem.OnAttackButton;
            }

            if (BattleSystem.EnemyController.EnemyCharObjects.Count == 0)
            {
                BattleSystem.EnemyController.gameObject.SetActive(false);
                BattleSystem.EnemyController.StopTree();
                BattleSystem.SetState(new Won(BattleSystem));
            }

            BattleSystem.PointsOfAction -= 2;
            if (BattleSystem.PointsOfAction == 0)
            {
                BattleSystem.SetEnemyTurn();
            }
            BattleSystem.FieldController.TurnOnCells();
        }
        else
        {
            BattleSystem.GameUIPresenter.AddMessageToGameLog("Недостаточно очков действий");
        }
       
        yield break;
    }
    public override IEnumerator UseAttackAbility()
    {
        /*Логика при применении способности 1*/
        yield break;
    }
    public override IEnumerator UseDefensiveAbility()
    {
        /*Логика при применении способности 2*/
        yield break;
    }
    public override IEnumerator UseBuffAbility()
    {
        /*Логика при применении способности 3*/
        yield break;
    }
    public override IEnumerator UseSupportCard()
    {
        /*Логика при применении карты помощи*/
        yield break;
    }
    public override IEnumerator UseItem()
    {
        /*Логика при применении предмета*/
        yield break;
    }
}