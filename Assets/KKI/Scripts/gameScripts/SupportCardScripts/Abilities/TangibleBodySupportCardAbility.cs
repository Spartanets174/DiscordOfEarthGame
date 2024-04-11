using System;
using UnityEngine;

[Serializable]
public class TangibleBodySupportCardAbility : BaseSupport�ardAbility, ITurnCountable
{
    public int TurnCount { get => abilityData.turnCount; set => abilityData.turnCount = value; }
    public bool IsBuff { get => abilityData.isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private Character character;
    private TangibleBodySupportCardAbilityData abilityData;
    public override void Init(BattleSystem battleSystem, BaseSupport�ardAbilityData baseAbilityData)
    {
        this.battleSystem = battleSystem;
        abilityData = (TangibleBodySupportCardAbilityData)baseAbilityData;
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

        character.HealMoreThenMax(1);
        character.PhysDefence += abilityData.physAmount;
        character.PhysAttack += abilityData.physAmount;

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
        character.PhysDefence -= abilityData.physAmount;
        character.PhysAttack -= abilityData.physAmount;
        bool isDeath = character.Damage(abilityData.healAmount, $"{abilityData.support�ardAbilityName}");

        if (isDeath)
        {
            string characterType = "";
            if (character is PlayerCharacter)
            {
                characterType = "�������";
            }
            else
            {
                characterType = "���������";
            }
            battleSystem.gameLogCurrentText.Value = $"������ ��������������� �������� �� ����� \"��������� ����\" �������������, {characterType} �������� {character.CharacterName} ��������";
            GameObject.Destroy(character.gameObject);
        }
        OnReturnToNormal?.Invoke(this);
    }

}
[Serializable]
public class TangibleBodySupportCardAbilityData : BaseSupport�ardAbilityData
{
    public string selectCardText;

    public float healAmount;

    public float physAmount;

    public int turnCount;

    [Header("�� �������!")]
    public bool isBuff;
}