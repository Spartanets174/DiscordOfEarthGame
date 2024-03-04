using UnityEngine;

public class SecretPathSupportCardAbility : BaseSupport�ardAbility
{
    private SetAbiableCellsBehaviour setAbiableCellsBehaviour;
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("�������� ���������", battleSystem));
        SetSelectCharacterBehaviour(new SetAbiableCellsBehaviour("�������� ������ ��� �����������", battleSystem));
        SetUseCardBehaviour(new MoveToCellBehaviour(battleSystem));

        setAbiableCellsBehaviour = (SetAbiableCellsBehaviour)SelectCharacterBehaviour;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnCancelSelection()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.OnClick += SetCellsToMove;
                playerCharacter.OnClick += SelectCharacter;
            }
        }

    }

    private void SetCellsToMove(GameObject @object)
    {
        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= UseCard;
        }
        setAbiableCellsBehaviour.cellsToMove = battleSystem.GetCellsForMove(@object.GetComponent<Character>(), 3);
    }
    private void OnSelectCharacter()
    {

        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.IsEnabled = true;
        }
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var item in setAbiableCellsBehaviour.cellsToMove)
            {
                item.OnClick += UseCard;
            }
        }


    }

    private void OnCardUse()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCellsToMove;
            playerCharacter.OnClick -= SelectCharacter;
        }

        foreach (var item in setAbiableCellsBehaviour.cellsToMove)
        {
            item.OnClick -= UseCard;
        }
    }
}
