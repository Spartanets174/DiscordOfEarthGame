using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSupportCardAbility : BaseSupportÑardAbility
{
    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Âûáåðèòå ïåðñîíàæà", battleSystem));
        SetSelectCharacterBehaviour(new SetAbiableCellsBehaviour("Âûáåðèòå êëåòêó äëÿ ïåðåìåùåíèÿ", battleSystem));
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
    }

    private void SetCellsToMove(GameObject @object)
    {
        foreach (var cell in setAbiableCellsBehaviour.cellsToMove)
        {
            cell.OnClick -= UseCard;
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
    }

    private void OnCardUse()
    {
        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }

        foreach (var cell in setAbiableCellsBehaviour.cellsToMove)
        {
            cell.OnClick -= UseCard;
        }
    }
}
