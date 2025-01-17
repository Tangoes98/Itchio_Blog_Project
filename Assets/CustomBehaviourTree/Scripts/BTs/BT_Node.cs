using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum NodeState
{
    Success, Failure, Running
}

public abstract class BT_Node
{
    bool _firstTimeExecute = true;
    public NodeState RunNode()
    {
        //todo: Function to execute node once
        if (_firstTimeExecute)
        {
            _firstTimeExecute = false;
            NodeState executeResult = NodeEnter();
            if (executeResult == NodeState.Success
                || executeResult == NodeState.Failure)
            {
                NodeEnd();
                return executeResult;
            }
        }

        //todo: Function to update node each frame
        NodeState updateResult = NodeTick();
        if (updateResult == NodeState.Success
            || updateResult == NodeState.Failure)
            NodeEnd();
        return updateResult;
    }

    protected virtual NodeState NodeEnter()
        => NodeState.Running;
    protected virtual NodeState NodeTick()
        => NodeState.Running;
    protected virtual void NodeExit()
        => AbortState = NodeState.Running;


    public NodeState AbortState;
    public virtual void NodeAbort()
        => NodeEnd();

    void NodeEnd()
    {
        NodeExit();
        _firstTimeExecute = true;
    }

    public string CurrentActiveNode;
}
