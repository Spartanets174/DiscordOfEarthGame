using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    protected BattleSystem BattleSystem;


    public event Action OnStepStarted;
    public event Action OnStepCompleted;
    public State(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
    }

    public void OnStepStartedInvoke()
    {
        OnStepStarted?.Invoke();
    }

    public void OnStepCompletedInvoke()
    {
        OnStepCompleted?.Invoke();
    }


    public virtual IEnumerator Start()
    {
        yield break;
    }
    public virtual IEnumerator UnitStatement(GameObject character)
    {
        yield break;
    }
    public virtual IEnumerator ChooseCharacter(GameObject character)
    {
        yield break;
    }
    public virtual IEnumerator Move(GameObject cell)
    {
        yield break;
    }
    public virtual IEnumerator Attack(Character target)
    {
        yield break;
    }
    public virtual IEnumerator UseAttackAbility()
    {
        yield break;
    }
    public virtual IEnumerator UseDefensiveAbility()
    {
        yield break;
    }
    public virtual IEnumerator UseBuffAbility()
    {
        yield break;
    }
    public virtual IEnumerator UseSupportCard()
    {
        yield break;
    }
    public virtual IEnumerator UseItem()
    {
        yield break;
    }
}
