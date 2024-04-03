using System;
using UnityEngine;


[Serializable]
public class SergeantMajorCharacterBuffAbility : BaseCharacterAbility
{
    [SerializeField]
    private float physDamageAmount;

    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private float magDefenceAmount;


    [SerializeField]
    private float chanceToIncreaseDamage;

    private Character character;
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

        float chance = UnityEngine.Random.Range(0f, 1f);

        if (chance <= chanceToIncreaseDamage)
        {
            character.PhysAttack += physDamageAmount;
        }
        else
        {
            character.PhysDefence += physDefenceAmount;
            character.MagDefence += magDefenceAmount;

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