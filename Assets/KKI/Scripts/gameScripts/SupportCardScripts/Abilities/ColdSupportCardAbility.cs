using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColdSupportCardAbility : BaseSupport�ardAbility, ITurnCountable
{
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private List<EnemyCharacter> enemyCharacters;
    private List<PlayerCharacter> playerCharacters;

    public event Action<ITurnCountable> OnReturnToNormal;

    protected override void Start()
    {

        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("����������� �����"));
        TurnCount = 1;
        m_isBuff = false;
        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnDestroy()
    {
        m_cardSelectBehaviour.OnSelected -= OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x => x.Class == enums.Classes.������).ToList();
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.IsFreezed = true;
            }
        }
        else
        {
            playerCharacters = battleSystem.PlayerController.PlayerCharactersObjects.Where(x => x.Class == enums.Classes.������).ToList();
            foreach (var playerCharacter in playerCharacters)
            {
                playerCharacter.IsFreezed = true;
            }
        }

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }

    public void ReturnToNormal()
    {
        if (battleSystem.State is PlayerTurn)
        {          
            foreach (var playerCharacter in playerCharacters)
            {
                playerCharacter.IsFreezed = false;
            }
        }
        else
        {
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.IsFreezed = false;
            }
        }
        OnReturnToNormal?.Invoke(this);
    }
}
