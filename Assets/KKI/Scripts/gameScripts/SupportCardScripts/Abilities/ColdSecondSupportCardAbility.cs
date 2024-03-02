using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColdSecondSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
{
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private List<EnemyCharacter> enemyCharacters;

    public event Action<ITurnCountable> OnReturnToNormal;

    protected override void Start()
    {

        base.Start();

        SetCardSelectBehaviour(new EmptySelectBehaviour("Èñïîëüçóéòå êàðòó"));
        TurnCount = 2;
        m_isBuff = false;
        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {      
        enemyCharacters = battleSystem.EnemyController.EnemyCharObjects.Where(x => x.Class == enums.Classes.Ëó÷íèê).ToList();

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
