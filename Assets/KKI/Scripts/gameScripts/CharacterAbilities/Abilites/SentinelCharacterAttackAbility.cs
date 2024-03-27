using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SentinelCharacterAttackAbility : BaseCharacterAbility
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private int range;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;
    private FormulaAttackSelected—haractersBehaviour attackSelected—haractersBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("",battleSystem, abilityOwner, range, "attack"));
        SetUseCardBehaviour(new FormulaAttackSelected—haractersBehaviour(damage, battleSystem, abilityOwner, "\"“ˇÊÂÎ‡ˇ ÛÍ‡\""));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;
        attackSelected—haractersBehaviour = (FormulaAttackSelected—haractersBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }
    private void OnSelected()
    {
        attackSelected—haractersBehaviour.characters= selectCellsToAttackInRangeBehaviour.charactersOnCells;
        foreach (var character in attackSelected—haractersBehaviour.characters)
        {
            character.PhysDefence--;
        }
        UseCard(abilityOwner.gameObject);       
    }


    private void OnCardUse()
    {
        selectCellsToAttackInRangeBehaviour.charactersOnCells.Clear();
        attackSelected—haractersBehaviour.characters.Clear();
    }
}
