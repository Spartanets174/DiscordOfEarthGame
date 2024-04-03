using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class SelfTaughtCharacterDefenceAbility : BaseCharacterAbility, ITurnCountable
{
   

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private Character character;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите союзного персонажа для баффа", battleSystem));
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
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }
        else
        {
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }

        character.CanDamage = false;
        character.CanBeDamaged = false;

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

    public void ReturnToNormal()
    {
        character.CanDamage = true;
        character.CanBeDamaged = true;
    }
}