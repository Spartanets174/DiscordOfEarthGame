using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProtectedDonkeyCharacterBuffAbility : BaseCharacterAbility, ITurnCountable
{
    public int TurnCount { get => abilityData.turnCount; set => abilityData.turnCount = value; }
    public bool IsBuff { get => abilityData.isBuff; }

    private ProtectedDonkeyCharacterBuffAbilityData abilityData;
    private List<Character> characterList = new List<Character>();

    private bool hasPeople = false;
    private bool hasGnomes = false;
    private bool hasElfs = false;
    private bool hasDarkElfs = false;
    private bool hasMagicCreatures = false;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner, BaseCharacterAbilityData characterAbilityData)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        abilityData = (ProtectedDonkeyCharacterBuffAbilityData)characterAbilityData;
        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));

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
            character.MaxSpeed += abilityData.rangeAmount;
            character.PhysDefence += abilityData.physDefenceAmount;
            character.MagDefence += abilityData.magDefenceAmount;
            character.InstantiateEffectOnCharacter(abilityData.useEffects["buff"]);

        }



        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);
    }


    private void AddToCharacterList(Character character)
    {
        switch (character.Race)
        {
            case Enums.Races.����:
                if (!hasPeople)
                {
                    characterList.Add(character);
                    hasPeople = true;
                }
                break;
            case Enums.Races.�����:
                if (!hasGnomes)
                {
                    characterList.Add(character);
                    hasGnomes = true;
                }
                break;
            case Enums.Races.�����:
                if (!hasElfs)
                {
                    characterList.Add(character);
                    hasElfs = true;
                }
                break;
            case Enums.Races.Ҹ���������:
                if (!hasDarkElfs)
                {
                    characterList.Add(character);
                    hasDarkElfs = true;
                }
                break;
            case Enums.Races.������������������:
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
            character.MaxSpeed -= abilityData.rangeAmount;
            character.PhysDefence -= abilityData.physDefenceAmount;
            character.MagDefence -= abilityData.magDefenceAmount;
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
[Serializable]
public class ProtectedDonkeyCharacterBuffAbilityData : BaseCharacterAbilityData
{
    public float physDefenceAmount;

    public float magDefenceAmount;

    public float rangeAmount;

    public int turnCount;

    [Header("�� �������!")]
    public bool isBuff;
}