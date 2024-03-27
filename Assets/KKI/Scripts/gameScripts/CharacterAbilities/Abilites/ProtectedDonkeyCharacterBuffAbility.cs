using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProtectedDonkeyCharacterBuffAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private float magDefenceAmount;

    [SerializeField]
    private float rangeAmount;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private List<Character> characterList = new List<Character>();

    private bool hasPeople = false;
    private bool hasGnomes = false;
    private bool hasElfs = false;
    private bool hasDarkElfs = false;
    private bool hasMagicCreatures = false;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("»спользуйте карту"));

        hasPeople = false;
        hasGnomes = false;
        hasElfs = false;
        hasDarkElfs = false;
        hasMagicCreatures = false;

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                AddToCharacterList(playerCharacter);
            }
        }
        else
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                AddToCharacterList(enemyCharacter);
            }
        }

        foreach (var character in characterList)
        {
            character.MaxSpeed += rangeAmount;
            character.PhysDefence += physDefenceAmount;
            character.MagDefence += magDefenceAmount;
        }



        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);
    }


    private void AddToCharacterList(Character character)
    {
        switch (character.Race)
        {
            case Enums.Races.Ћюди:
                if (!hasPeople)
                {
                    characterList.Add(character);
                    hasPeople = true;
                }
                break;
            case Enums.Races.√номы:
                if (!hasGnomes)
                {
                    characterList.Add(character);
                    hasGnomes = true;
                }
                break;
            case Enums.Races.Ёльфы:
                if (!hasElfs)
                {
                    characterList.Add(character);
                    hasElfs = true;
                }
                break;
            case Enums.Races.“ЄмныеЁльфы:
                if (!hasDarkElfs)
                {
                    characterList.Add(character);
                    hasDarkElfs = true;
                }
                break;
            case Enums.Races.ћагические—ущества:
                if (!hasMagicCreatures)
                {
                    characterList.Add(character);
                    hasMagicCreatures = true;
                }
                break;
        }
    }

    public void ReturnToNormal()
    {
        foreach (var character in characterList)
        {
            character.MaxSpeed -= rangeAmount;
            character.PhysDefence -= physDefenceAmount;
            character.MagDefence -= magDefenceAmount;
        }

        hasPeople = false;
        hasGnomes = false;
        hasElfs = false;
        hasDarkElfs = false;
        hasMagicCreatures = false;

        characterList.Clear();

        OnReturnToNormal?.Invoke(this);
    }

}
