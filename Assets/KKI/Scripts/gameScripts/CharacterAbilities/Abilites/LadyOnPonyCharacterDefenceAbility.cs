using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LadyOnPonyCharacterDefenceAbility : BaseCharacterAbility
{
    [SerializeField]
    private int range;

    [SerializeField]
    private float physDefenceAmount;


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
