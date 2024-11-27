using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBT : BT_BehaviourTree
{
    public bool IsPlayerInsight;
    BT_Blackboard _enemyBlackboard;
    protected override void ConstructTree(out BT_Node outRootNode)
    {
        _enemyBlackboard = new();
        BT_Selector rootNode = new();
        //BT_Sequencer rootNode = new();
        EnemyTestTask attackPlayer = new(3f, "EenmyAttackTask", "AttackPlayer");
        BT_Decorator_Condition conditionalDeco = new(attackPlayer, () => IsPlayerInsight, _enemyBlackboard, "IsPlayerInsight");

        BT_Sequencer petrolSeq = new();
        EnemyTestTask taskA = new(3f, "TaskA", "");
        EnemyTestTask taskB = new(3f, "TaskB", "");
        EnemyTestTask taskC = new(3f, "TaskC", "");
        BT_Decorator_Blackboard bbDeco = new(petrolSeq, _enemyBlackboard);


        rootNode.AddChild(conditionalDeco);
        rootNode.AddChild(bbDeco);

        petrolSeq.AddChild(taskA);
        petrolSeq.AddChild(taskB);
        petrolSeq.AddChild(taskC);


        outRootNode = rootNode;
    }
    //BT_Sequencer TestNode;
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _enemyBlackboard.SetData("IsPlayerInsight", true);


        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _enemyBlackboard.SetData("IsPlayerInsight", false);


        }

    }
}
