using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class GargoyleCharacterDefenceAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour(""));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        abilityOwner.IsFreezed = true;
        abilityOwner.CanDamage = false;
        abilityOwner.CanUseAbilities = false;
        abilityOwner.IgnorePhysDamage = true;

        UseCard(abilityOwner.gameObject);
    }


    public void ReturnToNormal()
    {
        abilityOwner.IsFreezed = false;
        abilityOwner.CanDamage = true;
        abilityOwner.CanUseAbilities = true;
        abilityOwner.IgnorePhysDamage = false;

        OnReturnToNormal?.Invoke(this);
    }
}