using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DarknessSecondSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
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
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Âûáåðèòå ñîþçíîãî ïåðñîíàæà", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Âûáåðèòå âðàæåñêîãî ïåðñîíàæà", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));

        m_isBuff = true;
        TurnCount = 1;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_cardSecondSelectBehaviour.OnSelected += OnSecondSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
    }


    private void OnDestroy()
    {
        m_cardSelectBehaviour.OnCancelSelection -= OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection -= OnCancelSelection;
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        m_cardSecondSelectBehaviour.OnSelected -= OnSecondSelected;
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
        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectSecondCharacterInvoke;
        });

        battleSystem.EnemyController.SetEnemiesState(true);

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
        battleSystem.PlayerController.SetPlayerState(true, x =>
        {
            x.IsEnabled = true;
            x.OnClick -= SelectSecondCharacterInvoke;
        });

        battleSystem.EnemyController.SetEnemiesState(true, (x) => {
            x.IsChosen = false;
            x.OnClick -= SelectCharacter;
        });
    }

    public void ReturnToNormal()
    {
        character.MagAttack -= 2;
        enemyCharacter.MagDefence += 1;

        OnReturnToNormal?.Invoke(this);
    }
}