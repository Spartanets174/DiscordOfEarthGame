using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class DarkElfArcherCharacterAttackAbility : BaseCharacterAbility
{
    [SerializeField]
    private int range;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("Нажмите на персонажа в красной клетке для атаки", battleSystem, abilityOwner, range, "attack"));
        SetUseCardBehaviour(new FormulaAttackSelectedСharacterBehaviour(abilityOwner.PhysAttack, battleSystem, abilityOwner, "\"Зоркий глаз\""));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
            {
                enemyCharacter.OnClick += UseCard;
            }
        }
    }


    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            enemyCharacter.OnClick -= UseCard;
        }
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }
}

