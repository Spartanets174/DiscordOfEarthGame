using System;
using UnityEngine;

[Serializable]
public class TangibleBodySupportCardAbility : BaseSupport�ardAbility, ITurnCountable
{
    [SerializeField]
    private float healAmount;

    [SerializeField]
    private float physAmount;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private Character character;
    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("�������� ���������", battleSystem));
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
        character.PhysDefence += physAmount;
        character.PhysAttack += physAmount;

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
        character.PhysDefence -= physAmount;
        character.PhysAttack -= physAmount;
        bool isDeath = character.Damage(healAmount, "��������� ����");

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
