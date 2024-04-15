using System;
using UnityEngine;

[Serializable]
public class UnityWithNatureSupportCardAbility : BaseSupport�ardAbility, ITurnCountable
{
    public int TurnCount { get => abilityData.turnCount; set => abilityData.turnCount = value; }
    public bool IsBuff { get => abilityData.isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private Character character;
    private UnityWithNatureSupportCardAbilityData abilityData;
    public override void Init(BattleSystem battleSystem, BaseSupport�ardAbilityData baseAbilityData)
    {
        this.battleSystem = battleSystem;
        abilityData = (UnityWithNatureSupportCardAbilityData)baseAbilityData;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour(abilityData.selectCardText, battleSystem));
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

        character.HealMoreThenMax(abilityData.healAmount, abilityData.support�ardAbilityName);
        character.PhysDefence += abilityData.physDefence;

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
        character.PhysDefence -= abilityData.physDefence;
        bool isDeath = character.Damage(abilityData.healAmount, abilityData.support�ardAbilityName);

        if (isDeath)
        {
            string characterType = "";
            if (character is PlayerCharacter)
            {
                characterType = "�������";
            }
            if (character is EnemyCharacter)
            {
                characterType = "���������";
            }
            battleSystem.gameLogCurrentText.Value = $"������ ��������������� �������� �� ����� \"�������� � �������� 2\" �������������, {characterType} �������� {character.CharacterName} ��������";
            GameObject.Destroy(character.gameObject);
        }
        OnReturnToNormal?.Invoke(this);
    }

}
[Serializable]
public class UnityWithNatureSupportCardAbilityData : BaseSupport�ardAbilityData
{
    public string selectCardText;

    public float physDefence;

    public int healAmount;

    public int turnCount;

    [Header("�� �������!")]
    public bool isBuff;
}