using System.Collections;
using UnityEngine;
using BehaviourTree;


public class Movement : Node
{
    private BattleSystem _battleSystem;
    private BehaviourTree.Tree _EnemyBT;
    public Movement(BattleSystem battleSystem, BehaviourTree.Tree EnemyBT) {
        _battleSystem = battleSystem;
        _EnemyBT = EnemyBT;
    }
    public override NodeState Evaluate()
    {
        Cell cell = (Cell)GetData("cell");
        if (cell!=null)
        {
            _battleSystem.StartCoroutine(StartAction(cell));
            state = NodeState.SUCCESS;
        }
        else
        {
            state = NodeState.FAILURE;
        }
        
        ClearData("cell");
        return state;      
    }
   
    private IEnumerator StartAction(Cell cell)
    {
        yield return new WaitForSeconds(2);
        _battleSystem.OnMoveButton(cell.gameObject);
        _EnemyBT.RestartTree();
    }
}
