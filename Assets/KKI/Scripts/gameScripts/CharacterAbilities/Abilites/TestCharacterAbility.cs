using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TestCharacterAbility : BaseCharacterAbility, ITurnCountable
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
    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        characters.Clear();
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Выберите вражеского персонажа для атаки", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Выберите второго вражеского персонажа для атаки", battleSystem));
        SetThirdCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Выберите третьего вражеского персонажа для атаки", battleSystem));
        SetSelectCharacterBehaviour(new SetAbiableCellsBehaviour("Выберите клетку", battleSystem));
        SetUseCardBehaviour(new AttackSelectedСharacterBehaviour(damage, battleSystem, "\"Тестовая\""));

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_cardSecondSelectBehaviour.OnSelected += OnSecondSelected;
        m_cardThirdSelectBehaviour.OnSelected += OnThirdSelected;

        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardThirdSelectBehaviour.OnCancelSelection += OnCancelSelection;

        setAbiableCellsBehaviour = (SetAbiableCellsBehaviour)SelectCharacterBehaviour;
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
            enemyCharacter.OnClick += SelectThirdCharacterInvoke;
        }
    }

    private void SelectThirdCharacterInvoke(GameObject gameObject)
    {
        characters.Add(gameObject.GetComponent<Character>());

        battleSystem.EnemyController.SetEnemiesChosenState(false, x =>
        {
            x.OnClick -= SelectThirdCharacterInvoke;
        });

        SelectThirdCard();
    }

    private void OnThirdSelected()
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

        battleSystem.EnemyController.SetEnemiesChosenState(false, x =>
        {
            x.OnClick -= SelectCharacter;
        });

        if (characters.Count == 3)
        {
            foreach (var character in characters)
            {
                if (character!=null)
                {
                    setAbiableCellsBehaviour.cellsToMove.AddRange(battleSystem.FieldController.GetCellsForMove(character, character.Speed));
                }
                
            }
            foreach (var item in setAbiableCellsBehaviour.cellsToMove)
            {
                item.OnClick += (x)=> UseCard(characters[0].gameObject);
            }
            Debug.Log("Cells count "+ setAbiableCellsBehaviour.cellsToMove.Count);
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
        foreach (var x in battleSystem.EnemyController.EnemyCharObjects)
        {
            x.OnClick -= SelectThirdCharacterInvoke;
            x.OnClick -= SelectSecondCharacterInvoke;
            x.OnClick -= SelectCharacter;
        }
        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= (x) => UseCard(characters[0].gameObject);
        }
        setAbiableCellsBehaviour.cellsToMove.Clear();
    }

    public void ReturnToNormal()
    {
        Debug.Log("Returned");
        OnReturnToNormal?.Invoke(this);
    }
}

