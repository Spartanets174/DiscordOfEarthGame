using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class SergeantMajorCharacterAttackAbility : BaseCharacterAbility
{
    [SerializeField]
    private float increaseDamageAmount;

    [SerializeField]
    private float damage;

    [SerializeField]
    private int range;

    List<Character> characters ;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;
    private SelectCellsWithCharactersInRangeBehaviour secondSelectCellsToAttackInRangeBehaviour;

    private FormulaAttackSelected�haractersBehaviour formulaAttackSelected�haractersBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        characters = new();
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("������� �� ��������� � ������� ������ ��� �����", battleSystem, abilityOwner, range, "attack"));
        SetSecondCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("������� �� ��������� � ������� ������ ��� �������������� �����", battleSystem, abilityOwner, 1, "attack"));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new FormulaAttackSelected�haractersBehaviour(damage, battleSystem, abilityOwner, "\"����� ���\"", increaseDamageAmount));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;
        secondSelectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSecondSelectBehaviour;
        formulaAttackSelected�haractersBehaviour = (FormulaAttackSelected�haractersBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;

        m_cardSecondSelectBehaviour.OnSelected += OnSecondSelected;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;

        m_useCardBehaviour.OnCardUse += OnCardUse;
    }



    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
            {
                enemyCharacter.OnClick += SelectSecondInvoke;
            }
        }
    }

    private void SelectSecondInvoke(GameObject gameObject)
    {
        foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            enemyCharacter.OnClick -= SelectSecondInvoke;
        }
        characters.Add(gameObject.GetComponent<Character>());
        secondSelectCellsToAttackInRangeBehaviour.chosenCharacter = characters.FirstOrDefault();


        SelectSecondCard();
    }


    private void OnSecondSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            if (secondSelectCellsToAttackInRangeBehaviour.charactersOnCells.Count>0)
            {
                foreach (var enemyCharacter in secondSelectCellsToAttackInRangeBehaviour.charactersOnCells)
                {
                    enemyCharacter.OnClick += SelectCharacter;
                }
            }
            else
            {
                OnSelectCharacter();
            }
            
        }

    }

    private void OnSelectCharacter()
    {
        foreach (var enemyCharacter in secondSelectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            enemyCharacter.OnClick -= SelectCharacter;
        }

        if (secondSelectCellsToAttackInRangeBehaviour.charactersOnCells.Count > 0)
        {
            characters.Add(battleSystem.CurrentChosenCharacter.Value);
        }      

        formulaAttackSelected�haractersBehaviour.characters.AddRange(characters);

        UseCard(abilityOwner.gameObject);
    }


    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        foreach (var enemyCharacter in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            enemyCharacter.OnClick -= SelectSecondInvoke;
        }
        foreach (var enemyCharacter in secondSelectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            enemyCharacter.OnClick -= SelectCharacter;
        }
        characters.Clear();
        selectCellsToAttackInRangeBehaviour.charactersOnCells.Clear();
        secondSelectCellsToAttackInRangeBehaviour.charactersOnCells.Clear();
    }

}