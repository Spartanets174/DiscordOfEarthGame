using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSupportCardAbility : BaseSupportСardAbility
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите персонажа", battleSystem));
        SetSelectCharacterBehaviour(new HighlightAbiableCellsBehaviour("Выберите клетку для перемещения", battleSystem));
        SetUseCardBehaviour(new MoveToCellBehaviour(battleSystem));

        cardSelectBehaviour.OnSelected += OnSelected;
        selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
        useCardBehaviour.OnCardUse += OnCardUse;
    }

    public override void SelectCard()
    {
        cardSelectBehaviour.SelectCard();
    }

    public override void SelectCharacter(GameObject gameObject)
    {
        selectCharacterBehaviour.SelectCharacter(gameObject);
    }

    public override void UseCard(GameObject gameObject)
    {
        useCardBehaviour.UseAbility(gameObject);
    }

    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            PlayerTurn playerTurn = (PlayerTurn)battleSystem.State;
            playerTurn.OnPlayerTurnCompleted();
        }       

        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick += SelectCharacter;
        }
    }
    private void OnSelectCharacter()
    {
        foreach (var enemyCharacter in battleSystem.EnemyController.EnemyCharObjects)
        {
            enemyCharacter.IsEnabled = true;
        }

        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.IsChosen = false;
        }
        foreach (var playerCharacter in battleSystem.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectCharacter;
        }

        foreach (var cell in battleSystem.FieldController.CellsOfFieled)
        {
            if (cell.transform.childCount==0)
            {
                cell.OnClick += UseCard;
            }
        }
    }

    private void OnCardUse()
    {
        if (battleSystem.State is PlayerTurn)
        {
            PlayerTurn playerTurn = (PlayerTurn)battleSystem.State;
            playerTurn.OnPlayerTurnStarted();
            battleSystem.CurrentPlayerCharacter.IsChosen = false;
        }
        else
        {
            EnemyTurn enemyTurn = (EnemyTurn)battleSystem.State;
            battleSystem.EnemyController.CurrentEnemyCharacter.IsChosen = false;
        }
        foreach (var cell in battleSystem.FieldController.CellsOfFieled)
        {
            cell.OnClick -= UseCard;
        }
        OnSupportCardAbilityUsedInvoke();
    }
}
