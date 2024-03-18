using System;
using System.Linq;
using UniRx;
using UnityEngine;

[Serializable]
public class StormCallSupportCardAbility : BaseSupportÑardAbility
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private Vector2 area;

    private SelectCellsBehaviour selectCellsBehaviour;
    private AttackAllCharactersInAreaBehaviour attackAllCharactersInAreaBehaviour;
    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsBehaviour("Âûáåðèòå îáëàñòü äëÿ íàíåñåíèÿ óäàðà", battleSystem, area, "attack"));

        SetUseCardBehaviour(new AttackAllCharactersInAreaBehaviour(damage, battleSystem, "\"Ïðèçûâ áóðè\""));

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
        battleSystem.FieldController.TurnOnCells();
    }
}
