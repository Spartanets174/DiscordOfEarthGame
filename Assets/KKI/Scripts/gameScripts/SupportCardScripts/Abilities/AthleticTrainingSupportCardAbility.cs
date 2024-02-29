using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AthleticTrainingSupportCardAbility : BaseSupportÑardAbility
{
    protected override void Start()
    {
        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàğòó"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        battleSystem.PointsOfAction += 3;
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }
}
