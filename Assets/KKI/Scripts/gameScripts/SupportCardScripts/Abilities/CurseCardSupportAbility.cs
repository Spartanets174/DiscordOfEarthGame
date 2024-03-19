using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CurseCardSupportAbility : BaseSupport—ardAbility, ITurnCountable
{
    [SerializeField]
    private int pointsOfAction;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff;}

    private List<EnemyCharacter> enemyCharacters;
    private List<PlayerCharacter> playerCharacters;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("»ÒÔÓÎ¸ÁÛÈÚÂ Í‡ÚÛ"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        battleSystem.PointsOfAction.Value += pointsOfAction;

        if (battleSystem.State is PlayerTurn)
        {
            enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x => x.Class == enums.Classes.œ‡Î‡‰ËÌ).ToList();
            foreach (var enemyCharacter in enemyCharacters)
            {
                enemyCharacter.IsFreezed = true;
            }
        }
        else
        {
            playerCharacters = battleSystem.PlayerController.PlayerCharactersObjects.Where(x => x.Class == enums.Classes.œ‡Î‡‰ËÌ).ToList();
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
