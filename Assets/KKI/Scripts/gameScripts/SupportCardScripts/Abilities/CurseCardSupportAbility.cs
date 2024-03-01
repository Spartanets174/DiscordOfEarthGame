using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurseCardSupportAbility : BaseSupport—ardAbility, ITurnCountable
{
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff;}

    private List<EnemyCharacter> enemyCharacters;

    public event Action<ITurnCountable> OnReturnToNormal;

    protected override void Start()
    {
        
        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("»ÒÔÓÎ¸ÁÛÈÚÂ Í‡ÚÛ"));

        m_isBuff = false;
        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        battleSystem.PointsOfAction += 1;

        TurnCount = 1;
              
        enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x=>x.Class==enums.Classes.œ‡Î‡‰ËÌ).ToList();

        foreach (var enemyCharacter in enemyCharacters)
        {
            enemyCharacter.IsFreezed = true;
        }

        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(null);
    }

    public void ReturnToNormal()
    {
        foreach (var enemyCharacter in enemyCharacters)
        {
            enemyCharacter.IsFreezed = false;
        }
        OnReturnToNormal?.Invoke(this);
    }

}
