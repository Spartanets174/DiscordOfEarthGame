using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleansingSecondSupportCardAbility : BaseSupportÑardAbility
{
    protected override void Start()
    {
        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàğòó"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.SetStatsToNormal();
        }
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }

}
