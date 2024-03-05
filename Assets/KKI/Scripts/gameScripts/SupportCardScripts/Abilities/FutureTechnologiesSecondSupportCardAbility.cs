using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FutureTechnologiesSecondSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
{
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private Character character;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Âûáåðèòå ïåðñîíàæà", battleSystem));
        SetSelectCharacterBehaviour(new EmptySelectCharacterBehaviour(""));

        m_isBuff = true;
        TurnCount = 2;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
    }

    private void OnDestroy()
    {
        m_cardSelectBehaviour.OnCancelSelection -= OnCancelSelection;
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter -= OnSelectCharacter;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.OnClick += SelectCharacter;
            }
        }
        
    }
    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }
        else
        {
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }
            

        character.MagDefence += 2;
        character.PhysDefence += 2;

        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectCharacter;
            playerCharacter.IsChosen = false;
        }
        UseCard(null);
    }

    private void OnCancelSelection()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectCharacter;
        }
    }

    public void ReturnToNormal()
    {
        character.MagDefence -= 2;
        character.PhysDefence -= 2;

        OnReturnToNormal?.Invoke(this);
    }

}