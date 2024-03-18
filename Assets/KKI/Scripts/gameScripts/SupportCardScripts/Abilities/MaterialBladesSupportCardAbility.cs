using System;
using UnityEngine;

[Serializable]
public class MaterialBladesSupportCardAbility : BaseSupportÑardAbility, ITurnCountable
{ 
    [SerializeField]
    private float damage;

    [SerializeField]
    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    [SerializeField]
    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private Character character;

    public event Action<ITurnCountable> OnReturnToNormal;

    public override void Init(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllEnemyUnitsBehaviour("Âûáåðèòå âðàæåñêîãî ïåðñîíàæà äëÿ àòàêè", battleSystem));
        SetSelectCharacterBehaviour(new SetCurrentEnemyCharacterBehaviour("Âûáåðèòå âòîðîãî âðàæåñêîãî ïåðñîíàæà äëÿ àòàêè", battleSystem));
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
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
        }
        else
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
        }

        character.IsFreezed = true;

        UseCard(character.gameObject);
    }

    private void OnCardUse()
    {
        OnCancelSelection();
    }

    private void OnCancelSelection()
    {
        battleSystem.EnemyController.SetEnemiesState(true, x =>
        {
            x.OnClick += SelectCharacter;
            x.IsChosen = false;
        });
    }
    public void ReturnToNormal()
    {
        character.IsFreezed = false;
    }
}
