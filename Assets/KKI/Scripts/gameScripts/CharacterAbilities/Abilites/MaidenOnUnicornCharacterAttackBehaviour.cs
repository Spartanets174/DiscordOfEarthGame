using System;
using UnityEngine;

[Serializable]
public class MaidenOnUnicornCharacterAttackBehaviour : BaseCharacterAbility
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private Vector2 area;

    [SerializeField]
    private Enums.Directions direction;

    private SelectCellsToAttackInAreaRangedBehaviour selectCellsToAttackInAreaRangedBehaviour;
    private AttackAllCharactersInAreaBehaviour attackAllCharactersInAreaBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsToAttackInAreaRangedBehaviour(battleSystem, abilityOwner, area, direction));
        SetUseCardBehaviour(new AttackAllCharactersInAreaBehaviour(damage, battleSystem, "\"Большой брат\""));

        selectCellsToAttackInAreaRangedBehaviour = (SelectCellsToAttackInAreaRangedBehaviour)CardSelectBehaviour;
        attackAllCharactersInAreaBehaviour = (AttackAllCharactersInAreaBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }
    private void OnSelected()
    {
        attackAllCharactersInAreaBehaviour.cellsToAttack = selectCellsToAttackInAreaRangedBehaviour.cells;
        UseCard(abilityOwner.gameObject);
    }


    private void OnCardUse()
    {
        selectCellsToAttackInAreaRangedBehaviour.cells.Clear();
        attackAllCharactersInAreaBehaviour.cellsToAttack.Clear();
    }
}
