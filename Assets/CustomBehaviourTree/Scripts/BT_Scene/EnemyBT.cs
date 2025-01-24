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
        EnemyTestTask attackPlayer = new(2f, "EenmyAttackTask", "AttackPlayer");
        BT_Decorator_Condition conditionDecorator = new(attackPlayer, () => IsPlayerInsight, _enemyBlackboard, "IsPlayerInsight");

        BT_Sequencer petrolSeq = new();
        EnemyTestTask patrolToA = new(1.6f, "PatrolToA", "PatrolA");
        EnemyTestTask patrolToB = new(1.6f, "PatrolToB", "PatrolB");
        EnemyTestTask patrolToC = new(1.6f, "PatrolToC", "PatrolC");
        BT_Decorator_Blackboard blackboardDecorator = new(petrolSeq, _enemyBlackboard);


        rootNode.AddChild(conditionDecorator);
        rootNode.AddChild(blackboardDecorator);

        petrolSeq.AddChild(patrolToA);
        petrolSeq.AddChild(patrolToB);
        petrolSeq.AddChild(patrolToC);


        outRootNode = rootNode;
    }

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
