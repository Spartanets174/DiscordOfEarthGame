using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ForestLadsDefenceAbility : BaseCharacterAbility
{
    [SerializeField]
    private float physDefenceAmount;

    [SerializeField]
    private float magDefenceAmount;


    private List<Character> characters = new List<Character>();
    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectAllPlayerUnitsBehaviour("Выберите союзного персонажа для баффа", battleSystem));
        SetSelectCharacterBehaviour(new EmptySelectCharacterBehaviour(""));

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_selectCharacterBehaviour.OnSelectCharacter += OnSelectCharacter;
    }
    private void OnSelected()
    {
        if (battleSystem.State is PlayerTurn)
        {
            foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
            {
                playerCharacter.OnClick += SelectCharacter;
            }
        }

    }
    private void OnSelectCharacter()
    {
        Character character;
        if (battleSystem.State is PlayerTurn)
        {
            character = battleSystem.PlayerController.CurrentPlayerCharacter;
            character.PhysDefence += physDefenceAmount;
            character.MagDefence += magDefenceAmount;
        }
        else
        {
            character = battleSystem.EnemyController.CurrentEnemyCharacter;
            character.PhysDefence += physDefenceAmount;
            character.MagDefence += magDefenceAmount;
        }

        characters.Add(GetNextCharacterByDirection(character.PositionOnField, Enums.Directions.top, 1));
        characters.Add(GetNextCharacterByDirection(character.PositionOnField, Enums.Directions.bottom, 1));
        characters.Add(GetNextCharacterByDirection(character.PositionOnField, Enums.Directions.right, 1));
        characters.Add(GetNextCharacterByDirection(character.PositionOnField, Enums.Directions.left, 1));

        characters = characters.Where(x => x != null).ToList();
        if (characters.Count > 0)
        {
            character.PhysDefence ++;
            character.MagDefence ++;
        }

        battleSystem.PlayerController.SetPlayerChosenState(false, x =>
        {
            x.OnClick -= SelectCharacter;
        });

        UseCard(null);
    }

    private void OnCancelSelection()
    {
        foreach (var playerCharacter in battleSystem.PlayerController.PlayerCharactersObjects)
        {
            playerCharacter.OnClick -= SelectCharacter;
        }
    }


    private Character GetNextCharacterByDirection(Vector2 pos, Enums.Directions direction, int localRange)
    {
        int newI = (int)pos.x;
        int newJ = (int)pos.y;

        for (int i = 0; i < localRange; i++)
        {
            switch (direction)
            {
                case Enums.Directions.top:
                    newI--;
                    break;
                case Enums.Directions.bottom:
                    newJ--;
                    break;
                case Enums.Directions.right:
                    newI++;
                    break;
                case Enums.Directions.left:
                    newJ++;
                    break;
            }

            if (newI >= 7 || newI < 0)
            {
                break;
            }
            if (newJ >= 11 || newJ < 0)
            {
                break;
            }

            Cell cell = battleSystem.FieldController.GetCell(newI, newJ);
            Character enemy;
            if (battleSystem.State is PlayerTurn)
            {
                enemy = cell.GetComponentInChildren<Enemy>();
            }
            else
            {
                enemy = cell.GetComponentInChildren<PlayerCharacter>();
            }


            KostilEnemy kostilEnemy = cell.GetComponentInChildren<KostilEnemy>();
            if (cell.transform.childCount > 0)
            {

                if (enemy != null && enemy is not KostilEnemy)
                {
                    return enemy;
                }
                if (kostilEnemy != null)
                {
                    return enemy;
                }
            }

        }
        return null;
    }
}