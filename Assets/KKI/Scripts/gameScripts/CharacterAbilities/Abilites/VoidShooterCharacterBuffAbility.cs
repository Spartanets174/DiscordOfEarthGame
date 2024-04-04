using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoidShooterCharacterBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float chance;

    [SerializeField]
    private float amount;

    private Character character;

    public override void Init(BattleSystem battleSystem, Character abilityOwner)
    {
        this.abilityOwner = abilityOwner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Выберите вражеского персонажа для дебаффа", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new EmptyUseAbilityBehaviour());

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
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
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }
        else
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }

        float tempChance = UnityEngine.Random.Range(0f, 1f);
        Character tempCharacter = battleSystem.PlayerController.PlayerCharactersObjects[UnityEngine.Random.Range(0, battleSystem.PlayerController.PlayerCharactersObjects.Count)];
        if (tempChance <= chance)
        {
            character.PhysDefence -= amount;
            tempCharacter.PhysDefence += amount;
        }
        else
        {
            character.MagDefence -= amount;
            tempCharacter.MagDefence += amount;

        }

        UseCard(character.gameObject);
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        battleSystem.EnemyController.SetEnemiesStates(true, false, x =>
        {
            x.OnClick -= SelectCharacter;
        });
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }
}
