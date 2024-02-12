using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class BattleSystem : StateMachine, ILoadable
{
    [SerializeField]
    private EnemyController enemyController;
    public EnemyController EnemyController => enemyController;
    [SerializeField]
    private FieldController field;
    public FieldController FieldController => field;
    [SerializeField]
    private GameUIPresenter gameUIPresenter;
    public GameUIPresenter GameUIPresenter => gameUIPresenter;


    [Space, Header("Prefabs")]
    [SerializeField]
    private PlayerCharacter charPrefab;
    public PlayerCharacter CharPrefab => charPrefab;

    private List<PlayerCharacter> m_playerCharactersObjects=new();
    public List<PlayerCharacter> PlayerCharactersObjects => m_playerCharactersObjects;


    private Character currentChosenCharacter;
    private PlayerCharacter currentPlayerCharacter;
    public PlayerCharacter CurrentPlayerCharacter => currentPlayerCharacter;

    private float m_pointsOfAction;
    public float PointsOfAction
    {
        get => m_pointsOfAction;
        set
        {
            m_pointsOfAction = value;
            gameUIPresenter.SetPointsOfActionAnd—ube(m_pointsOfAction);
        }
    }

    public void Init()
    {
        FieldController.InvokeActionOnField(AddOnCellClick);
        gameUIPresenter.EndMoveButton.onClick.AddListener(SetEnemyTurn);

        SetState(new Begin(this));
    }

    private void AddOnCellClick(Cell cell)
    {
        cell.OnClick += FieldController.TurnOnCells;
    }

    public void OnUnitStatementButton(GameObject character)
    {
        StartCoroutine(State.UnitStatement(character));
    }
    public void OnChooseCharacterButton(GameObject character)
    {
        StartCoroutine(State.ChooseCharacter(character));
    }
    public void OnMoveButton(GameObject cell)
    {
        StartCoroutine(State.Move(cell));
    }

    public void OnAttackButton(GameObject target)
    {
        StartCoroutine(State.Attack(target));
    }

    public void OnAttackAbilityButton()
    {
        StartCoroutine(State.UseAttackAbility());
    }

    public void OnDefensiveAbilityButton()
    {
        StartCoroutine(State.UseDefensiveAbility());
    }

    public void OnBuffAbilityButton()
    {
        StartCoroutine(State.UseBuffAbility());
    }
    public void OnSupportCardButton()
    {
        StartCoroutine(State.UseSupportCard());
    }
    public void OnUseItemButton()
    {
        StartCoroutine(State.UseItem());
    }


    public PlayerCharacter InstasiatePlayerCharacter(CharacterCard characterCard, Transform parent)
    {
        PlayerCharacter prefab = Instantiate(CharPrefab, Vector3.zero, Quaternion.identity, parent);
        prefab.transform.localPosition = new Vector3(0, 1, 0);
        m_playerCharactersObjects.Add(prefab);

        prefab.SetData(characterCard,null, m_playerCharactersObjects.Count-1);

        GameUIPresenter.SetChosenStateToCards(false);
        GameUIPresenter.EbableUnspawnedCards();
        if (m_playerCharactersObjects.Count == 5)
        {
            StartGame();
        }

        return prefab;
    }
   
    private void StartGame()
    {
        int cubeValue = UnityEngine.Random.Range(1, 6);

        GameUIPresenter.SetPointsOfActionAnd—ube(cubeValue);
        GameUIPresenter.AddMessageToGameLog($"Õ‡ ÍÛ·ËˆÂ ‚˚Ô‡ÎÓ {cubeValue}");

        EnemyController.gameObject.SetActive(false);

        foreach (var playerChar in m_playerCharactersObjects)
        {
            playerChar.OnClick += SetCurrentChosenCharacter;
            playerChar.OnClick += SetCurrentPlayerChosenCharacter;
            playerChar.IsEnabled = true;
        }
        foreach (var enemyChar in EnemyController.EnemyCharObjects)
        {
            enemyChar.OnClick += SetCurrentChosenCharacter;
            enemyChar.IsEnabled = true;
        }
        foreach (var staticEnemyChar in EnemyController.StaticEnemyCharObjects)
        {
            staticEnemyChar.OnClick += SetCurrentChosenCharacter;
            staticEnemyChar.IsEnabled = true;
        }

        if (cubeValue % 2 == 0)
        {
            SetPlayerTurn();
        }
        else
        {
            SetEnemyTurn();
        }
    }

    public void SetPlayerTurn()
    {
        EnemyController.gameObject.SetActive(false);
        EnemyController.StopTree();
        SetState(new PlayerTurn(this));
    }

    public void SetEnemyTurn()
    {
        EnemyController.gameObject.SetActive(true);
        SetState(new EnemyTurn(this));
    }

    private void SetCurrentChosenCharacter(GameObject character)
    {
        if (character != null)
        {
            if (currentChosenCharacter != null)
            {
                currentChosenCharacter.IsChosen = false;
            }           
            currentChosenCharacter = character.GetComponent<Character>();
            currentChosenCharacter.IsChosen = true;
            GameUIPresenter.SetChosenCharDeatils(currentChosenCharacter);
        }
        else
        {
            Debug.LogError("ÕÂÚ ÔÂÒÓÌ‡Ê‡");
        }
    }

    private void SetCurrentPlayerChosenCharacter(GameObject character)
    {
        if (character != null)
        {
            if (currentPlayerCharacter != null)
            {
                currentPlayerCharacter.IsCurrentPlayerCharacter = false;
            }
            currentPlayerCharacter = character.GetComponent<PlayerCharacter>();
            currentPlayerCharacter.IsCurrentPlayerCharacter = true;
        }
        else
        {
            Debug.LogError("ÕÂÚ ÔÂÒÓÌ‡Ê‡");
        }
    }

    private Cell GetCellForMove(int i, int j, Vector2 pos)
    {
        float newI = pos.x + i;
        float newJ = pos.y + j;
        if (newI >= 7 || newI < 0)
        {
            return null;
        }
        if (newJ >= 11 || newJ < 0)
        {
            return null;
        }
        if (FieldController.GetCell((int)newI, (int)newJ).transform.childCount > 0)
        {
            return null;
        }

        return FieldController.GetCell((int)newI, (int)newJ);
    }

    public List<Cell> GetCellsForMove(Character character)
    {
        List<Cell> cellsToMove = new();
        bool top = true;
        bool bottom = true;
        bool left = true;
        bool rigth = true;
        for (int i = 1; i <= character.Speed; i++)
        {
            Cell topCell = GetCellForMove(-i, 0, character.PositionOnField);
            Cell bottomCell = GetCellForMove(0, -i, character.PositionOnField);
            Cell leftCell = GetCellForMove(i, 0, character.PositionOnField);
            Cell rigtCell = GetCellForMove(0, i, character.PositionOnField);

            top = topCell != null && top;
            bottom = bottomCell != null && bottom;
            rigth = rigtCell != null && rigth;
            left = leftCell != null && left;

            SetActiveCell(topCell, top, cellsToMove);
            SetActiveCell(bottomCell, bottom, cellsToMove);
            SetActiveCell(rigtCell, rigth, cellsToMove);
            SetActiveCell(leftCell, left, cellsToMove);
        }
        return cellsToMove;
    }

    private void SetActiveCell(Cell cell, bool isAllowed, List<Cell> cellsToMove)
    {
        if (cell != null && isAllowed)
        {
            if (State is PlayerTurn)
            {
                cell.SetCellMovable();
            }           
            cellsToMove.Add(cell);
        }
    }
    
}
