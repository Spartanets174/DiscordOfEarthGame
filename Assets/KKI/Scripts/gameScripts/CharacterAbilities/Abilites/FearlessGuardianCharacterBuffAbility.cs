using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FearlessGuardianCharacterBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float magDefenceAmount;

    [SerializeField]
    private float physDefenceAmount;

    private List<Character> characters = new();

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        characters.Clear();
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("�������� �������� ��������� ��� ��������", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("�������� ������� �������� ��������� ��� ��������", battleSystem));
        SetThirdCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("�������� �������� �������� ��������� ��� ��������", battleSystem));
        SetSelectCharacterBehaviour(new EmptySelectCharacterBehaviour(""));

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_cardSecondSelectBehaviour.OnSelected += OnSecondSelected;
        m_cardThirdSelectBehaviour.OnSelected += OnThirdSelected;

        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardThirdSelectBehaviour.OnCancelSelection += OnCancelSelection;
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
            playerCharacter.OnClick += SelectThirdCharacterInvoke;
        }
    }

    private void SelectThirdCharacterInvoke(GameObject gameObject)
    {
        characters.Add(gameObject.GetComponent<Character>());

        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectThirdCharacterInvoke;
        });

        SelectThirdCard();
    }

    private void OnThirdSelected()
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

        if (characters.Count == 3)
        {
            foreach (var character in characters)
            {
                if (character != null)
                {
                    character.PhysDefence += physDefenceAmount;
                    character.MagDefence += magDefenceAmount;
                }
            }
        }
        Uncubscribe();
        UseCard(null);
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
            playerCharacter.OnClick -= SelectThirdCharacterInvoke;
            playerCharacter.OnClick -= SelectSecondCharacterInvoke;
            playerCharacter.OnClick -= SelectCharacter;
        }

    }
}