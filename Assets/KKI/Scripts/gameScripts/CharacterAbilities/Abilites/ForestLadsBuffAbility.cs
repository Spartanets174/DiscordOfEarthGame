using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ForestLadsBuffAbility : BaseCharacterAbility, ITurnCountable
{
    public int TurnCount { get => abilityData.turnCount; set => abilityData.turnCount = value; }

    public bool IsBuff { get => abilityData.isBuff; }

    private ForestLadsBuffAbilityData abilityData;

    public event Action<ITurnCountable> OnReturnToNormal;
    public override void Init(BattleSystem battleSystem, Character owner, BaseCharacterAbilityData characterAbilityData)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        abilityData = (ForestLadsBuffAbilityData)characterAbilityData;
        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.Range += abilityData.attackRange;
        }

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);
    }

    public void ReturnToNormal()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.Range -= abilityData.attackRange;
        }
        OnReturnToNormal?.Invoke(this);
    }
}
[Serializable]
public class ForestLadsBuffAbilityData : BaseCharacterAbilityData
{
    public int attackRange;

    public int turnCount;

    [Header("�� �������!!!")]
    public bool isBuff;
}