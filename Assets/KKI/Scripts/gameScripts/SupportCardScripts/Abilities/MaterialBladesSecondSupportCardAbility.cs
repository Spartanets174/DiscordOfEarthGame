using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;

[Serializable]
public class MaterialBladesSecondSupportCardAbility : BaseSupportСardAbility, ITurnCountable
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private List<Character> characters = new();
    AttackSelectedСharactersBehaviour attackSelectedСharactersBehaviour;
    public override void Init(BattleSystem battleSystem)
    {
        characters.Clear();
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Выберите вражеского персонажа для атаки", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Выберите второго вражеского персонажа для атаки", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new AttackSelectedСharactersBehaviour(damage, battleSystem, "\"Материальные клинки 2\""));

        attackSelectedСharactersBehaviour = (AttackSelectedСharactersBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_cardSecondSelectBehaviour.OnSelected += OnSecondSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;

        m_cardSecondSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                enemyCharacter.OnClick += SelectSecondCharacterInvoke;
            }

        }
    }

    private void SelectSecondCharacterInvoke(GameObject gameObject)
    {
        characters.Add(gameObject.GetComponent<Character>());

        battleSystem.EnemyController.SetEnemiesChosenState(false, x =>
        {
            x.OnClick -= SelectSecondCharacterInvoke;
        });

        SelectSecondCard();
    }



    private void OnSecondSelected()
    {
        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.OnClick += SelectCharacter;
        }
    }


    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            characters.Add(battleSystem.EnemyController.CurrentEnemyCharacter);
        }
        else
        {
            characters.Add(battleSystem.PlayerController.CurrentPlayerCharacter);
        }

        attackSelectedСharactersBehaviour.charactersToAttack = characters;
        UseCard(null);
        foreach (var character in characters)
        {
            character.IsFreezed = true;

        }
    }

    private void OnCardUse()
    {
        Uncubscribe();
    }

    private void OnCancelSelection()
    {
        characters.Clear();
        Uncubscribe();
    }

    private void Uncubscribe()
    {
        battleSystem.EnemyController.SetEnemiesStates(true, false, x =>
        {
            x.OnClick -= SelectSecondCharacterInvoke;
            x.OnClick -= SelectCharacter;
        });

        battleSystem.PlayerController.SetPlayerStates(true, false);
    }

    public void ReturnToNormal()
    {
        foreach (var character in characters)
        {
            character.IsFreezed = false;
        }
        OnReturnToNormal?.Invoke(this);
    }
}
