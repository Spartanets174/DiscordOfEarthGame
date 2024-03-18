using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialBladesSecondSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private List<Character> characters = new();

    public event Action<ITurnCountable> OnReturnToNormal;
    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Âûáåðèòå âðàæåñêîãî ïåðñîíàæà äëÿ àòàêè", battleSystem));
        SetSecondCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Âûáåðèòå âðàæåñêîãî ïåðñîíàæà", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("", battleSystem));
        SetUseCardBehaviour(new AttackSelectedÑharactersBehaviour(damage, battleSystem, "\"Ìàòåðèàëüíûå êëèíêè\""));

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
            {
                enemyCharacter.OnClick += SelectCharacter;
            }
        }
    }


    private void OnSelectCharacter()
    {
        if (battleSystem.State is PlayerTurn)
        {
            characters.Add(battleSystem.EnemyController.CurrentEnemyCharacter);
        }
        else
        {
            characters.Add(battleSystem.PlayerController.CurrentPlayerCharacter);
        }

        if (characters.Count >= 2)
        {
            foreach (var character in characters)
            {
                character.IsFreezed = true;
                UseCard(character.gameObject);
            }
        }
        else
        {
            SelectSecondCard();
        }
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        battleSystem.EnemyController.SetEnemiesState(false, x =>
        {
            x.OnClick += SelectCharacter;
            x.IsChosen = false;
        });
    }
    public void ReturnToNormal()
    {
        foreach (var character in characters)
        {
            character.IsFreezed = false;
        }

    }
}
