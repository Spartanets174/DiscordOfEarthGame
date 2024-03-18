using System;
using UnityEngine;

[Serializable]
public class TeleportSupportCardAbility : BaseSupportÑardAbility
{
    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
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
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.OnClick += SetCellsToMove;
                playerCharacter.OnClick += SelectCharacter;
            }
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
        battleSystem.EnemyController.SetEnemiesState(true);
        foreach (var cell in setAbiableCellsBehaviour.cellsToMove)
        {
            cell.OnClick += UseCard;
        }
    }

    private void OnCardUse()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
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
