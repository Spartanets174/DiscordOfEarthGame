using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MagicToadCharacterBuffAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float healthAmount;

    [SerializeField]
    private float magDefenceAmount;

    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }


    private float healAmount;
    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("Используйте карту"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        abilityOwner.PhysDefence += physDefenceAmount;
        abilityOwner.MagDefence += magDefenceAmount;
        abilityOwner.Heal(healthAmount);
        healAmount = abilityOwner.LastHealAmount;

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);
    }

    public void ReturnToNormal()
    {
        abilityOwner.PhysDefence -= physDefenceAmount;
        abilityOwner.MagDefence -= magDefenceAmount;
        abilityOwner.Damage(healAmount, "Склизкая оболочка");
        OnReturnToNormal?.Invoke(this);
    }
}
