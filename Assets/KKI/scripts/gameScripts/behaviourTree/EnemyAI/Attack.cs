using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Attack : Node
{
    BattleSystem m_battleSystem;
    private BehaviourTree.Tree m_enemyBT;
    public Attack(BattleSystem battleSystem, EnemyController EnemyBT)
    {
        m_battleSystem = battleSystem;
        m_enemyBT = EnemyBT;
    }

    public override NodeState Evaluate()
    {
        Character _character = (Character)GetData("enemy");

        m_battleSystem.OnAttackButton(_character.gameObject);        
        state = NodeState.RUNNING;
        ClearData("enemy");
        m_enemyBT.RestartTree();
        return state;
    }
    
}
