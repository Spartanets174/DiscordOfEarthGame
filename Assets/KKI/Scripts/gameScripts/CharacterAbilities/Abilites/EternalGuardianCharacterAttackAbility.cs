using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EternalGuardianCharacterAttackAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float damage;  

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private Character character;
    

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("¬˚·ÂËÚÂ ‚‡ÊÂÒÍÓ„Ó ÔÂÒÓÌ‡Ê‡ ‰Îˇ ‡Ú‡ÍË", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new FormulaAttackSelected—haracterBehaviour(damage, battleSystem, abilityOwner, "\"¡ÓÎ¸ ‚ „Û‰Ë\""));

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

        float speed = Mathf.Ceil(character.MaxSpeed / 2);
        character.MaxSpeed = speed;

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
        character.MaxSpeed = character.Card.speed;
        OnReturnToNormal?.Invoke(this);
        character = null;
    }
}
[Serializable]
public class EternalGuardianCharacterAttackAbilityData
{
    public float damage;

    public int turnCount;

    [Header("Õ≈ “–Œ√¿“‹")]
    public bool isBuff;
}