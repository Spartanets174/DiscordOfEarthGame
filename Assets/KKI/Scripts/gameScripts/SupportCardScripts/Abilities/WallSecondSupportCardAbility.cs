using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallSecondSupportCardAbility : BaseSupport—ardAbility
{
    private SelectCellsBehaviour selectCellsBehaviour;
    private SpawnObjectBehaviour spawnObjectBehaviour;

    private WallEnemyCharacter wallEnemyCharacter;

    private List<GameObject> kostilGameObjects = new();
    protected override void Start()
    {
        base.Start();

        SetCardSelectBehaviour(new SelectCellsBehaviour("¬˚·ÂËÚÂ Ó·Î‡ÒÚ¸ ‰Îˇ ‡ÁÏÂ˘ÂÌËˇ", battleSystem, new Vector2(3, 1), "allowed"));
        SetUseCardBehaviour(new SpawnObjectBehaviour(battleSystem));

        selectCellsBehaviour = (SelectCellsBehaviour)CardSelectBehaviour;
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
        
        spawnObjectBehaviour.prefab = battleSystem.GameUIPresenter.CurrentGameSupportCardDisplay.Value.CurrentCardSupport.prefab;



        if (selectCellsBehaviour.highlightedCells.Where(x=>x.transform.childCount>0).ToList().Count==0&& selectCellsBehaviour.highlightedCells.Count==3)
        {
            spawnObjectBehaviour.rotation = selectCellsBehaviour.range.x > selectCellsBehaviour.range.y ? Vector3.zero : new Vector3(0, 90, 0);
            UseCard(selectCellsBehaviour.clickedCell.gameObject);
        }
        else
        {
            SelectCard();
        }
    }

    private void OnWallDeath(Character character)
    {
        foreach (var kostilGameObject in kostilGameObjects)
        {
            Destroy(kostilGameObject);
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
