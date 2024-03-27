using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PeasantWithPitchforkCharacterBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private int range;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("", battleSystem, abilityOwner, range, "allowed"));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }
    private void OnSelected()
    {
        foreach (var character in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            character.PhysDefence += physDefenceAmount;
        }
        selectCellsToAttackInRangeBehaviour.charactersOnCells.Clear();
        UseCard(abilityOwner.gameObject);
    }

}
