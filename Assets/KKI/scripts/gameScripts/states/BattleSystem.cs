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

    private float m_pointsOfAction;
    public float PointsOfAction
    {
        get => m_pointsOfAction;
        set => m_pointsOfAction = value;
    }

    public void Init()
    {
        FieldController.InvokeActionOnField(AddOnCellClick);

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

        GameUIPresenter.SetPointsOfActionAnd�ube(cubeValue);
        GameUIPresenter.AddMessageToGameLog($"�� ������ ������ {cubeValue}");

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

        foreach (var cell in FieldController.CellsOfFieled)
        {
            cell.OnClick += SetCurrentChosenCharacterOnCellClick;
        }

        SetPlayerTurn();
        /*if (cubeValue % 2 == 0)
        {
            SetPlayerTurn()
        }
        else
        {
            SetEnemyTurn()
        }*/
    }

    public void SetPlayerTurn()
    {
        EnemyController.gameObject.SetActive(false);
        SetState(new PlayerTurn(this));
    }

    public void SetEnemyTurn()
    {
        EnemyController.gameObject.SetActive(true);
        SetState(new EnemyTurn(this));
    }

    private void SetCurrentChosenCharacterOnCellClick(GameObject cell)
    {
        Character temp = cell.GetComponentInChildren<Character>();
        if (temp!=null)
        {
            SetCurrentChosenCharacter(temp.gameObject);
        }
        
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
            Debug.LogError("��� ���������");
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
            Debug.LogError("��� ���������");
        }
    }

    public PlayerCharacter GetCurrentPlayerChosenCharacter()
    {
        return currentPlayerCharacter;
    }

    public Cell IsCellExist(int i, int j, Vector2 pos)
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
        if (FieldController.GetCell((int)newI, (int)newJ).transform.childCount > 1)
        {
            return null;
        }

        return FieldController.GetCell((int)newI, (int)newJ);
    }

    //������� ���-�� ���������� ������ �� �������������� ����� ������
    
    /*  //������� ��� �������� ������ �� ����������������
      public bool isCell(float cellCoord, float charCoord, int charFeature)
      {
          double numberOfCells = Math.Floor(Mathf.Abs(charCoord - cellCoord));
          if (numberOfCells <= charFeature)
          {
              return true;
          }
          else
          {           
              return false;
          }
      }
      
      //������� ��� ��������� � ���������� ������ ������
      public void isCellEven(bool even, bool isNormal, Cell cell)
      {
          if (isNormal)
          {
              if (even)
              {
                  cell.GetComponent<MeshRenderer>().material.color = this.field.CellsOfFieled[0, 0].baseColor.color;
              }
              else
              {
                  cell.GetComponent<MeshRenderer>().material.color = this.field.CellsOfFieled[0, 0].offsetColor.color;
              }
          }
          else
          {
              if (even)
              {
                  cell.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
              }
              else
              {
                  cell.GetComponent<MeshRenderer>().material.color = new Color(0, 0.5f, 0);
              }
          }
      }
      //�������� ���� �����

      //���� �� ��� �� ������ ������
      private bool isEnemyOnCell(GameObject cell)
      {
          if (cell.transform.childCount != 1)
          {
              return true;
          }
          else
          {
              return false;
          }
      }
      //��������� ���������� � ����� ��� ����� �� ����
      public void cahngeCardWindow(GameObject character, bool isEnemy)
      {
          *//*cardImage.transform.parent.gameObject.SetActive(true);
         healthBar.GetComponent<healthBar>().SetMaxHealth((float)character.GetComponent<character>().card.health);
          healthBar.GetComponent<healthBar>().SetHealth((float)character.GetComponent<character>().health);
          physAttack.text = $"���������� ����� {character.GetComponent<character>().physAttack * 100}";
          magAttack.text = $"���������� ����� {character.GetComponent<character>().magAttack * 100}";
          physDefence.text = $"���������� ������ {character.GetComponent<character>().physDefence * 100}";
          magDefence.text = $"���������� ����� {character.GetComponent<character>().magDefence * 100}";
          cardImage.sprite = character.GetComponent<character>().card.image;
          switch (character.GetComponent<character>().Class)
          {
              case enums.Classes.�������:
                  Class.sprite = classesSprite[0];
                  break;
              case enums.Classes.������:
                 Class.sprite = classesSprite[1];
                  break;
              case enums.Classes.���:
                  Class.sprite = classesSprite[2];
                  break;
              case enums.Classes.���������:
                  Class.sprite = classesSprite[3];
                  break;
          }
          switch (character.GetComponent<character>().race)
          {
              case enums.Races.����:
                  rase.text = "�";
                  break;
              case enums.Races.�����:
                  rase.text = "�";
                  break;
              case enums.Races.�����:
                  rase.text = "�";
                  break;
              case enums.Races.Ҹ���������:
                  rase.text = "�";
                  break;
              case enums.Races.������������������:
                  rase.text = "�";
                  break;
          }
          if (isEnemy||character.GetComponent<character>().isStaticEnemy)
          {
              cardImage.transform.parent.transform.parent.GetComponent<Image>().color = new Color(0.9921569f, 0.8740318f, 0.8666667f, 1);
          }
          else
          {
              cardImage.transform.parent.transform.parent.GetComponent<Image>().color = new Color(0.8707209f, 0.9921569f, 0.8666667f,1);
          }
          for (int i = 0; i < EnemyCharObjects.Count; i++)
          {
              EnemyCharObjects[i].GetComponent<Outline>().enabled =false;
          }
          for (int i = 0; i < EnemyStaticCharObjects.Count; i++)
          {
              EnemyStaticCharObjects[i].GetComponent<Outline>().enabled = false;
          }
          character.GetComponent<Outline>().enabled = true;*//*
      }
      //���������� ���� ������
      public void endMove()
      {        
          SetState(new EnemyTurn(this));
          enemyManager.enabled = true;
          isEnemyTurn = true;
          enemyManager.gameObject.SetActive(true);
          enemyManager.RestartTree();
      }
      public void endEnemyMove()
      {
          enemyManager.enabled = false;
          isEnemyTurn = false;
          enemyManager.gameObject.SetActive(false);
          enemyManager.StopTree();
          SetState(new PlayerTurn(this));
      }*/
}
