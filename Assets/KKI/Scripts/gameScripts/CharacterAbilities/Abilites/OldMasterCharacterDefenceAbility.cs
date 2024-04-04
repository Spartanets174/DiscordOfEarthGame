using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OldMasterCharacterDefenceAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float physAttackAmount;
    [SerializeField]
    private float physDefenceAmount;

    private Character character;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите союзного персонажа для услиения", battleSystem));
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

        character.PhysAttack += physAttackAmount;
        character.PhysDefence -= physDefenceAmount;


        OnCancelSelection();
        UseCard(character.gameObject);
    }


    private void OnCancelSelection()
    {
        battleSystem.EnemyController.SetEnemiesStates(true, false);
        battleSystem.PlayerController.SetPlayerStates(true, false, x =>
        {
            x.OnClick -= SelectCharacter;
        });
    }

    public void ReturnToNormal()
    {
        character.PhysAttack -= physAttackAmount;
        character.PhysDefence += physDefenceAmount;

        OnReturnToNormal?.Invoke(this);
        character = null;
    }
}
