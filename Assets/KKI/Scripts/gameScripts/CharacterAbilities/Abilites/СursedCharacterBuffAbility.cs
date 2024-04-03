using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class СursedCharacterBuffAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float critAmount;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private float currentCritAmount;
    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("Используйте карту"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (abilityOwner.CritChance < critAmount)
        {
            currentCritAmount = critAmount - abilityOwner.CritChance;

            abilityOwner.CritChance += currentCritAmount;          
        }
        
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);

    }

    public void ReturnToNormal()
    {
       abilityOwner.CritChance -= currentCritAmount;

        OnReturnToNormal?.Invoke(this);
    }

}
