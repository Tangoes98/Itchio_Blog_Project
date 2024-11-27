using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_CompositeNode : BT_Node
{

    LinkedList<BT_Node> _childNodes = new();
    LinkedListNode<BT_Node> _currentChildNode;
    protected BT_Node CurrentChildNode() => _currentChildNode.Value;

    public void AddChild(BT_Node node)
    => _childNodes.AddLast(node);

    protected bool GetNextChildNode()
    {
        if (_currentChildNode == _childNodes.Last)
            return false;

        _currentChildNode = _currentChildNode.Next;
        return true;
    }


    // Both Selector and Sequencer will run NodeEnter by inheriting this class
    protected override NodeState NodeEnter()
    {
        if (_childNodes.Count == 0)
            return NodeState.Failure;

        _currentChildNode = _childNodes.First;
        return _abortState;
    }
}
