using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightningStrikeSupportCardAbility : BaseSupportÑardAbility
{
    private SelectCellsBehaviour selectCellsBehaviour;
    private AttackAllCharactersInAreaBehaviour attackAllCharactersInAreaBehaviour;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectCellsBehaviour("Âûáåðèòå îáëàñòü äëÿ íàíåñåíèÿ óäàðà", battleSystem, new Vector2(1, 1), "attack"));

        SetUseCardBehaviour(new AttackAllCharactersInAreaBehaviour(1f, battleSystem, "\"Ïðèçûâ áóðè\""));

        selectCellsBehaviour = (SelectCellsBehaviour)CardSelectBehaviour;
        attackAllCharactersInAreaBehaviour = (AttackAllCharactersInAreaBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        foreach (var item in selectCellsBehaviour.highlightedCells)
        {
            item.OnClick -= selectCellsBehaviour.OnSelectedInvoke;
        }
        battleSystem.FieldController.InvokeActionOnField(selectCellsBehaviour.UnSubscribe);
        attackAllCharactersInAreaBehaviour.cellsToAttack = selectCellsBehaviour.highlightedCells.Where(x => x.GetComponentInChildren<Character>() != null).ToList();
        if (attackAllCharactersInAreaBehaviour.cellsToAttack.Count == 0)
        {
           SelectCard();
        }
        else
        {
            UseCard(null);
        }
    }


    private void OnCardUse()
    {
        foreach (var item in selectCellsBehaviour.highlightedCells)
        {
            item.OnClick -= selectCellsBehaviour.OnSelectedInvoke;
        }
        battleSystem.FieldController.TurnOnCells();
    }
}
