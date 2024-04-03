using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SelfTaughtCharacterAttackAbility : BaseCharacterAbility
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private int range;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;
    private FormulaAttackSelectedÑharacterBehaviour formulaAttackSelectedÑharacterBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("Íàæìèòå íà ïåğñîíàæà â êğàñíîé êëåòêå äëÿ àòàêè", battleSystem, abilityOwner, range, "attack"));
        SetUseCardBehaviour(new FormulaAttackSelectedÑharacterBehaviour(damage, battleSystem, abilityOwner, "\"Ñìåğòåëüíàÿ äğîáü\""));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;
        formulaAttackSelectedÑharacterBehaviour = (FormulaAttackSelectedÑharacterBehaviour)UseCardBehaviour;

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
        selectCellsToAttackInRangeBehaviour.charactersDirectionsOnCells.Clear();
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }
}