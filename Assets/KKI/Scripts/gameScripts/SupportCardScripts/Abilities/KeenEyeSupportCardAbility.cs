using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class KeenEyeSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
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

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàðòó"));
        TurnCount = 1;
        m_isBuff = true;
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
            playerCharacters = battleSystem.PlayerController.PlayerCharactersObjects.Where(x => x.Class == enums.Classes.Ëó÷íèê).ToList();
            foreach (var playerCharacter in playerCharacters)
            {
                playerCharacter.CritChance = 0.6f;
            }
        }
        else
        {
            enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x => x.Class == enums.Classes.Ëó÷íèê).ToList();
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.CritChance = 0.6f;
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
                playerCharacter.CritChance = playerCharacter.Card.critChance;
            }
        }
        else
        {
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.CritChance = enemyCharacter.Card.critChance;
            }
        }
        OnReturnToNormal?.Invoke(this);
    }
}