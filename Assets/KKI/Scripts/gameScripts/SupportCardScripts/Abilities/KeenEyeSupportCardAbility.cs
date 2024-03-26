using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class KeenEyeSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
{
    [SerializeField]
    private float critChance;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private List<EnemyCharacter> enemyCharacters;
    private List<PlayerCharacter> playerCharacters;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàðòó"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            playerCharacters = battleSystem.PlayerController.PlayerCharactersObjects.Where(x => x.Class == Enums.Classes.Ëó÷íèê).ToList();
            foreach (var playerCharacter in playerCharacters)
            {
                playerCharacter.CritChance = critChance;
            }
        }
        else
        {
            enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x => x.Class == Enums.Classes.Ëó÷íèê).ToList();
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.CritChance = critChance;
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