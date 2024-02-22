using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AthleticTrainingSecondSupportCardAbility : BaseSupportÑardAbility
{
    protected override void Start()
    {
        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàğòó"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()        
    {
        battleSystem.PointsOfAction += 6;
        OnSupportCardAbilityUsedInvoke();
        m_cardSelectBehaviour.OnSelected -= OnSelected;
    }

   
}
