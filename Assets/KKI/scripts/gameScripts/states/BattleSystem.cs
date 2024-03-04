using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class BattleSystem : StateMachine, ILoadable
{
    [Header("Controllers")]
    [SerializeField]
    private PlayerController m_playerController;
    public PlayerController PlayerController => m_playerController;
    [SerializeField] 
    private EnemyController enemyController;
    public EnemyController EnemyController => enemyController;
    [SerializeField]
    private FieldController m_fieldController;
    public FieldController FieldController => m_fieldController;
    [SerializeField]
    private GameUIPresenter gameUIPresenter;
    public GameUIPresenter GameUIPresenter => gameUIPresenter;

    private List<ITurnCountable> m_playerTurnCountables=new();
    public List<ITurnCountable> PlayerTurnCountables => m_playerTurnCountables;

    private List<ITurnCountable> m_enemyTurnCountables=new();
    public List<ITurnCountable> EnemyTurnCountables => m_enemyTurnCountables;

    private Character currentChosenCharacter;
   

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
        PlayerController.OnPlayerCharacterSpawned += OnPlayerCharacterSpawned;

        foreach (var item in gameUIPresenter.GameSupportCards)
        {
            item.DragAndDropComponent.OnDropEvent += OnSupportCardButton;
            item.DragAndDropComponent.OnDropEvent += x=>{ FieldController.TurnOnCells(); };
        }

        SetState(new Begin(this));
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
    public void OnSupportCardButton(GameObject cardSupport)
    {
        StartCoroutine(State.UseSupportCard(cardSupport));
    }
    public void OnUseItemButton()
    {
        StartCoroutine(State.UseItem());
    }

    private void OnPlayerCharacterSpawned()
    {
        GameUIPresenter.SetChosenStateToCards(false);
        GameUIPresenter.EbableUnspawnedCards();
        if (PlayerController.PlayerCharactersObjects.Count == 5)
        {
            StartGame();
        }
    }

    private void AddOnCellClick(Cell cell)
    {
        cell.OnClick += x => { FieldController.TurnOnCells(); };
    }

    private void StartGame()
    {
        int cubeValue = UnityEngine.Random.Range(1, 6);

        GameUIPresenter.SetPointsOfActionAnd—ube(cubeValue);
        GameUIPresenter.AddMessageToGameLog($"Õ‡ ÍÛ·ËˆÂ ‚˚Ô‡ÎÓ {cubeValue}");

        EnemyController.gameObject.SetActive(false);

        foreach (var playerChar in PlayerController.PlayerCharactersObjects)
        {
            playerChar.OnClick += SetCurrentChosenCharacter;
            playerChar.OnClick += PlayerController.SetCurrentPlayerChosenCharacter;
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

    [ContextMenu("SetPlayerTurn")]
    public void SetPlayerTurn()
    {
        EnemyController.gameObject.SetActive(false);
        EnemyController.StopTree();
        SetState(new PlayerTurn(this));
    }
    [ContextMenu("SetEnemyTurn")]
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

    public List<Cell> GetCellsForMove(Character character, int numberOfCells)
    {
        List<Cell> cellsToMove = new();
        bool top = true;
        bool bottom = true;
        bool left = true;
        bool rigth = true;
        for (int i = 1; i <= numberOfCells; i++)
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
