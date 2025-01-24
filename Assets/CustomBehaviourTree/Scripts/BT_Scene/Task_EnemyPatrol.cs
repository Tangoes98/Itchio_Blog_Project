using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_EnemyPatrol : BT_Node
{
    Timer _timer;
    string _taskName;
    public Task_EnemyPatrol(float patrolTime, string taskName)
    {
        _timer = new(patrolTime);
        _taskName = taskName;
    }


    protected override NodeState NodeEnter()
    {
        return NodeState.Running;
    }

    protected override NodeState NodeTick()
    {
        if (!_timer.IsTimeUp())
            return NodeState.Running;

        Debug.Log($"{_taskName} is time up");
        return NodeState.Success;
    }

    protected override void NodeExit()
    {
        _timer.ResetTimer();
    }


}
