using UnityEngine;
using BehaviourTree;

public class ChooseCharacter : Node
{
    private BattleSystem m_battleSystem;
    private EnemyController m_enemyController;

    public ChooseCharacter(EnemyController enemyController, BattleSystem battleSystem)
    {
        m_battleSystem = battleSystem;
        m_enemyController = enemyController;
    }

    public override NodeState Evaluate()
    {
        EnemyCharacter enemyCharacter = ChooseChar();
        Debug.Log(enemyCharacter.CharacterName);
        if (enemyCharacter!=null)
        {
            m_battleSystem.OnChooseCharacterButton(enemyCharacter.gameObject);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            state = NodeState.FAILURE;
            return state;
        }       
    }
    public EnemyCharacter ChooseChar()
    {
        EnemyCharacter enemyCharacter=null;
        bool isCharacterChosen=false;
        
        while (!isCharacterChosen)
        {
            if (!IsAviableCharacters())
            {
                break;
            }
            enemyCharacter = m_enemyController.EnemyCharObjects[Random.Range(0, m_enemyController.EnemyCharObjects.Count)];
            isCharacterChosen = IsCharValid(enemyCharacter);
        }
        return enemyCharacter;
    }

    private bool IsAviableCharacters()
    {
        int count = 0;
        for (int i = 0; i < m_enemyController.EnemyCharObjects.Count; i++)
        {
            if (m_enemyController.EnemyCharObjects[i].Speed == 0 && m_enemyController.EnemyCharObjects[i].IsAttackedOnTheMove)
            {
                count++;              
            }
        }
        if (count == m_enemyController.EnemyCharObjects.Count)
        {
            m_battleSystem.SetPlayerTurn();
            return false;
        }
        else
        {
            return true;
        }
        
    }
    private bool IsCharValid(EnemyCharacter enemy)
    {
        if (!enemy.IsAttackedOnTheMove || enemy.Speed > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}