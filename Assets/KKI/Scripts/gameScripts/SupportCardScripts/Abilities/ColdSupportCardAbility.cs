using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ColdSupportCardAbility : BaseSupport�ardAbility, ITurnCountable
{
    public int TurnCount { get => abilityData.turnCount; set => abilityData.turnCount = value; }
    public bool IsBuff { get => abilityData.isBuff; }

    private List<EnemyCharacter> enemyCharacters;
    private List<PlayerCharacter> playerCharacters;

    private ColdSupportCardAbilityData abilityData;
    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, BaseSupport�ardAbilityData baseAbilityData)
    {
        this.battleSystem = battleSystem;
        abilityData = (ColdSupportCardAbilityData)baseAbilityData;
        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x => x.Class == Enums.Classes.������).ToList();
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.IsFreezed = true;
            }
        }
        else
        {
            playerCharacters = battleSystem.PlayerController.PlayerCharactersObjects.Where(x => x.Class == Enums.Classes.������).ToList();
            foreach (var playerCharacter in playerCharacters)
            {
                playerCharacter.IsFreezed = true;
            }
        }

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }

    public void ReturnToNormal()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in playerCharacters)
            {
                playerCharacter.IsFreezed = false;
            }
        }
        else
        {
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.IsFreezed = false;
            }
        }
        OnReturnToNormal?.Invoke(this);
    }
}
[Serializable]
public class ColdSupportCardAbilityData : BaseSupport�ardAbilityData
{
    public int turnCount;

    [Header("�� �������!")]
    public bool isBuff;
}