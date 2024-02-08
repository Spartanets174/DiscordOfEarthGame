using BehaviourTree;
using System.Collections.Generic;
using UnityEngine;
using Tree = BehaviourTree.Tree;

public class EnemyController : Tree, ILoadable
{
    [Header("Main sripts")]
    [SerializeField]
    private BattleSystem battleSystem;
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

    private FieldController fieldController;
    public void Init()
    {
        fieldController = battleSystem.FieldController;
    }

    protected override Node SetupTree()
    {
        Node root = new Sequence(new List<Node>
                {
                    new ChooseChar(battleSystem,true),
                    new Selector(new List<Node>
                    {
                        new Sequence(new List<Node>{
                            new checkCellsForAttack(battleSystem,this),
                            new Attack(battleSystem,this),
                        }),
                        new Sequence(new List<Node>{
                            new checkCellsForMove(battleSystem,this),
                            new Movement(battleSystem,this),
                        }),
                    })

                });
        return root;
    }


    public void CreateEnemy()
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

    private void InstantiateStaticEnemy(CharacterCard characterCard, Material material, Cell cell)
    {
        StaticEnemyCharacter staticEnemyCharacter = Instantiate(staticEnemyPrefab, Vector3.zero, Quaternion.identity, cell.transform);
        staticEnemyCharacter.transform.localPosition = new Vector3(0, 1, 0);
        m_staticEnemyCharObjects.Add(staticEnemyCharacter);

        staticEnemyCharacter.SetData(characterCard, material, m_staticEnemyCharObjects.Count-1);
      
     /*   //Заполнение листа с клетками для атаки
        EnemyStaticCharObjects[EnemyStaticCharObjects.Count - 1].GetComponent<staticEnemyAttack>().listOfCellToAttack.Add(fieldController.CellsOfFieled[i, j + 1].gameObject);
        EnemyStaticCharObjects[EnemyStaticCharObjects.Count - 1].GetComponent<staticEnemyAttack>().listOfCellToAttack.Add(fieldController.CellsOfFieled[i, j - 1].gameObject);
        EnemyStaticCharObjects[EnemyStaticCharObjects.Count - 1].transform.GetChild(0).gameObject.SetActive(false);*/
    }

    public void InstantiateEnemies()
    {
        int count = 0;
        int count2 = 0;
        //Спавн двигающихся врагов
        while (count < 5)
        {
            Cell Cell = fieldController.GetCell(Random.Range(0, fieldController.CellsOfFieled.GetLength(0)), Random.Range(0, 2));
            if (!IsEnemyOnCell(Cell))
            {
                EnemyCharacter enemyCharacter = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, Cell.transform);
                enemyCharacter.transform.localPosition = new Vector3(0, 1, 0);
                m_enemyCharObjects.Add(enemyCharacter);

                enemyCharacter.SetData(m_enemyCharCards[count],redMaterial, count);
                count++;
            }
            count2++;
            if (count2>100)
            {
                break;
            }
        }
        //Спавн статических врагов
        for (int i = 0; i < fieldController.CellsOfFieled.GetLength(0); i++)
        {
            for (int j = 0; j < fieldController.CellsOfFieled.GetLength(1); j++)
            {
                //Спавн ассасинов
                if ((j == 4 || j == 6) && (i == 0 || i == 6))
                {
                    InstantiateStaticEnemy(assasinCard, assasinMaterial, fieldController.GetCell(i,j));                   
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
    }
    private bool IsEnemyOnCell(Cell cell)
    {
        Debug.Log(cell.transform.childCount);
        if (cell.transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
