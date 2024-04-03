using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SergeantMajorCharacterDefenceAbility : BaseCharacterAbility
{
    [SerializeField]
    private float magDefenceAmount;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("Используйте карту"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        abilityOwner.MagDefence += magDefenceAmount;

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);

    }

}
