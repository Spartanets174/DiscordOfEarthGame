using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class StormCallSupportCardAbility : BaseSupport�ardAbility
{
    private SelectCellsBehaviour selectCellsBehaviour;
    private AttackAllCharactersInAreaBehaviour attackAllCharactersInAreaBehaviour;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectCellsBehaviour("�������� ������� ��� ��������� �����", battleSystem,new Vector2(3,3)));

        SetUseCardBehaviour(new AttackAllCharactersInAreaBehaviour(0.5f,battleSystem, "\"������ ����\""));

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

        if (attackAllCharactersInAreaBehaviour.cellsToAttack.Count==0)
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
        battleSystem.FieldController.TurnOnCells();
    }
}
