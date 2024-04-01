using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class WarriorOfLightCharacterDefenceAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private int range;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private SelectCellsWithCharactersInRangeBehaviour selectCellsToAttackInRangeBehaviour;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsWithCharactersInRangeBehaviour("", battleSystem, abilityOwner, range, "allowed"));

        selectCellsToAttackInRangeBehaviour = (SelectCellsWithCharactersInRangeBehaviour)CardSelectBehaviour;

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }
    private void OnSelected()
    {
        foreach (var character in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            character.IgnoreMagDamage = true;
        }
        UseCard(abilityOwner.gameObject);
    }

    public void ReturnToNormal()
    {
        foreach (var character in selectCellsToAttackInRangeBehaviour.charactersOnCells)
        {
            character.IgnoreMagDamage = false;
        }

        OnReturnToNormal?.Invoke(this);
    }
}
