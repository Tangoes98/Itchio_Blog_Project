using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public abstract void StateEnter();
    public abstract void StateUpdate(float deltaTime);
    public abstract void StateExit();
}
