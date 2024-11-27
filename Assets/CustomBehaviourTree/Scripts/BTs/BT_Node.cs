using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum NodeState
{
    Success, Failure, Running
}

public abstract class BT_Node
{
    bool firstTimeExecute = true;
    public NodeState RunNode()
    {
        //todo: Function to execute node once
        if (firstTimeExecute)
        {
            firstTimeExecute = false;
            NodeState executeResult = NodeEnter();
            if (executeResult == NodeState.Success || executeResult == NodeState.Failure)
            {
                EndNode();
                return executeResult;
            }
        }

        //todo: Function to update node each frame
        NodeState updateResult = NodeTick();
        if (updateResult == NodeState.Success || updateResult == NodeState.Failure)
        {
            EndNode();
        }
        return updateResult;
    }

    protected NodeState _abortState = NodeState.Running;
    protected virtual NodeState NodeEnter()
        => NodeState.Running;
    protected virtual NodeState NodeTick()
        => NodeState.Running;
    protected virtual void NodeExit()
        => _abortState = NodeState.Running;
    public virtual void NodeAbort()
        => EndNode();

    void EndNode()
    {
        firstTimeExecute = true;
        NodeExit();
    }
}
