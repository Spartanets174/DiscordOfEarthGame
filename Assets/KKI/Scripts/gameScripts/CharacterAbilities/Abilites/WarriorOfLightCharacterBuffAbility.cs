using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WarriorOfLightCharacterBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float healAmount;

    [SerializeField]
    private float physDamageAmount;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("Используйте карту"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        abilityOwner.OnDeath += OnDeath;
        

        

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);
    }

    private void OnDeath(Character character)
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            if (playerCharacter.Class == Enums.Classes.Паладин)
            {
                playerCharacter.PhysAttack += physDamageAmount;

                if (playerCharacter.Health == playerCharacter.MaxHealth)
                {
                    playerCharacter.MaxHealth += healAmount;
                }
                playerCharacter.Heal(healAmount);
            }
        }
    }
}
