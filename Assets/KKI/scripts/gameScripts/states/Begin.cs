using System.Collections;
using UnityEngine;
public class Begin : State
{ 
    public Begin(BattleSystem battleSystem) : base(battleSystem)
    {
    }

    public override IEnumerator Start()
    {
        BattleSystem.GameUIPresenter.AddMessageToGameLog($"������� ����������� ������.");        
        BattleSystem.EnemyController.CreateEnemy();
        BattleSystem.EnemyController.InstantiateEnemies();

        OnStepStarted += AddUnitStatementAction;
        OnStepCompleted += RemoveUnitStatementAction;

        BattleSystem.FieldController.TurnOffCells();
        OnStepStartedInvoke();
        yield break;
    }

    private void AddUnitStatementAction()
    {
        foreach (var gameCards in BattleSystem.GameUIPresenter.GameCharacterCardDisplays)
        {
            gameCards.OnClick += BattleSystem.OnUnitStatementButton;
        }
        BattleSystem.FieldController.InvokeActionOnField(AddOnCellsClick);
    }

    private void RemoveUnitStatementAction()
    {
        foreach (var gameCards in BattleSystem.GameUIPresenter.GameCharacterCardDisplays)
        {
            gameCards.OnClick -= BattleSystem.OnUnitStatementButton;
        }
        BattleSystem.FieldController.InvokeActionOnField(RemoveOnCellsClick);
    }

    private void AddOnCellsClick(Cell cell)
    {
        cell.OnClick += BattleSystem.OnMoveButton;
    }

    private void RemoveOnCellsClick(Cell cell)
    {
        cell.OnClick -= BattleSystem.OnMoveButton;
    }

    public override IEnumerator UnitStatement(GameObject character)
    {
        GameCharacterCardDisplay cardDisplay = character.GetComponent<GameCharacterCardDisplay>();        
        BattleSystem.GameUIPresenter.SetChosenCard(cardDisplay);
        BattleSystem.FieldController.TurnOffCells();
        BattleSystem.FieldController.InvokeActionOnField(SetActiveCells);
        yield break;
    }

    private void SetActiveCells(Cell cell)
    {
        if (cell.CellIndex.y == BattleSystem.FieldController.CellsOfFieled.GetLength(1) - 1 || cell.CellIndex.y == BattleSystem.FieldController.CellsOfFieled.GetLength(1) - 2)
        {
            cell.SetCellMovable();
        }
    }

    public override IEnumerator Move(GameObject cell)
    {
        if (cell.transform.childCount == 1)
        {
           
            GameCharacterCardDisplay cardDisplay = BattleSystem.GameUIPresenter.GetChosenCard();           
            if (cardDisplay!=null)
            {
                cardDisplay.IsCharacterSpawned = true;
                cardDisplay.SetCharacter(BattleSystem.InstasiatePlayerCharacter(cardDisplay.CurrentCharacterCard, cell.transform));               
            }
            else
            {
                Debug.LogError("��� ��������� �����");
            }
        }       
        yield break;
    }
}
