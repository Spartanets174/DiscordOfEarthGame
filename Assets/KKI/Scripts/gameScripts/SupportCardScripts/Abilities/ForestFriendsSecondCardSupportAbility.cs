using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestFriendsSecondCardSupportAbility : BaseSupportÑardAbility
{
    protected override void Start()
    {
        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàðòó"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            battleSystem.FieldController.InvokeActionOnField((x) =>
            {
                if (x.IsSwamp)
                {
                    x.TransitionCostEnemy += 2;
                }
            });
        }
        else
        {
            battleSystem.FieldController.InvokeActionOnField((x) => {
                if (x.IsSwamp)
                {
                    x.TransitionCostPlayer += 2;
                }
            });
        }
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }
}
