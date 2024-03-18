using System.Collections.Generic;
using UnityEngine;


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

    private Dictionary<ITurnCountable, int> m_playerTurnCountables = new();
    public Dictionary<ITurnCountable, int> PlayerTurnCountables => m_playerTurnCountables;

    private Dictionary<ITurnCountable, int> m_enemyTurnCountables = new();
    public Dictionary<ITurnCountable, int> EnemyTurnCountables => m_enemyTurnCountables;

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

    private static BattleSystem m_instance;
    public static BattleSystem Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<BattleSystem>();
            }
            return m_instance;
        }
    }

    public void Init()
    {
        m_instance = this;
        FieldController.InvokeActionOnField(x => x.OnClick += TurnOnCells);
        PlayerController.OnPlayerCharacterSpawned += OnPlayerCharacterSpawned;

        foreach (var item in gameUIPresenter.GameSupportCards)
        {
            item.DragAndDropComponent.OnDropEvent += OnSupportCardButton;
            item.DragAndDropComponent.OnDropEvent += TurnOnCells;
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
    public void OnSupportCardButton(GameObject cardSupport)
    {
        StartCoroutine(State.UseSupportCard(cardSupport));
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

    public void OnUseItemButton()
    {
        StartCoroutine(State.UseItem());
    }
    [ContextMenu("SetPlayerTurn")]
    public void SetPlayerTurn()
    {
        EnemyController.StopTree();
        SetState(new PlayerTurn(this));
    }
    [ContextMenu("SetEnemyTurn")]
    public void SetEnemyTurn()
    {
        SetState(new EnemyTurn(this));
    }
    public void SetWin()
    {
        EnemyController.StopTree();
        PlayerController.ClearDisposables();
        SetState(new Won(this));
    }
    public void SetLost()
    {
        EnemyController.StopTree();
        PlayerController.ClearDisposables();
        SetState(new Lost(this));
    }
    private void TurnOnCells(GameObject gameObject)
    {
        FieldController.TurnOnCells();
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

    private void StartGame()
    {
        PlayerController.SetPlayerState(true, x =>
        {
            x.OnClick += SetCurrentChosenCharacter;
            x.OnPositionOnFieldChanged += EnemyController.AttackPlayerCharacterOnMove;
        });

        EnemyController.SetEnemiesState(true, (x) =>
        {
            x.OnClick += SetCurrentChosenCharacter;
            x.OnPositionOnFieldChanged += EnemyController.AttackEnemyCharacterOnMove;
        });
        EnemyController.SetStaticEnemiesState(true, (x) => { x.OnClick += SetCurrentChosenCharacter; });

        int cubeValue = UnityEngine.Random.Range(1, 6);

        GameUIPresenter.SetPointsOfActionAnd—ube(cubeValue);
        GameUIPresenter.AddMessageToGameLog($"Õ‡ ÍÛ·ËˆÂ ‚˚Ô‡ÎÓ {cubeValue}");

        if (cubeValue % 2 == 0)
        {
            SetPlayerTurn();
        }
        else
        {
            SetEnemyTurn();
        }
    }

    public void SetCurrentChosenCharacter(GameObject character)
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
}
