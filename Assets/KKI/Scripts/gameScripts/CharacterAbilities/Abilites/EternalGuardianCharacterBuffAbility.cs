using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EternalGuardianCharacterBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float healAmount;

    private List<Character> characters = new();

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        characters.Clear();
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsWithConditionBehaviour("Выберите союзного персонажа для лечения", battleSystem, CanBeHealed));
        SetSecondCardSelectBehaviour(new SelectAllPlayerUnitsWithConditionBehaviour("Выберите второго союзного персонажа для лечения", battleSystem, CanBeHealed));
        SetSelectCharacterBehaviour(new EmptySelectCharacterBehaviour(""));
        SetUseCardBehaviour(new EmptyUseAbilityBehaviour());

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_cardSecondSelectBehaviour.OnSelected += OnSecondSelected;

        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection += OnCancelSelection;
    }



    private bool CanBeHealed(Character character)
    {
        return character.Health != character.MaxHealth;
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
        characters.Add(gameObject.GetComponent<Character>());

        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectSecondCharacterInvoke;
        });

        SelectSecondCard();
    }
    private void OnSecondSelected()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick += SelectCharacter;
        }
    }

    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            characters.Add(battleSystem.PlayerController.CurrentPlayerCharacter);
        }
        else
        {
            characters.Add(battleSystem.EnemyController.CurrentEnemyCharacter);
        }

        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectCharacter;
        });

        if (characters.Count == 2)
        {
            foreach (var character in characters)
            {
                if (character != null)
                {
                    character.Heal(healAmount);
                }
            }
        }
        UseCard(abilityOwner.gameObject);
    }

    private void OnCardUse()
    {
        Uncubscribe();
    }

    private void OnCancelSelection()
    {
        Uncubscribe();
    }

    private void Uncubscribe()
    {
        characters.Clear();
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectSecondCharacterInvoke;
            playerCharacter.OnClick -= SelectCharacter;
        }

    }

}
[Serializable]
public class EternalGuardianCharacterBuffAbilityData : BaseCharacterAbilityData
{
    public float healAmount;
}