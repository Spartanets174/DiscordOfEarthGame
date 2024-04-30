using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class KeenEyeSupportCardAbility : BaseSupport�ardAbility, ITurnCountable
{
    public int TurnCount { get => abilityData.turnCount; set => abilityData.turnCount = value; }
    public bool IsBuff { get => abilityData.isBuff; }

    private List<EnemyCharacter> enemyCharacters;
    private List<PlayerCharacter> playerCharacters;
    private KeenEyeSupportCardAbilityData abilityData;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, BaseSupport�ardAbilityData baseAbilityData)
    {
        this.battleSystem = battleSystem;
        abilityData = (KeenEyeSupportCardAbilityData)baseAbilityData;
        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            playerCharacters = battleSystem.PlayerController.PlayerCharactersObjects.Where(x => x.Class == Enums.Classes.������).ToList();
            foreach (var playerCharacter in playerCharacters)
            {
                playerCharacter.CritChance = abilityData.critChance;
                playerCharacter.InstantiateEffectOnCharacter(abilityData.effect);

            }
        }
        else
        {
            enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x => x.Class == Enums.Classes.������).ToList();
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.CritChance = abilityData.critChance;
                enemyCharacter.InstantiateEffectOnCharacter(abilityData.effect);

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
                playerCharacter.CritChance = playerCharacter.Card.critChance;
            }
        }
        else
        {
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.CritChance = enemyCharacter.Card.critChance;
            }
        }
        OnReturnToNormal?.Invoke(this);
    }
}
[Serializable]
public class KeenEyeSupportCardAbilityData : BaseSupport�ardAbilityData
{
    public  float critChance;

    public int turnCount;

    [Header("�� �������!")]
    public bool isBuff;
}