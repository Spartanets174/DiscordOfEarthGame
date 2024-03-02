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
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.SetStatsToNormal();
        }
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }

}
