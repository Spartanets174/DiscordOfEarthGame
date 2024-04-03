using System;
using UnityEngine;

[Serializable]
public class FallenElfCharacterAttackAbility : BaseCharacterAbility
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private int range;

    [SerializeField]
    private float healPercent;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;
    private FormulaAttackSelected�haracterBehaviour formulaAttackSelected�haracterBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("������� �� ��������� � ������� ������ ��� �����", battleSystem, abilityOwner, range, "attack"));
        SetUseCardBehaviour(new FormulaAttackSelected�haracterBehaviour(damage, battleSystem, abilityOwner, "\"�����\""));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;
        formulaAttackSelected�haracterBehaviour = (FormulaAttackSelected�haracterBehaviour)UseCardBehaviour;

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
        abilityOwner.Heal(formulaAttackSelected�haracterBehaviour.attackedCharacter.LastDamageAmount * healPercent);
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            enemyCharacter.OnClick -= UseCard;
        }
        selectCellsToAttackInRangeBehaviour.charactersDirectionsOnCells.Clear();
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }
}