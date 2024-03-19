using System;
using UnityEngine;

[Serializable]
public class AthleticTrainingSupportCardAbility : BaseSupportÑardAbility
{
    [SerializeField]
    private int pointsOfAction;
    public override void Init(BattleSystem battleSystem)
    {
       this.battleSystem = battleSystem;

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàğòó"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        battleSystem.PointsOfAction.Value += pointsOfAction;
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }
}
