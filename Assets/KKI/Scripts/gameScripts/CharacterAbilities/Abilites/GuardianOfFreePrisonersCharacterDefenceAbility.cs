using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GuardianOfFreePrisonersCharacterDefenceAbility : BaseCharacterAbility, ITurnCountable
{
    [SerializeField]
    private float magDefenceAmount;

    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new EmptySelectBehaviour("Используйте карту"));

        m_cardSelectBehaviour.OnSelected += OnSelected;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var character in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                if (character.Race == Enums.Races.МагическиеСущества)
                {
                    character.PhysDefence += physDefenceAmount;
                    character.MagDefence += magDefenceAmount;
                }
            }
        }
        else
        {
            foreach (var character in battleSystem.EnemyController.EnemyCharObjects)
            {
                if (character.Race == Enums.Races.МагическиеСущества)
                {
                    character.PhysDefence += physDefenceAmount;
                    character.MagDefence += magDefenceAmount;
                }
            }
        }
        m_cardSelectBehaviour.OnSelected -= OnSelected;
        UseCard(abilityOwner.gameObject);

    }

    public void ReturnToNormal()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var character in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                if (character.Race == Enums.Races.МагическиеСущества)
                {
                    character.PhysDefence -= physDefenceAmount;
                    character.MagDefence -= magDefenceAmount;
                }
            }
        }
        else
        {
            foreach (var character in battleSystem.EnemyController.EnemyCharObjects)
            {
                if (character.Race == Enums.Races.МагическиеСущества)
                {
                    character.PhysDefence -= physDefenceAmount;
                    character.MagDefence -= magDefenceAmount;
                }
            }
        }

        OnReturnToNormal?.Invoke(this);
    }

}
