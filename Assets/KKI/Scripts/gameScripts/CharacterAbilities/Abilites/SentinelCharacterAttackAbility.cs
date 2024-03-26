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

    private SelectCellsToAttackInRangeBehaviour selectCellsToAttackInRangeBehaviour;
    private AttackSelected—haractersBehaviour attackSelected—haractersBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsToAttackInRangeBehaviour("",battleSystem, abilityOwner, range));
        SetUseCardBehaviour(new AttackSelected—haractersBehaviour(damage, battleSystem, "\"“ˇÊÂÎ‡ˇ ÛÍ‡\""));

        selectCellsToAttackInRangeBehaviour = (SelectCellsToAttackInRangeBehaviour)CardSelectBehaviour;
        attackSelected—haractersBehaviour = (AttackSelected—haractersBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }
    private void OnSelected()
    {
        attackSelected—haractersBehaviour.charactersToAttack= selectCellsToAttackInRangeBehaviour.enemiesToAttack;
        foreach (var character in attackSelected—haractersBehaviour.charactersToAttack)
        {
            character.PhysDefence--;
        }
        UseCard(abilityOwner.gameObject);       
    }


    private void OnCardUse()
    {
        selectCellsToAttackInRangeBehaviour.enemiesToAttack.Clear();
        attackSelected—haractersBehaviour.charactersToAttack.Clear();
    }
}
