using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
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
            playerCharacter.ParentCell.OnClick -= BattleSystem.OnAttackButton;
        }
        cellsToMove = new();
        enemiesToAttack = new();


        BattleSystem.FieldController.TurnOffCells();
        BattleSystem.FieldController.InvokeActionOnField((cell) =>
        {                     
            bool top = true;
            bool bottom = true;
            bool left = true;
            bool rigth = true;
            for (int i = 1; i <= playerCharacter.Speed; i++)
            {
                Cell topCell = BattleSystem.IsCellExist(-i, 0, playerCharacter.PositionOnField);
                Cell bottomCell = BattleSystem.IsCellExist(0, -i, playerCharacter.PositionOnField);
                Cell leftCell = BattleSystem.IsCellExist(i, 0, playerCharacter.PositionOnField);
                Cell rigtCell = BattleSystem.IsCellExist(0, i, playerCharacter.PositionOnField);

                top = topCell != null&& top;
                bottom = bottomCell != null && bottom;
                rigth = rigtCell != null && rigth;
                left = leftCell != null && left;

                SetActiveCell(topCell,top);
                SetActiveCell(bottomCell, bottom);
                SetActiveCell(rigtCell, rigth);
                SetActiveCell(leftCell, left);
            }

            top = true;
            bottom = true;
            left = true;
            rigth = true;

            for (int i = 1; i <= playerCharacter.Range; i++)
            {
                Cell topCell = IsEnemyExist(-i, 0, playerCharacter.PositionOnField);
                Cell bottomCell = IsEnemyExist(0, -i, playerCharacter.PositionOnField);
                Cell leftCell = IsEnemyExist(i, 0, playerCharacter.PositionOnField);
                Cell rigtCell = IsEnemyExist(0, i, playerCharacter.PositionOnField);
 
                top = topCell != null && top && !playerCharacter.IsAttackedOnTheMove;
                bottom = bottomCell != null && bottom && !playerCharacter.IsAttackedOnTheMove;
                rigth = rigtCell != null && rigth && !playerCharacter.IsAttackedOnTheMove;
                left = leftCell != null && left && !playerCharacter.IsAttackedOnTheMove;
             
                SetAttackableCell(topCell, top);
                SetAttackableCell(bottomCell, bottom);
                SetAttackableCell(leftCell, rigth);
                SetAttackableCell(rigtCell, left);           
            }         
        });

        foreach (var item in cellsToMove)
        {
            item.OnClick += BattleSystem.OnMoveButton;
        }
        foreach (var item in enemiesToAttack)
        {
            item.OnClick += BattleSystem.OnAttackButton;
            playerCharacter.ParentCell.OnClick += BattleSystem.OnAttackButton;
        }
        yield break;
    }
    private void SetActiveCell(Cell cell, bool isAllowed)
    {
        if (cell != null && isAllowed)
        {
            cell.SetCellMovable();
            cellsToMove.Add(cell);
        }
    }

    private void SetAttackableCell(Cell cell, bool isAllowed)
    {
        if (cell == null) return;

        Enemy enemy = cell.GetComponentInChildren<Enemy>();
        if (isAllowed && enemy != null)
        {
            cell.SetCellState(true);
            cell.SetColor("attack", (cell.CellIndex.y + cell.CellIndex.x) % 2 == 0);
            enemiesToAttack.Add(enemy);
        }
    }

    private Cell IsEnemyExist(int i, int j, Vector2 pos)
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

        return BattleSystem.FieldController.GetCell((int)newI, (int)newJ);
    }

    public override IEnumerator Move(GameObject cell)
    {
        Cell currentCell = cell.GetComponent<Cell>();
        PlayerCharacter playerCharacter = BattleSystem.GetCurrentPlayerChosenCharacter();
        Vector2 pos = playerCharacter.PositionOnField;
        float numOfCells = Mathf.Abs((pos.x+pos.y) - (currentCell.CellIndex.x + currentCell.CellIndex.y));

        if (numOfCells <= BattleSystem.PointsOfAction)
        {
            BattleSystem.PointsOfAction -= numOfCells;
            playerCharacter.Speed -= Convert.ToInt32(numOfCells);
            //Установление координат в новой клетке
            playerCharacter.transform.SetParent(currentCell.transform);
            playerCharacter.transform.localPosition = new Vector3(0, 1, 0);

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
            PlayerCharacter playerCharacter = BattleSystem.GetCurrentPlayerChosenCharacter();
            Enemy currentTarget = target.GetComponent<Enemy>();

            playerCharacter.IsChosen = false;

            playerCharacter.IsAttackedOnTheMove = true;

            bool isDeath = currentTarget.Damage(playerCharacter);
            /*target.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<healthBar>().SetHealth((float)target.health);*/
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

               /* GameObject.Destroy(target.gameObject);
                BattleSystem.gameLog.text += $"Вражеский юнит {target.name} убит" + "\n";
                BattleSystem.gameLogScrollBar.value = 0;*/
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

            foreach (var item in cellsToMove)
            {
                item.OnClick -= BattleSystem.OnMoveButton;
            }
            foreach (var item in enemiesToAttack)
            {
                item.OnClick -= BattleSystem.OnAttackButton;
                playerCharacter.ParentCell.OnClick -= BattleSystem.OnAttackButton;
            }
        }
        else
        {
           /*BattleSystem.gameLog.text += "Недостаточно очков действий" + "\n";*/
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