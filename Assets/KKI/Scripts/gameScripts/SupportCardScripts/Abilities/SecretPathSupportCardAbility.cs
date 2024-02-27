using System;
using System.Collections.Generic;
using UnityEngine;

public class SecretPathSupportCardAbility : BaseSupportСardAbility
{
    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите персонажа", battleSystem));
        SetSelectCharacterBehaviour(new SetAbiableCellsBehaviour("Выберите клетку для перемещения", battleSystem));
        SetUseCardBehaviour(new MoveToCellBehaviour(battleSystem));

        setAbiableCellsBehaviour = (SetAbiableCellsBehaviour)SelectCharacterBehaviour;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnCancelSelection()
    {
        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }
    }

    private void OnSelected()
    {
        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick += SetCellsToMove;
            playerCharacter.OnClick += SelectCharacter;
        }
        OnSupportCardAbilitySelectedInvoke();
    }

    private void SetCellsToMove(GameObject @object)
    {
        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= UseCard;
        }
        setAbiableCellsBehaviour.cellsToMove = battleSystem.GetCellsForMove(@object.GetComponent<Character>(), 3);
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
        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }

        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= UseCard;
        }
        OnSupportCardAbilityUsedInvoke();
    }
}
