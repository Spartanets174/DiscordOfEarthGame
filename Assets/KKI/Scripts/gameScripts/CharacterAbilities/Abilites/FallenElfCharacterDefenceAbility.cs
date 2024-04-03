using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FallenElfCharacterDefenceAbility : BaseCharacterAbility, ITurnCountable
{
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

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);

    }

    public void ReturnToNormal()
    {
        abilityOwner.PhysDefence -= physDefenceAmount;
        abilityOwner.MagDefence -= magDefenceAmount;

        OnReturnToNormal?.Invoke(this);
    }

}
