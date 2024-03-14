using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleansingSecondSupportCardAbility : BaseSupport�ardAbility
{
    protected override void Start()
    {
        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));

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
