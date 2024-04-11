using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SecretKnowledgesCardSupportAbility : BaseSupport�ardAbility
{
    private SecretKnowledgesCardSupportAbilityData abilityData;
    public override void Init(BattleSystem battleSystem, BaseSupport�ardAbilityData baseAbilityData)
    {
        this.battleSystem = battleSystem;
        abilityData = (SecretKnowledgesCardSupportAbilityData)baseAbilityData;
        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var character in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                character.UseAbilityCost = abilityData.pointsOfUsingAbility;
            }
        }
        else
        {
            foreach (var character in battleSystem.EnemyController.EnemyCharObjects)
            {
                character.UseAbilityCost = abilityData.pointsOfUsingAbility;
            }
        }
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }
}
[Serializable]
public class SecretKnowledgesCardSupportAbilityData : BaseSupport�ardAbilityData
{
    public int pointsOfUsingAbility;
}