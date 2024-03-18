using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CleansingSecondSupportCardAbility : BaseSupportÑardAbility
{
    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;

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
