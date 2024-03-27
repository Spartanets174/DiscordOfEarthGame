using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProtectedDonkeyCharacterAttackAbility : BaseCharacterAbility
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private int range;

    private SelectCellsInRangeBehaviour selectCellsInRangeBehaviour;
    private FormulaAttackEnemyCharactersInAreaBehaviour formulaAttackAllCharactersInAreaBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsInRangeBehaviour("�������� ������� ��� �����", battleSystem, abilityOwner, new Vector2(3, 2), range, "attack"));
        SetUseCardBehaviour(new FormulaAttackEnemyCharactersInAreaBehaviour(damage, battleSystem, abilityOwner, "\"������� ��!\""));

        selectCellsInRangeBehaviour = (SelectCellsInRangeBehaviour)CardSelectBehaviour;
        formulaAttackAllCharactersInAreaBehaviour = (FormulaAttackEnemyCharactersInAreaBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }
    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            formulaAttackAllCharactersInAreaBehaviour.cellsToAttack = selectCellsInRangeBehaviour.highlightedCells.Where(x => x.GetComponentInChildren<Enemy>() != null).ToList();
        }
        else
        {
            formulaAttackAllCharactersInAreaBehaviour.cellsToAttack = selectCellsInRangeBehaviour.highlightedCells.Where(x => x.GetComponentInChildren<PlayerCharacter>() != null).ToList();
        }     
        if (formulaAttackAllCharactersInAreaBehaviour.cellsToAttack.Count == 0)
        {
            SelectCard();
        }
        else
        {
            UseCard(abilityOwner.gameObject);
        }
    }


    private void OnCardUse()
    {
        selectCellsInRangeBehaviour.highlightedCells.Clear();
        formulaAttackAllCharactersInAreaBehaviour.cellsToAttack.Clear();
    }
}