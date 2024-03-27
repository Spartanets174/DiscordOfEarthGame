using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PeasantWithPitchforkCharacterDefenceAbility : BaseCharacterAbility
{
    [SerializeField]
    private int range;

    [SerializeField]
    private GameObject prefab;

    private SelectCellsInRangeBehaviour selectCellsBehaviour;
    private SpawnObjectBehaviour spawnObjectBehaviour;

    private WallEnemyCharacter wallEnemyCharacter;

    private List<GameObject> kostilGameObjects = new();
    public override void Init(BattleSystem battleSystem, Character owner)
    {
        this.abilityOwner = owner;
        this.battleSystem = battleSystem;
        SetCardSelectBehaviour(new SelectCellsInRangeBehaviour("Выберите область для размещения", battleSystem, abilityOwner, new Vector2(3, 1), range, "allowed"));
        SetUseCardBehaviour(new SpawnObjectBehaviour(prefab));

        selectCellsBehaviour = (SelectCellsInRangeBehaviour)CardSelectBehaviour;
        spawnObjectBehaviour = (SpawnObjectBehaviour)UseCardBehaviour;

        m_cardSelectBehaviour.OnCancelSelection += OnCancelSelection;
        m_cardSelectBehaviour.OnSelected += OnSelected;
        m_useCardBehaviour.OnCardUse += OnCardUse;
    }

    private void OnSelected()
    {
        foreach (var item in selectCellsBehaviour.highlightedCells)
        {
            item.OnClick -= selectCellsBehaviour.OnSelectedInvoke;
        }
        battleSystem.FieldController.InvokeActionOnField(selectCellsBehaviour.UnSubscribe);

        if (selectCellsBehaviour.highlightedCells.Where(x => x.transform.childCount > 0).ToList().Count == 0 && selectCellsBehaviour.highlightedCells.Count == 3)
        {
            spawnObjectBehaviour.rotation = selectCellsBehaviour.size.x > selectCellsBehaviour.size.y ? Vector3.zero : new Vector3(0, 90, 0);
            UseCard(selectCellsBehaviour.clickedCell.gameObject);
        }
        else
        {
            SelectCard();
        }
    }

    private void OnWallDeath(Character character)
    {
        GameObject.Destroy(wallEnemyCharacter.gameObject);
        foreach (var kostilGameObject in kostilGameObjects)
        {
            GameObject.Destroy(kostilGameObject);
        }
        kostilGameObjects.Clear();
    }

    private void OnCardUse()
    {
        wallEnemyCharacter = spawnObjectBehaviour.spawnedPrefab.GetComponent<WallEnemyCharacter>();
        wallEnemyCharacter.OnDeath += OnWallDeath;
        wallEnemyCharacter.OnClick += battleSystem.SetCurrentChosenCharacter;
        foreach (var cell in selectCellsBehaviour.highlightedCells)
        {
            GameObject gameObject = new GameObject("kostil");
            gameObject.transform.SetParent(cell.transform);
            gameObject.AddComponent<KostilEnemy>();
            gameObject.GetComponent<KostilEnemy>().WallEnemyCharacter = wallEnemyCharacter;
            kostilGameObjects.Add(gameObject);
        }

        battleSystem.FieldController.TurnOnCells();
    }

    private void OnCancelSelection()
    {
        foreach (var item in selectCellsBehaviour.highlightedCells)
        {
            item.OnClick -= selectCellsBehaviour.OnSelectedInvoke;
        }
        battleSystem.FieldController.TurnOnCells();
    }
}
