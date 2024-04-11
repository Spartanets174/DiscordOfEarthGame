using System;
using UnityEngine;

[Serializable]
public class MaterialBladesSupportCardAbility : BaseSupport�ardAbility, ITurnCountable
{ 
    public int TurnCount { get => abilityData.turnCount; set => abilityData.turnCount = value; }
    public bool IsBuff { get => abilityData.isBuff; }

    private Character character;
    private MaterialBladesSupportCardAbilityData abilityData;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, BaseSupport�ardAbilityData baseAbilityData)
    {
        this.battleSystem = battleSystem;
        abilityData = (MaterialBladesSupportCardAbilityData)baseAbilityData;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour(abilityData.selectCardText, battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new AttackSelected�haracterBehaviour(abilityData.damage, battleSystem, $"\"{abilityData.support�ardAbilityName}\""));

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                enemyCharacter.OnClick += SelectCharacter;
            }
        }

    }


    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }
        else
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }

        character.IsFreezed = true;

        UseCard(character.gameObject);
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        battleSystem.EnemyController.SetEnemiesStates(true,false, x =>
        {
            x.OnClick -= SelectCharacter;
        });
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }
    public void ReturnToNormal()
    {
        character.IsFreezed = false;
        OnReturnToNormal?.Invoke(this);
    }
}
[Serializable]
public class MaterialBladesSupportCardAbilityData : BaseSupport�ardAbilityData
{
    public string selectCardText;

    public float damage;

    public int turnCount;

    [Header("�� �������!")]
    public bool isBuff;
}