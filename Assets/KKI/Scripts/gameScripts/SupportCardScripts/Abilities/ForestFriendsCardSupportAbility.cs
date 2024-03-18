using System;
using UnityEngine;

[Serializable]
public class ForestFriendsCardSupportAbility : BaseSupportÑardAbility
{
    [SerializeField]
    private int transitionCost;
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
            battleSystem.FieldController.InvokeActionOnField((x) =>
            {
                if (x.IsSwamp)
                {
                    x.TransitionCostEnemy += transitionCost;
                }
            });
        }
        else
        {
            battleSystem.FieldController.InvokeActionOnField((x) =>
            {
                if (x.IsSwamp)
                {
                    x.TransitionCostPlayer += transitionCost;
                }
            });
        }

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }
}
