using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OldMasterCharacterBuffbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float healthIcnreaseAmount;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private List<Character> characterList = new List<Character>();

    public event Action<ITurnCountable> OnReturnToNormal;

    private float increaseAmount;
    private float healAmount;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("Используйте карту"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {

        SetCharacters();
        foreach (var character in characterList)
        {
            increaseAmount = (character.MaxHealth * (1 + healthIcnreaseAmount)) - character.MaxHealth;
            character.MaxHealth = character.MaxHealth * (1 + healthIcnreaseAmount);

            if (character.Health == character.MaxHealth)
            {
                character.Heal(character.MaxHealth);
                healAmount = increaseAmount;
            }
            else
            {
                healAmount = character.Health * healthIcnreaseAmount;
                character.Heal(healAmount);
            }
        }



        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);
    }

    private void SetCharacters()
    {
        while (characterList.Count < 2)
        {
            Character character;
            if (battleSystem.State is PlayerTurn)
            {
                character = battleSystem.PlayerController.PlayerCharactersObjects[UnityEngine.Random.Range(0, battleSystem.PlayerController.PlayerCharactersObjects.Count)];
                if (!characterList.Contains(character))
                {
                    characterList.Add(character);
                }
            }
            else
            {
                character = battleSystem.EnemyController.EnemyCharObjects[UnityEngine.Random.Range(0, battleSystem.EnemyController.EnemyCharObjects.Count)];
                if (!characterList.Contains(character))
                {
                    characterList.Add(character);
                }
            }
        }
    }

    public void ReturnToNormal()
    {
        foreach (var character in characterList)
        {
            character.MaxHealth = character.Card.health;
            if (character.Health == character.MaxHealth)
            {
                character.Damage(increaseAmount, "\"Мы одна кровь\"");
            }
            else
            {
                character.Damage(healAmount, "\"Мы одна кровь\"");
            }
        }

        characterList.Clear();

        OnReturnToNormal?.Invoke(this);
    }

}
