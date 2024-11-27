using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFiniteStateMachine_WithoutMono
{
    protected BaseState _CurrentState;
    public void UpdateState() => _CurrentState?.StateUpdate(Time.deltaTime);
    public void SwitchState(BaseState state)
    {
        _CurrentState?.StateExit();
        _CurrentState = state;
        state.StateEnter();
    }
}
