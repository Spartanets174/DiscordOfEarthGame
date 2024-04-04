using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoidShooterCharacterDefenceAbility : BaseCharacterAbility
{
    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private float magDefenceAmount;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour(""));
        SetUseCardBehaviour(new EmptyUseAbilityBehaviour());

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        abilityOwner.PhysDefence += physDefenceAmount;
        abilityOwner.MagDefence += magDefenceAmount;
        UseCard(abilityOwner.gameObject);
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {

    }
}

