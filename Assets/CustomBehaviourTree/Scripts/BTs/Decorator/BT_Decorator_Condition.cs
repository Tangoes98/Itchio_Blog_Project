using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Decorator_Condition : BT_Decorator
{
    Func<bool> _conditionResult;
    BT_Blackboard _blackboard;
    string _conditionKey;
    public BT_Decorator_Condition(BT_Node child, Func<bool> conditionResult, BT_Blackboard blackboard, string conditionKey) : base(child)
    {
        _conditionResult = conditionResult;
        _blackboard = blackboard;
        _conditionKey = conditionKey;
    }

    protected override NodeState NodeEnter()
    {
        _blackboard.OnBlackboardValueChange -= BlackboardEvt;
        _blackboard.OnBlackboardValueChange += BlackboardEvt;

        return base.NodeEnter();
    }

    protected override NodeState NodeTick()
    {
        if (!_conditionResult.Invoke())
            return NodeState.Failure;

        return _child.RunNode();
    }


    void BlackboardEvt(string key, object value)
    {

        //* Update Deco condition
        if (key == _conditionKey && value is bool)
        {
            _conditionResult = () => (bool)value;
            return;
        }

        //* Exit node
        NodeAbort();
    }
}

// if (!_blackboard.GetData(condition, out result))
//     return;