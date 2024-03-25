using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FearlessGuardianCharacterDefenceAbility : BaseCharacterAbility
{
    [SerializeField]
    private float magDefenceAmount;

    [SerializeField]
    private float physDefenceAmount;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите союзного персонажа для дебаффа", battleSystem));
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
            battleSystem.PlayerController.CurrentPlayerCharacter.PhysDefence -= physDefenceAmount;
            battleSystem.PlayerController.CurrentPlayerCharacter.MagDefence -= magDefenceAmount;

        }
        else
        {
            battleSystem.EnemyController.CurrentEnemyCharacter.PhysDefence -= physDefenceAmount;
            battleSystem.EnemyController.CurrentEnemyCharacter.MagDefence -= magDefenceAmount;
        }

        abilityOwner.PhysDefence += physDefenceAmount;
        abilityOwner.MagDefence += magDefenceAmount;

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