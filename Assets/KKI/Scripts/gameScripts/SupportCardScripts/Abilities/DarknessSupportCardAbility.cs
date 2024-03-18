using System;
using UnityEngine;

[Serializable]
public class DarknessSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
{
    [SerializeField]
    private float characterMagAttack;
    [SerializeField]
    private float enemyCharacterMagDefence;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    private Character character;

    private Character enemyCharacter;
    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Âûáåðèòå ñîþçíîãî ïåðñîíàæà", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Âûáåðèòå âðàæåñêîãî ïåðñîíàæà", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSecondSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_cardSecondSelectBehaviour.OnSelected += OnSecondSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.OnClick += SelectSecondCharacterInvoke;
            }

        }
    }

    private void SelectSecondCharacterInvoke(GameObject gameObject)
    {
        if (battleSystem.State is PlayerTurn)
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }
        else
        {
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }

        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectSecondCharacterInvoke;
        });

        battleSystem.EnemyController.SetEnemiesState(true);
        SelectSecondCard();
    }



    private void OnSecondSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            battleSystem.EnemyController.SetEnemiesState(false, (x) =>
            {
                enemyCharacter.OnClick += SelectCharacter;
            });
        }
    }

    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            enemyCharacter = battleSystem.EnemyController.CurrentEnemyCharacter;
        }
        else
        {
            enemyCharacter = battleSystem.PlayerController.CurrentPlayerCharacter;
        }

        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.OnClick -= SelectCharacter;
        }

        character.MagAttack += characterMagAttack;
        enemyCharacter.MagDefence -= enemyCharacterMagDefence;

        UseCard(null);
    }

    private void OnCancelSelection()
    {
        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectSecondCharacterInvoke;
        });

        battleSystem.EnemyController.SetEnemiesChosenState(false, x => { x.OnClick -= SelectCharacter; });
    }

    public void ReturnToNormal()
    {
        character.MagAttack -= characterMagAttack;
        enemyCharacter.MagDefence += enemyCharacterMagDefence;

        OnReturnToNormal?.Invoke(this);
    }
}