using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class EnemyTurn : State
{
    public EnemyTurn(BattleSystem battleSystem) : base(battleSystem)
    {

    }
    public override IEnumerator Start()
    {
        new WaitForSecondsRealtime(1f);

        BattleSystem.GameUIPresenter.OnEnemyTurnStart();
        BattleSystem.FieldController.TurnOnCells();
        BattleSystem.PointsOfAction = 20;

        foreach (var enemyTurnCountable in BattleSystem.EnemyTurnCountables)
        {
            Debug.Log(enemyTurnCountable.TurnCount);
            if (enemyTurnCountable.TurnCount == 0)
            {
                enemyTurnCountable.ReturnToNormal();
            }
            else
            {
                enemyTurnCountable.TurnCount--;
            }
            Debug.Log(enemyTurnCountable.TurnCount);
        }
        

        foreach (var staticEnemy in BattleSystem.EnemyController.StaticEnemyCharObjects)
        {
            staticEnemy.AttackEnemyCharacters(BattleSystem);
        }
        
        foreach (var enemyCharacter in BattleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.ResetCharacter();
        }

        BattleSystem.EnemyController.SetupTree();
        BattleSystem.EnemyController.RestartTree();
        
        yield break;
    }
    public override IEnumerator ChooseCharacter(GameObject character)
    {
        EnemyCharacter enemyCharacter = character.GetComponent<EnemyCharacter>();
        BattleSystem.EnemyController.SetCurrentEnemyChosenCharacter(enemyCharacter);
        new WaitForSecondsRealtime(1f);
        yield return character;
    }
    

    public override IEnumerator Move(GameObject cell)
    {
        Cell cellToMove = cell.GetComponent<Cell>();
        EnemyCharacter enemyCharacter = BattleSystem.EnemyController.CurrentEnemyCharacter;
        Cell currentCell = enemyCharacter.GetComponentInParent<Cell>();
        int moveCost = BattleSystem.FieldController.GetMoveCost(currentCell, cellToMove,BattleSystem.State);

        if (moveCost > BattleSystem.PointsOfAction)
        {
            Debug.Log("Недостаточно очков действий");
            new WaitForSecondsRealtime(1f);
            yield break;
        }

        if (moveCost > enemyCharacter.Speed)
        {
            Debug.Log("Недостаточно скорости у персонажа");
            new WaitForSecondsRealtime(1f);
            yield break;
        }

        BattleSystem.PointsOfAction -= moveCost;
        enemyCharacter.Speed -= Convert.ToInt32(moveCost);

        Vector3 cellToMovePos = cellToMove.transform.position;
        enemyCharacter.transform.SetParent(cellToMove.transform);
        enemyCharacter.transform.DOMove(new Vector3(cellToMovePos.x, enemyCharacter.transform.position.y, cellToMovePos.z), 0.5f).OnComplete(() =>
        {
            enemyCharacter.transform.localPosition = new Vector3(0, 1, 0);

            foreach (var staticEnemy in BattleSystem.EnemyController.StaticEnemyCharObjects)
            {
                staticEnemy.AttackEnemyCharacter(BattleSystem, enemyCharacter);
            }
        });

        if (BattleSystem.PointsOfAction == 0)
        {
            BattleSystem.SetPlayerTurn();
        }

        new WaitForSecondsRealtime(1f);
        yield break;
    }
    public override IEnumerator Attack(GameObject target)
    {
        /*Логика при атаке*/
        if (2 <= BattleSystem.PointsOfAction)
        {
            EnemyCharacter enemyCharacter = BattleSystem.EnemyController.CurrentEnemyCharacter;
            Character currentTarget = target.GetComponent<Character>();

            enemyCharacter.IsAttackedOnTheMove = true;

            float finalDamage = currentTarget.Damage(enemyCharacter);
            bool isDeath = currentTarget.Health == 0;
            BattleSystem.GameUIPresenter.AddMessageToGameLog($"{enemyCharacter.CharacterName} наносит  юниту {currentTarget.CharacterName} {finalDamage * 100:00.00} урона");

            if (isDeath)
            {
                if (currentTarget is StaticEnemyCharacter)
                {
                    BattleSystem.EnemyController.StaticEnemyCharObjects.Remove((StaticEnemyCharacter)currentTarget);
                    BattleSystem.GameUIPresenter.AddMessageToGameLog($"Юнит {currentTarget.CharacterName} убит");
                }
                else
                {
                    BattleSystem.PlayerController.PlayerCharactersObjects.Remove((PlayerCharacter)currentTarget);
                    BattleSystem.GameUIPresenter.AddMessageToGameLog($"Союзный юнит {currentTarget.CharacterName} убит");
                }
                GameObject.Destroy(currentTarget.gameObject);               
            }

            if (BattleSystem.PlayerController.PlayerCharactersObjects.Count == 0)
            {
                BattleSystem.SetState(new Lost(BattleSystem));
            }

            BattleSystem.PointsOfAction -= 2;
            if (BattleSystem.PointsOfAction == 0)
            {
                BattleSystem.SetPlayerTurn();
            }
            BattleSystem.FieldController.TurnOnCells();
        }
        new WaitForSecondsRealtime(1f);
        yield break;
    }

    public override IEnumerator UseSupportCard(GameObject cardSupport)
    {

        GameSupportCardDisplay supportCardDisplay = cardSupport.GetComponent<GameSupportCardDisplay>();

        if (supportCardDisplay.GameSupportСardAbility is ITurnCountable turnCountable)
        {
            if (turnCountable.IsBuff)
            {
                BattleSystem.EnemyTurnCountables.Add(turnCountable);
                turnCountable.TurnCount--;
            }
            else
            {               
                BattleSystem.PlayerTurnCountables.Add(turnCountable);
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
}