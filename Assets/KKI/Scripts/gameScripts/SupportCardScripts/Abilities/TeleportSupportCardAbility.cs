using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSupportCardAbility : BaseSupport�ardAbility
{
    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("�������� ���������", battleSystem));
        SetSelectCharacterBehaviour(new SetAbiableCellsBehaviour("�������� ������ ��� �����������", battleSystem));
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

    private void SetCellsToMove(GameObject @object)
    {
        foreach (var cell in setAbiableCellsBehaviour.cellsToMove)
        {
            cell.OnClick += UseCard;
        }
        setAbiableCellsBehaviour.cellsToMove.Clear();
        battleSystem.FieldController.InvokeActionOnField(x =>
        {
            if (x.transform.childCount == 0)
            {
                setAbiableCellsBehaviour.cellsToMove.Add(x);
            }
        });
    }

    private void OnSelectCharacter()
    {
        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.IsEnabled = true;
        }


        

        foreach (var cell in setAbiableCellsBehaviour.cellsToMove)
        {
            cell.OnClick += UseCard;
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

        foreach (var cell in setAbiableCellsBehaviour.cellsToMove)
        {
            cell.OnClick += UseCard;
        }
        OnSupportCardAbilityUsedInvoke();
    }
}
