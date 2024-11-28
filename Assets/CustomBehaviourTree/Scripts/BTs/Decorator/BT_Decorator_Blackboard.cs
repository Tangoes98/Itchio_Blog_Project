using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Decorator_Blackboard : BT_Decorator
{
    BT_Blackboard _blackboard;
    public BT_Decorator_Blackboard(BT_Node child, BT_Blackboard blackboard) : base(child)
    {
        _blackboard = blackboard;
    }

    protected override NodeState NodeEnter()
    {
        _blackboard.OnBlackboardValueChange -= BlackboardEvt;
        _blackboard.OnBlackboardValueChange += BlackboardEvt;

        return base.NodeEnter();
    }

    protected override NodeState NodeTick()
    {
        return _child.RunNode();
    }

    void BlackboardEvt(string condition, object result)
    {
        NodeAbort();
    }
}
