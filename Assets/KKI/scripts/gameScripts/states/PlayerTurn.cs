using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


[Serializable]
public class PlayerTurn : State
{
    private List<Cell> cellsToMove = new();
    private List<Enemy> enemiesToAttack = new();
    private CompositeDisposable disposables = new();

    public PlayerTurn(BattleSystem battleSystem) : base(battleSystem)
    {
    }

    public override IEnumerator Start()
    {
        /*Логика при выборе старте*/
        new WaitForSecondsRealtime(1f);

        BattleSystem.GameUIPresenter.OnPlayerTurnStart();
        
        BattleSystem.PointsOfAction = 20;

        CheckPlayerTurnCountables();

        foreach (var playerCharacter in BattleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.ResetCharacter();
        }

        foreach (var staticEnemy in BattleSystem.EnemyController.StaticEnemyCharObjects)
        {
            staticEnemy.AttackPlayerCharacters(BattleSystem);
        }

        OnStepStarted += OnPlayerTurnStarted;
        OnStepCompleted += OnPlayerTurnCompleted;

     
        OnStepStartedInvoke();
        yield break;
    }


    //При выборе персонажа
    public override IEnumerator ChooseCharacter(GameObject character)
    {
        PlayerCharacter playerCharacter = character.GetComponent<PlayerCharacter>();

        foreach (var item in cellsToMove)
        {
            item.OnClick -= BattleSystem.OnMoveButton;
        }
        foreach (var item in enemiesToAttack)
        {
            item.OnClick -= BattleSystem.OnAttackButton;
        }

        BattleSystem.FieldController.TurnOffCells();
        cellsToMove = BattleSystem.GetCellsForMove(playerCharacter, playerCharacter.Speed);
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

    public override IEnumerator Move(GameObject cell)
    {
        Cell cellToMove = cell.GetComponent<Cell>();
        PlayerCharacter playerCharacter = BattleSystem.PlayerController.CurrentPlayerCharacter;
        Cell currentCell = playerCharacter.GetComponentInParent<Cell>();

        int moveCost = BattleSystem.FieldController.GetMoveCost(currentCell,cellToMove, BattleSystem.State);
        foreach (var item in cellsToMove)
        {
            item.OnClick -= BattleSystem.OnMoveButton;
        }
        foreach (var item in enemiesToAttack)
        {
            item.OnClick -= BattleSystem.OnAttackButton;
            playerCharacter.ParentCell.OnClick -= BattleSystem.OnAttackButton;
        }

        if (moveCost > BattleSystem.PointsOfAction )
        {
            BattleSystem.GameUIPresenter.AddMessageToGameLog("Недостаточно очков действий");
            yield break;
        }

        if (moveCost > playerCharacter.Speed)
        {
            BattleSystem.GameUIPresenter.AddMessageToGameLog("Недостаточно скорости у персонажа");
            yield break;
        }

        BattleSystem.PointsOfAction -= moveCost;
        playerCharacter.Speed -= Convert.ToInt32(moveCost);

        Vector3 cellToMovePos = cellToMove.transform.position;
        playerCharacter.transform.DOMove(new Vector3(cellToMovePos.x, playerCharacter.transform.position.y, cellToMovePos.z), 0.5f).OnComplete(() =>
        {
            playerCharacter.transform.SetParent(cellToMove.transform);
            playerCharacter.transform.localPosition = new Vector3(0, 1, 0);
            foreach (var staticEnemy in BattleSystem.EnemyController.StaticEnemyCharObjects)
            {
                staticEnemy.AttackPlayerCharacter(BattleSystem, playerCharacter);
            }
        });

        if (BattleSystem.PointsOfAction == 0)
        {
            BattleSystem.SetEnemyTurn();
        }
        yield break;
    }
    public override IEnumerator Attack(GameObject target)
    {
        if (2 <= BattleSystem.PointsOfAction)
        {
            PlayerCharacter playerCharacter = BattleSystem.PlayerController.CurrentPlayerCharacter;
            Enemy currentTarget = target.GetComponent<Enemy>();

            playerCharacter.IsAttackedOnTheMove = true;

            float finalDamage = currentTarget.Damage(playerCharacter);
            bool isDeath = currentTarget.Health == 0;
            if (finalDamage > 0)
            {
                BattleSystem.GameUIPresenter.AddMessageToGameLog($"{playerCharacter.CharacterName} наносит  юниту {currentTarget.CharacterName} {finalDamage * 100:00.00} урона");
            }
            else
            {
                BattleSystem.GameUIPresenter.AddMessageToGameLog($"{currentTarget.CharacterName} избежал получения урона от {playerCharacter.CharacterName}");
            }

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

                BattleSystem.GameUIPresenter.AddMessageToGameLog($"Вражеский юнит {currentTarget.CharacterName} убит");
                GameObject.Destroy(currentTarget.gameObject);             
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
    public override IEnumerator UseSupportCard(GameObject gameObject)
    {

        ResetPlayer();

        GameSupportCardDisplay supportCardDisplay = gameObject.GetComponent<GameSupportCardDisplay>();

        Observable.EveryUpdate().Where(x => Input.GetKey(KeyCode.Escape)).Subscribe(x =>
        {
            supportCardDisplay.GameSupportСardAbility.CancelUsingCard();
        }).AddTo(disposables);

        if (supportCardDisplay.GameSupportСardAbility is ITurnCountable turnCountable)
        {
            if (turnCountable.IsBuff)
            {
                BattleSystem.PlayerTurnCountables.Add(turnCountable);
                turnCountable.TurnCount--;
            }
            else
            {
                BattleSystem.EnemyTurnCountables.Add(turnCountable);
            }
        }

        
        supportCardDisplay.GameSupportСardAbility.SelectCard();
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

    public override IEnumerator UseItem()
    {
        /*Логика при применении предмета*/
        yield break;
    }
    public void OnPlayerTurnStarted()
    {
        Subscribe();
        foreach (var gameSupportCArdDisplay in BattleSystem.GameUIPresenter.GameSupportCards)
        {
            gameSupportCArdDisplay.GameSupportСardAbility.OnUsingCancel += OnUsingCancel;
            gameSupportCArdDisplay.GameSupportСardAbility.OnSupportCardAbilityUsed += OnSupportCardAbilityUsed;
            gameSupportCArdDisplay.GameSupportСardAbility.OnSupportCardAbilityCharacterSelected += OnSupportCardAbilityCharacterSelected;
        }


        BattleSystem.FieldController.TurnOnCells();
    }

    public void OnPlayerTurnCompleted()
    {
        ResetPlayer();
        foreach (var gameSupportCArdDisplay in BattleSystem.GameUIPresenter.GameSupportCards)
        {
            gameSupportCArdDisplay.GameSupportСardAbility.OnUsingCancel -= OnUsingCancel;
            gameSupportCArdDisplay.GameSupportСardAbility.OnSupportCardAbilityUsed -= OnSupportCardAbilityUsed;
            gameSupportCArdDisplay.GameSupportСardAbility.OnSupportCardAbilityCharacterSelected -= OnSupportCardAbilityCharacterSelected;
        }

        BattleSystem.FieldController.TurnOnCells();
    }


    private void ResetPlayer()
    {
        foreach (var SupportCard in BattleSystem.GameUIPresenter.GameSupportCards)
        {
            SupportCard.DragAndDropComponent.OnDropEvent -= OnDropEvent;
        }
        foreach (var playerCharacter in BattleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= BattleSystem.OnChooseCharacterButton;
            playerCharacter.IsChosen = false;
        }
        foreach (var cellToMove in cellsToMove)
        {
            cellToMove.OnClick -= BattleSystem.OnMoveButton;
        }
        foreach (var enemyToAttack in enemiesToAttack)
        {
            enemyToAttack.OnClick -= BattleSystem.OnAttackButton;
        }
        cellsToMove.Clear();
        enemiesToAttack.Clear();
    }


    private void OnSupportCardAbilityUsed(ICardUsable usable)
    {
        SetStateToNormal();
        ClearDisposables();
    }
    private void ClearDisposables()
    {
        disposables.Dispose();
        disposables.Clear();
        disposables = new();
    }
    private void SetStateToNormal()
    {
        Subscribe();
        if (BattleSystem.PlayerController.CurrentPlayerCharacter != null)
        {
            BattleSystem.PlayerController.CurrentPlayerCharacter.IsChosen = false;
        }
    }
    private void Subscribe()
    {
        foreach (var SupportCard in BattleSystem.GameUIPresenter.GameSupportCards)
        {
            SupportCard.DragAndDropComponent.OnDropEvent += OnDropEvent;           
        }
        foreach (var playerCharacter in BattleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick += BattleSystem.OnChooseCharacterButton;
        }

    }
    private void OnDropEvent(GameObject gameObject)
    {
        foreach (var SupportCard in BattleSystem.GameUIPresenter.GameSupportCards)
        {
            SupportCard.IsEnabled = false;
        }
    }
    private void OnUsingCancel()
    {
        SetStateToNormal();
        ClearDisposables();
        foreach (var SupportCard in BattleSystem.GameUIPresenter.GameSupportCards)
        {
            SupportCard.IsEnabled = true;
        }
    }
    private void CheckPlayerTurnCountables()
    {
        Debug.Log(BattleSystem.PlayerTurnCountables.Count);
        foreach (var playerTurnCountable in BattleSystem.PlayerTurnCountables)
        {
            if (playerTurnCountable.TurnCount == 0)
            {
                playerTurnCountable.ReturnToNormal();
                BattleSystem.PlayerTurnCountables.Remove(playerTurnCountable);
                CheckPlayerTurnCountables();
                break;
            }
            else
            {
                playerTurnCountable.TurnCount--;
            }
        }
    }

    private void OnSupportCardAbilityCharacterSelected(ICharacterSelectable selectable)
    {
        ClearDisposables();
    }

    public void SetEnemiesForAttack(Character character)
    {
        enemiesToAttack.Clear();
        if (character.IsAttackedOnTheMove) return;

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

            Cell cell = BattleSystem.FieldController.GetCell(newI, newJ);
            Enemy enemy = cell.GetComponentInChildren<Enemy>();
            KostilEnemy kostilEnemy = cell.GetComponentInChildren<KostilEnemy>();
            if (cell.transform.childCount > 0)
            {
                if (enemy != null&&enemy is not KostilEnemy)
                {
                    cell.SetColor("attack");
                    enemiesToAttack.Add(enemy);
                }
                if (kostilEnemy!=null)
                {
                    cell.SetColor("attack");
                    enemiesToAttack.Add(kostilEnemy.WallEnemyCharacter);
                }
                if (BattleSystem.PlayerController.CurrentPlayerCharacter.Class == enums.Classes.Маг)
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