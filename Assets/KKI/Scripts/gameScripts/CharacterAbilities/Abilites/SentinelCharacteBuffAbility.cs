using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SentinelCharacteBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float magDefenceAmount;

    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private float healAmount;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("�������� �������� ��������� ��� �����", battleSystem));
        SetSelectCharacterBehaviour(new EmptySelectCharacterBehaviour(""));

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
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
            battleSystem.PlayerController.CurrentPlayerCharacter.PhysDefence += physDefenceAmount;
            battleSystem.PlayerController.CurrentPlayerCharacter.MagDefence += magDefenceAmount;
            battleSystem.PlayerController.CurrentPlayerCharacter.Heal(healAmount);
        }
        else
        {
            battleSystem.EnemyController.CurrentEnemyCharacter.PhysDefence += physDefenceAmount;
            battleSystem.EnemyController.CurrentEnemyCharacter.MagDefence += magDefenceAmount;
            battleSystem.EnemyController.CurrentEnemyCharacter.Heal(healAmount);

        }

        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectCharacter;
        });

        UseCard(null);
    }

    private void OnCancelSelection()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectCharacter;
        }
    }

}