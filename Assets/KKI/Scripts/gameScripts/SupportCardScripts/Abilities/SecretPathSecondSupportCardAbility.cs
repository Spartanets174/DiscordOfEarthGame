using System.Collections.Generic;
using UnityEngine;

public class SecretPathSecondSupportCardAbility : BaseSupportСardAbility
{
    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите персонажа", battleSystem));
        SetSelectCharacterBehaviour(new SetAbiableCellsBehaviour("Выберите клетку для перемещения", battleSystem));
        SetUseCardBehaviour(new MoveToCellBehaviour(battleSystem));

        setAbiableCellsBehaviour = (SetAbiableCellsBehaviour)SelectCharacterBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            PlayerTurn playerTurn = (PlayerTurn)battleSystem.State;
            playerTurn.OnPlayerTurnCompleted();
        }
        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick += SetCellsToMove;
            playerCharacter.OnClick += SelectCharacter;
        }
        OnSupportCardAbilitySelectedInvoke();
    }

    private void SetCellsToMove(GameObject gameObject)
    {
        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= UseCard;
        }
        setAbiableCellsBehaviour.cellsToMove = battleSystem.GetCellsForMove(gameObject.GetComponent<Character>(), 5);
    }
    private void OnSelectCharacter()
    {
        
        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.IsEnabled = true;
        }
        

        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick += UseCard;
        }
        OnSupportCardAbilityCharacterSelectedInvoke();
    }

    private void OnCardUse()
    {
        if (battleSystem.State is PlayerTurn)
        {
            PlayerTurn playerTurn = (PlayerTurn)battleSystem.State;
            playerTurn.OnPlayerTurnStarted();
            battleSystem.CurrentPlayerCharacter.IsChosen = false;
        }
        else
        {
            EnemyTurn enemyTurn = (EnemyTurn)battleSystem.State;
            battleSystem.EnemyController.CurrentEnemyCharacter.IsChosen = false;
        }

        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }

        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick += UseCard;
        }
        OnSupportCardAbilityUsedInvoke();
    }
}