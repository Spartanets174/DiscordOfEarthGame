using System;
using UnityEngine;

public class UnityWithNatureSecondSupportCardAbility : BaseSupportСardAbility, ITurnCountable
{
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private Character character;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите персонажа", battleSystem));
        SetSelectCharacterBehaviour(new EmptySelectCharacterBehaviour(""));

        m_isBuff = true;
        TurnCount = 2;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
    }

    private void OnDestroy()
    {
        m_cardSelectBehaviour.OnCancelSelection -= OnCancelSelection;
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter -= OnSelectCharacter;
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

        character.HealMoreThenMax(2);
        character.PhysDefence += 2;

        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectCharacter;
            playerCharacter.IsChosen = false;
        }
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
        character.PhysDefence -= 2;
        float finalDamage = character.Damage(2);

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
