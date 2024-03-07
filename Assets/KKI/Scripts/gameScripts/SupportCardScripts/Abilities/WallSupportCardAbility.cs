using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallSupportCardAbility : BaseSupport—ardAbility, ITurnCountable
{
    public event Action<ITurnCountable> OnReturnToNormal;

    private int m_turnCount;
    public int TurnCount { get => m_turnCount; set => m_turnCount = value; }

    private bool m_isBuff;
    public bool IsBuff { get => m_isBuff; }

    private SelectCellsBehaviour selectCellsBehaviour;
    private SpawnObjectBehaviour spawnObjectBehaviour;

    private GameObject wall;

    private List<GameObject> kostilGameObjects = new();
    protected override void Start()
    {
        base.Start();

        TurnCount = 2;
        m_isBuff = true;

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

        if (selectCellsBehaviour.highlightedCells.Where(x => x.transform.childCount > 0).ToList().Count == 0 && selectCellsBehaviour.highlightedCells.Count == 3)
        {
            UseCard(selectCellsBehaviour.clickedCell.gameObject);
        }
        else
        {
            SelectCard();
        }
    }

    private void OnCardUse()
    {
        wall = spawnObjectBehaviour.spawnedPrefab;
        foreach (var cell in selectCellsBehaviour.highlightedCells)
        {
            GameObject gameObject = new GameObject("kostil");
            gameObject.transform.SetParent(cell.transform);
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
    public void ReturnToNormal()
    {
        foreach (var kostilGameObject in kostilGameObjects)
        {
            Destroy(kostilGameObject);
        }
        kostilGameObjects.Clear();

        Destroy(wall);
    }
}
