using System;
using UnityEngine;

[Serializable]
public class AthleticTrainingSupportCardAbility : BaseSupport�ardAbility
{
    private AthleticTrainingSupportCardAbilityData abilityData;
    public override void Init(BattleSystem battleSystem, BaseSupport�ardAbilityData baseAbilityData)
    {
       this.battleSystem = battleSystem;
        abilityData = (AthleticTrainingSupportCardAbilityData)baseAbilityData;

        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        battleSystem.PointsOfAction.Value += abilityData.pointsOfAction;
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }
}
[Serializable]
public class AthleticTrainingSupportCardAbilityData : BaseSupport�ardAbilityData
{
    public int pointsOfAction;
}