using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestFriendsCardSupportAbility : BaseSupport�ardAbility
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
            battleSystem.FieldController.InvokeActionOnField((x) => {
                if (x.IsSwamp)
                {
                    x.TransitionCostEnemy++;
                }               
            });
        }
        else
        {
            battleSystem.FieldController.InvokeActionOnField((x) => {
                if (x.IsSwamp)
                {
                    x.TransitionCostPlayer++;
                }
            });
        }
        
        OnSupportCardAbilityUsedInvoke();
        m_cardSelectBehaviour.OnSelected -= OnSelected;
    }
}
