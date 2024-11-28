using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyTestTask : BT_Node
{
    Timer _timer;
    string _taskName;
    string _taskLog;
    public EnemyTestTask(float time, string taskName, string taskLog)
    {
        _timer = new(time);
        _taskName = taskName;
        _taskLog = taskLog;
    }

    protected override NodeState NodeEnter()
    {
        //Debug.Log($"Enter task {_taskLog}");
        return NodeState.Running;
    }

    protected override NodeState NodeTick()
    {
        Debug.Log($"{_taskName} is running");
        if (!_timer.IsTimeUp())
            return NodeState.Running;

        return NodeState.Success;
    }

    protected override void NodeExit()
    {
        _timer.ResetTimer();
        base.NodeExit();
    }

}
