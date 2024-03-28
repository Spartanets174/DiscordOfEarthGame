using System;
using UnityEngine;

[Serializable]
public class LadyOnPonyCharacterBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private float magDefenceAmount;

    private Character character;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        character = null;
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsWithConditionBehaviour("Âûáåðèòå ñîþçíîãî ïåðñîíàæà äëÿ óñèëåíèÿ", battleSystem, CanBeBuffed));
        SetUseCardBehaviour(new FormulaAttackSelectedÑharacterBehaviour(damage, battleSystem, abilityOwner, "\"Óâå÷üÿ\""));

        m_cardSelectBehaviour.OnSelected += OnSelected;

        m_useCardBehaviour.OnCardUse += OnCardUse;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
    }



    private bool CanBeBuffed(Character character)
    {
        return character.Health > 1;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.OnClick += UseCard;
            }
        }
    }

    private void OnCardUse()
    {
        if (battleSystem.State is PlayerTurn)
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }
        else
        {
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }

        character.MagDefence += magDefenceAmount;
        character.PhysDefence += physDefenceAmount;
        Uncubscribe();
    }

    private void OnCancelSelection()
    {
        Uncubscribe();
    }

    private void Uncubscribe()
    {
        character = null;
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= UseCard;
        }

    }

}