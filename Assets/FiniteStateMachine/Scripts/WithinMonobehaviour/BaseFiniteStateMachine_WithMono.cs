using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFiniteStateMachine_WithMono : MonoBehaviour
{

    protected BaseState _CurrentState;

    protected virtual void Update()
    {
        _CurrentState?.StateUpdate(Time.deltaTime);
    }

    public void SwitchState(BaseState state)
    {
        _CurrentState?.StateExit();
        _CurrentState = state;
        state.StateEnter();
    }

}
