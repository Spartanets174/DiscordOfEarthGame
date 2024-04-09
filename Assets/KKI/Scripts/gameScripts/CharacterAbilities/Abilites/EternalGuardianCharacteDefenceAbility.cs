using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EternalGuardianCharacteDefenceAbility : BaseCharacterAbility, ITurnCountable
{
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

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);

    }

    public void ReturnToNormal()
    {
        abilityOwner.PhysDefence -= physDefenceAmount;
        OnReturnToNormal?.Invoke(this);
    }

}
[Serializable]
public class EternalGuardianCharacteDefenceAbilityData
{
    public float physDefenceAmount;

    public int turnCount;

    [Header("НЕ ТРОГАТЬ")]
    public bool isBuff;
}