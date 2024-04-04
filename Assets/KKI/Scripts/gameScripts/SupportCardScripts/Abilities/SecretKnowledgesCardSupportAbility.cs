using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SecretKnowledgesCardSupportAbility : BaseSupportÑardAbility
{
    [SerializeField]
    private int pointsOfUsingAbility;
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
            foreach (var character in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                character.UseAbilityCost = pointsOfUsingAbility;
            }
        }
        else
        {
            foreach (var character in battleSystem.EnemyController.EnemyCharObjects)
            {
                character.UseAbilityCost = pointsOfUsingAbility;
            }
        }
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }
}
