using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CleansingSecondSupportCardAbility : BaseSupportÑardAbility
{
    private CleansingSecondSupportCardAbilityData abilityData;
    public override void Init(BattleSystem battleSystem, BaseSupportÑardAbilityData baseAbilityData)
    {
        this.battleSystem = battleSystem;
        abilityData = (CleansingSecondSupportCardAbilityData)baseAbilityData;
        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàðòó"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            battleSystem.PlayerController.RemoveDebuffsAllPlayerCharacters();
        }
        else
        {
            battleSystem.EnemyController.RemoveDebuffsAllEnemyCharacters();
        }
        
        
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }

}
[Serializable]
public class CleansingSecondSupportCardAbilityData : BaseSupportÑardAbilityData
{
    
}