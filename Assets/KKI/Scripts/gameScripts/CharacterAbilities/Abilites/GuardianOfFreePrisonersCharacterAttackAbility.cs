using System;
using UnityEngine;

[Serializable]
public class GuardianOfFreePrisonersCharacterAttackAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private float speedDecreaseAmount;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private Character character;

    public event Action<ITurnCountable> OnReturnToNormal;

    private int decreaseAmount;
    public override void Init(BattleSystem battleSystem, Character abilityOwner)
    {
        this.abilityOwner = abilityOwner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Âûáåðèòå âðàæåñêîãî ïåðñîíàæà äëÿ àòàêè", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new AttackSelectedÑharacterBehaviour(damage, battleSystem, "\"Ñòàëüíûå îêîâû\""));

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

        decreaseAmount = (int)Math.Ceiling(character.Speed * speedDecreaseAmount);
        character.Speed = decreaseAmount;

        UseCard(character.gameObject);
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        battleSystem.EnemyController.SetEnemiesStates(true, false, x =>
        {
            x.OnClick -= SelectCharacter;
        });
        battleSystem.PlayerController.SetPlayerStates(true, false);
    }
    public void ReturnToNormal()
    {
        character.Speed += decreaseAmount;
        OnReturnToNormal?.Invoke(this);
    }
}
