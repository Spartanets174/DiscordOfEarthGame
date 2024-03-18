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

        CheckEnemyTurnCountables();

        BattleSystem.EnemyController.AttackAllEnemiesStaticCharacters();

        BattleSystem.EnemyController.ResetEnemyCharacters();      
        BattleSystem.EnemyController.SetupTree();
        BattleSystem.EnemyController.RestartTree();
        
        yield break;
    }

    private void CheckEnemyTurnCountables()
    {
        List<ITurnCountable> enemyTurnCountables = new List<ITurnCountable>();
        foreach (var enemyTurnCountable in BattleSystem.EnemyTurnCountables)
        {
            enemyTurnCountables.Add(enemyTurnCountable.Key);
        }
        foreach (var key in enemyTurnCountables)
        {
            if (BattleSystem.EnemyTurnCountables[key] == 0)
            {
                key.ReturnToNormal();
                BattleSystem.EnemyTurnCountables.Remove(key);
                CheckEnemyTurnCountables();
                break;
            }
            else
            {
                BattleSystem.EnemyTurnCountables[key]--;
            }
        }
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
        enemyCharacter.Move(moveCost, cellToMove.transform);

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
            if (finalDamage > 0)
            {
                BattleSystem.GameUIPresenter.AddMessageToGameLog($"{enemyCharacter.CharacterName} наносит  юниту {currentTarget.CharacterName} {finalDamage * 100:00.00} урона");
            }
            else
            {
                BattleSystem.GameUIPresenter.AddMessageToGameLog($"{currentTarget.CharacterName} избежал получения урона от {enemyCharacter.CharacterName}");
            }

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
                BattleSystem.SetLost();
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
                BattleSystem.EnemyTurnCountables.Add(turnCountable, turnCountable.TurnCount);
                BattleSystem.EnemyTurnCountables[turnCountable]--;
            }
            else
            {               
                BattleSystem.PlayerTurnCountables.Add(turnCountable, turnCountable.TurnCount);
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