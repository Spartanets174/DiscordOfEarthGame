using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GargoyleCharacterAttackAbility : BaseCharacterAbility
{
    [SerializeField]
    private Vector2 damageSpread;

    [SerializeField]
    private int range;

    private SelectCellsToAttackInRangeBehaviour selectCellsToAttackInRangeBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsToAttackInRangeBehaviour("Нажмите на персонажа в красной клетке для атаки", battleSystem, abilityOwner, range));
        SetUseCardBehaviour(new AttackSelectedСharacterBehaviour(UnityEngine.Random.Range(damageSpread.x, damageSpread.y), battleSystem, "\"Дыхание огнём\""));

        selectCellsToAttackInRangeBehaviour = (SelectCellsToAttackInRangeBehaviour)CardSelectBehaviour;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.enemiesToAttack)
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
        foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.enemiesToAttack)
        {
            enemyCharacter.OnClick -= UseCard;
        }
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }
}

