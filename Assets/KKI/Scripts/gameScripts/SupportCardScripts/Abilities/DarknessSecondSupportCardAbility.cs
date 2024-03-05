using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DarknessSecondSupportCardAbility : BaseSupportСardAbility, ITurnCountable
{
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private Character character;

    private Character enemyCharacter;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите союзного персонажа", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Выберите вражеского персонажа", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));

        m_isBuff = true;
        TurnCount = 1;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_cardSelectBehaviour.OnSelected += OnSecondSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
    }


    private void OnDestroy()
    {
        m_cardSelectBehaviour.OnCancelSelection -= OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection -= OnCancelSelection;
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        m_cardSelectBehaviour.OnSelected -= OnSecondSelected;
        m_selectCharacterBehaviour.OnSelectCharacter -= OnSelectCharacter;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.OnClick += SelectSecondCharacterInvoke;
            }

        }
    }

    private void SelectSecondCharacterInvoke(GameObject gameObject)
    {
        if (battleSystem.State is PlayerTurn)
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }
        else
        {
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }

        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectSecondCharacterInvoke;
            playerCharacter.IsChosen = false;
        }
        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.IsEnabled = true;
        }
        SelectSecondCard();
    }

    private void OnSecondSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                enemyCharacter.OnClick += SelectCharacter;
            }
        }
    }

    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            enemyCharacter = battleSystem.EnemyController.CurrentEnemyCharacter;
        }
        else
        {
            enemyCharacter = battleSystem.PlayerController.CurrentPlayerCharacter;
        }

        OnCancelSelection();

        character.MagAttack += 2;
        enemyCharacter.MagDefence -= 1;

        UseCard(null);
    }

    private void OnCancelSelection()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.IsChosen = false;
            playerCharacter.IsEnabled = true;
            playerCharacter.OnClick -= SelectSecondCharacterInvoke;
        }
        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.IsChosen = false;
            enemyCharacter.IsEnabled = true;
            enemyCharacter.OnClick -= SelectCharacter;
        }
    }

    public void ReturnToNormal()
    {
        character.MagAttack -= 2;
        enemyCharacter.MagDefence += 1;

        OnReturnToNormal?.Invoke(this);
    }
}