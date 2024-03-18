using System;
using UnityEngine;

[Serializable]
public class UnityWithNatureSupportCardAbility : BaseSupportСardAbility, ITurnCountable
{
    [SerializeField]
    private float physDefence;

    [SerializeField] 
    private int healAmount;

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
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите персонажа", battleSystem));
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

        character.HealMoreThenMax(healAmount);
        character.PhysDefence += physDefence;

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
        character.PhysDefence -= physDefence;
        float finalDamage = character.Damage(healAmount);
        bool isDeath = character.Health == 0;

        if (isDeath)
        {
            string characterType = "";
            if (character is StaticEnemyCharacter staticEnemyCharacter)
            {
                battleSystem.EnemyController.StaticEnemyCharObjects.Remove(staticEnemyCharacter);
            }
            if (character is PlayerCharacter playerCharacter)
            {
                battleSystem.PlayerController.PlayerCharactersObjects.Remove(playerCharacter);
                characterType = "союзный";
            }
            if (character is EnemyCharacter enemyCharacter)
            {
                battleSystem.EnemyController.EnemyCharObjects.Remove(enemyCharacter);
                characterType = "вражеский";
            }
            battleSystem.GameUIPresenter.AddMessageToGameLog($"Эффект дополнительного здоровья от карты \"Единство с природой 2\" заканчивается, {characterType} персонаж {character.CharacterName} погибает");
            GameObject.Destroy(character.gameObject);
        }
        OnReturnToNormal?.Invoke(this);
    }

}