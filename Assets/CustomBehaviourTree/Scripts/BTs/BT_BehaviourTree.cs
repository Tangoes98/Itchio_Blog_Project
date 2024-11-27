using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BT_BehaviourTree : MonoBehaviour
{
    BT_Node _rootNode;
    protected abstract void ConstructTree(out BT_Node outRootNode);

    protected virtual void Start()
    {
        ConstructTree(out _rootNode);
    }

    protected virtual void Update()
    {
        _rootNode.RunNode();
    }
}
