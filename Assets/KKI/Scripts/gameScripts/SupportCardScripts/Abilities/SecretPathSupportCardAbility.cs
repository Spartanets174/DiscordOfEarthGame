using System;
using UnityEngine;

[Serializable]
public class SecretPathSupportCardAbility : BaseSupportСardAbility
{
    [SerializeField]
    private int cellsCount;

    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
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
        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= UseCard;
        }
        setAbiableCellsBehaviour.cellsToMove = battleSystem.FieldController.GetCellsForMove(@object.GetComponent<Character>(), cellsCount);
    }
    private void OnSelectCharacter()
    {
        battleSystem.EnemyController.SetEnemiesState(true);
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var item in setAbiableCellsBehaviour.cellsToMove)
            {
                item.OnClick += UseCard;
            }
        }


    }

    private void OnCardUse()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }

        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= UseCard;
        }
    }
}
