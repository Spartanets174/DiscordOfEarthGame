using BehaviourTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Tree = BehaviourTree.Tree;

public class EnemyController : Tree
{
    [Header("Main sripts")]
    [SerializeField]
    private BattleSystem battleSystem;
    [SerializeField]
    private FieldController fieldController;
    [SerializeField]
    private PlayerData playerData;

    [Header("Static enemy cards")]
    [SerializeField]
    private CharacterCard assasinCard;
    [SerializeField]
    private CharacterCard goliafCard;
    [SerializeField]
    private CharacterCard elementalCard;

    [Header("Enemy prefabs")]
    [SerializeField]
    private EnemyCharacter enemyPrefab;
    [SerializeField]
    private StaticEnemyCharacter staticEnemyPrefab;

    [Header("Enemy materials")]
    [SerializeField]
    private Material redMaterial;
    [SerializeField]
    private Material goliafMaterial;
    [SerializeField]
    private Material assasinMaterial;
    [SerializeField]
    private Material elementalMaterial;

    private List<EnemyCharacter> m_enemyCharObjects = new();
    public List<EnemyCharacter> EnemyCharObjects => m_enemyCharObjects;

    private List<StaticEnemyCharacter> m_staticEnemyCharObjects = new();
    public List<StaticEnemyCharacter> StaticEnemyCharObjects => m_staticEnemyCharObjects;

    private List<CharacterCard> m_enemyCharCards=new();
    public List<CharacterCard> EnemyCharCards => m_enemyCharCards;

    private EnemyCharacter m_currentEnemyCharacter;
    public EnemyCharacter CurrentEnemyCharacter => m_currentEnemyCharacter;

    public override void Init()
    {
        CreateEnemy();
        InstantiateEnemies();
    }
    public override void SetupTree()
    {
        _root = new Sequence(new List<Node>
                {
                    new ChooseCharacter(this, battleSystem),
                    new Selector(new List<Node>
                    {
                        new Sequence(new List<Node>{
                            new CheckCellsForAttack(battleSystem,this),
                            new Attack(battleSystem,this),
                        }),
                        new Sequence(new List<Node>{
                            new CheckCellsForMove(battleSystem,this),
                            new Movement(battleSystem,this),
                        }),
                    })

                });
    }

    public override void RestartTree()
    {
        if (_root != null)
        {
            _root.Evaluate();
        }
        else
        {
            SetupTree();
        }
    }
    private void CreateEnemy()
    {
        while (m_enemyCharCards.Count < 5)
        {
            CharacterCard EnemyMan = playerData.allCharCards[Random.Range(0, playerData.allCharCards.Count)];
            if (!m_enemyCharCards.Contains(EnemyMan))
            {
                m_enemyCharCards.Add(EnemyMan);
            }

        }
    }
    private void InstantiateEnemies()
    {
        int count = 0;
        int count2 = 0;
        //Спавн статических врагов
        for (int i = 0; i < fieldController.CellsOfFieled.GetLength(0); i++)
        {
            for (int j = 0; j < fieldController.CellsOfFieled.GetLength(1); j++)
            {
                //Спавн ассасинов
                if ((j == 4 || j == 6) && (i == 0 || i == 6))
                {
                    InstantiateStaticEnemy(assasinCard, assasinMaterial, fieldController.GetCell(i, j));
                }
                //Спавн голиафов
                if ((j == 4 || j == 6) && (i == 2 || i == 4))
                {
                    InstantiateStaticEnemy(goliafCard, goliafMaterial, fieldController.GetCell(i, j));
                }
                //Спавн элементалей
                if ((j == 2 || j == 8) && i % 2 != 0)
                {
                    InstantiateStaticEnemy(elementalCard, elementalMaterial, fieldController.GetCell(i, j));
                }
            }
        }
        //Спавн двигающихся врагов
        while (count < 5)
        {
            Cell Cell = fieldController.GetCell(Random.Range(0, fieldController.CellsOfFieled.GetLength(0)), Random.Range(0, 2));
            if (!IsEnemyOnCell(Cell))
            {
                EnemyCharacter enemyCharacter = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, Cell.transform);
                enemyCharacter.transform.localPosition = new Vector3(0, 1, 0);
                m_enemyCharObjects.Add(enemyCharacter);

                enemyCharacter.SetData(m_enemyCharCards[count], redMaterial, count);
                count++;
            }
            count2++;
            if (count2 > 100)
            {
                break;
            }
        }

    }
    private bool IsEnemyOnCell(Cell cell)
    {
        if (cell.transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void InstantiateStaticEnemy(CharacterCard characterCard, Material material, Cell cell)
    {
        StaticEnemyCharacter staticEnemyCharacter = Instantiate(staticEnemyPrefab, Vector3.zero, Quaternion.identity, cell.transform);
        staticEnemyCharacter.transform.localPosition = new Vector3(0, 1, 0);
        m_staticEnemyCharObjects.Add(staticEnemyCharacter);

        staticEnemyCharacter.SetData(characterCard, material, m_staticEnemyCharObjects.Count-1);

        SetAttackableCells(enums.Directions.top, staticEnemyCharacter);
        SetAttackableCells(enums.Directions.bottom, staticEnemyCharacter);
        SetAttackableCells(enums.Directions.right, staticEnemyCharacter);
        SetAttackableCells(enums.Directions.left, staticEnemyCharacter);
    }
    private void SetAttackableCells(enums.Directions direction, StaticEnemyCharacter staticEnemyCharacter )
    {
        int newI = (int)staticEnemyCharacter.PositionOnField.x;
        int newJ = (int)staticEnemyCharacter.PositionOnField.y;
        
        for (int i = 0; i < staticEnemyCharacter.Range; i++)
        {
            switch (direction)
            {
                case enums.Directions.top:
                    newI--;
                    break;
                case enums.Directions.bottom:
                    newJ--;
                    break;
                case enums.Directions.right:
                    newI++;
                    break;
                case enums.Directions.left:
                    newJ++;
                    break;
            }
           
            if (newI >= 7 || newI < 0)
            {
                break;
            }
            if (newJ >= 11 || newJ < 0)
            {
                break;
            }

            Cell cell = battleSystem.FieldController.GetCell(newI, newJ);
            
            if (cell.transform.childCount==0)
            {
                staticEnemyCharacter.CellsToAttack.Add(cell);
            }
            
        }
    }


    public void SetCurrentEnemyChosenCharacter(EnemyCharacter character)
    {
        if (character != null)
        {
            if (m_currentEnemyCharacter != null)
            {
                m_currentEnemyCharacter.IsCurrentEnemyCharacter = false;
            }
            m_currentEnemyCharacter = character;
            m_currentEnemyCharacter.IsCurrentEnemyCharacter = true;
        }
        else
        {
            Debug.LogError("Нет персонажа");
        }
    }

   
}
