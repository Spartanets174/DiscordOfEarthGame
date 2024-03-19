using System;
using System.Collections.Generic;
using UniRx;
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
   /* [SerializeField]
    private GameUICoordinator gameUIPresenter;
    public GameUICoordinator GameUIPresenter => gameUIPresenter;*/

    private Dictionary<ITurnCountable, int> m_playerTurnCountables = new();
    public Dictionary<ITurnCountable, int> PlayerTurnCountables => m_playerTurnCountables;

    private Dictionary<ITurnCountable, int> m_enemyTurnCountables = new();
    public Dictionary<ITurnCountable, int> EnemyTurnCountables => m_enemyTurnCountables;

    public ReactiveProperty<Character> CurrentChosenCharacter = new ReactiveProperty<Character>();

    public ReactiveProperty<int> PointsOfAction = new ReactiveProperty<int>();

    public ReactiveProperty<string> gameLogCurrentText = new ReactiveProperty<string>();

    public event Action<Begin> OnGameStarted;
    public event Action OnGameEnded;
    public event Action<PlayerTurn> OnPlayerTurnStarted;
    public event Action<EnemyTurn> OnEnemyTurnStarted;

    private CompositeDisposable disposables = new();
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
        FieldController.InvokeActionOnField(x => x.OnClick += x=> FieldController.TurnOnCells());
        PlayerController.PlayerCharactersObjects.ObserveCountChanged().Subscribe(x =>
        {
            if (x == 5)
            {
                StartGame();
            }
            if (x == 0)
            {
                SetLost();
            }
        }).AddTo(disposables);

        EnemyController.EnemyCharObjects.ObserveCountChanged().Subscribe(x =>
        {
            if (x==0)
            {
                SetWin();
            }
        }).AddTo(disposables);

        Begin begin = new(this);
        SetState(begin);
        OnGameStarted?.Invoke(begin);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        disposables.Clear();
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
        PlayerTurn playerTurn = new(this);
        OnPlayerTurnStarted?.Invoke(playerTurn);
        SetState(playerTurn);
    }
    [ContextMenu("SetEnemyTurn")]
    public void SetEnemyTurn()
    {
        EnemyTurn enemyTurn = new(this);
        OnEnemyTurnStarted?.Invoke(enemyTurn);
        SetState(enemyTurn);
    }
    public void SetWin()
    {
        EnemyController.StopTree();
        PlayerController.ClearDisposables();
        SetState(new Won(this));
        OnGameEnded?.Invoke();
    }
    public void SetLost()
    {
        EnemyController.StopTree();
        PlayerController.ClearDisposables();
        SetState(new Lost(this));
        OnGameEnded?.Invoke();
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

        gameLogCurrentText.Value = $"�� ������ ������ {cubeValue}";

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
            if (CurrentChosenCharacter.Value != null)
            {
                CurrentChosenCharacter.Value.IsChosen = false;
            }
            CurrentChosenCharacter.Value = character.GetComponent<Character>();
            CurrentChosenCharacter.Value.IsChosen = true;
        }
        else
        {
            Debug.LogError("��� ���������");
        }
    }
}
