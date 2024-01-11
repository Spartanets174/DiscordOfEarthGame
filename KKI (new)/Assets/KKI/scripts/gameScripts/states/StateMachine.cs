using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine: MonoBehaviour
{
    public State State;

    public void SetState(State state)
    {
        if (State!=null)
        {
            State.OnStepCompletedInvoke();
        }
        State = state;
        StartCoroutine(State.Start());
    }
}
