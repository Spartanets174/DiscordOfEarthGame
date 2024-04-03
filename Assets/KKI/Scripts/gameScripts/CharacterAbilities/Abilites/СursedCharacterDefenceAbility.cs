using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class СursedCharacterDefenceAbility : BaseCharacterAbility, ITurnCountable
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
        SetCardSelectBehaviour(new EmptySelectBehaviour("Используйте карту"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        abilityOwner.IgnoreMagDamage = true;

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);

    }

    public void ReturnToNormal()
    {
        abilityOwner.IgnoreMagDamage = false;
        OnReturnToNormal?.Invoke(this);
    }

}
