using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyTurn : State
{
    public EnemyTurn(BattleSystem battleSystem) : base(battleSystem)
    {

    }
    public override IEnumerator Start()
    {
        new WaitForSecondsRealtime(1f);

        BattleSystem.GameUIPresenter.SetDragAllowToSupportCards(false);
        BattleSystem.GameUIPresenter.AddMessageToGameLog("Ход противника.");
        BattleSystem.GameUIPresenter.OnEnemyTurnStart();
        BattleSystem.FieldController.TurnOnCells();
        BattleSystem.PointsOfAction = 20;

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
        BattleSystem.GameUIPresenter.AddMessageToGameLog($"Враг планирует свой ход...");
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
        Cell currentCell = cell.GetComponent<Cell>();
        EnemyCharacter enemyCharacter = BattleSystem.EnemyController.CurrentEnemyCharacter;
        Vector2 pos = enemyCharacter.PositionOnField;
        float numOfCells = Mathf.Abs((pos.x + pos.y) - (currentCell.CellIndex.x + currentCell.CellIndex.y));

        BattleSystem.PointsOfAction -= numOfCells;
        enemyCharacter.Speed -= Convert.ToInt32(numOfCells);

        Vector3 currentCellPos = currentCell.transform.position;
        enemyCharacter.transform.SetParent(currentCell.transform);
        enemyCharacter.transform.DOMove(new Vector3(currentCellPos.x, enemyCharacter.transform.position.y, currentCellPos.z), 0.5f).OnComplete(() =>
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
                    BattleSystem.PlayerCharactersObjects.Remove((PlayerCharacter)currentTarget);
                    BattleSystem.GameUIPresenter.AddMessageToGameLog($"Союзный юнит {currentTarget.CharacterName} убит");
                }
                GameObject.Destroy(currentTarget.gameObject);               
            }

            if (BattleSystem.PlayerCharactersObjects.Count == 0)
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