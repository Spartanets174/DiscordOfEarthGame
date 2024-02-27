using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectCellsBehaviour : ICardSelectable
{
    private BattleSystem battleSystem;
    public event Action OnSelected;
    public event Action OnCancelSelection;

    private Vector2 range;
    private string m_selectCardTipText;

    public List<Cell> highlightedCells = new();
    public string SelectCardTipText
    {
        get
        {
            return m_selectCardTipText;
        }
    }

    public SelectCellsBehaviour(string text, BattleSystem battleSystem, Vector2 range)
    {
        m_selectCardTipText = text;
        this.battleSystem = battleSystem;
        this.range = range;
    }

    public void SelectCard()
    {
        battleSystem.FieldController.InvokeActionOnField(Subscribe);
    }

    public void OnSelectedInvoke(GameObject gameObject)
    {
        OnSelected?.Invoke();
    }

    public void Subscribe(Cell cell)
    {
        cell.SetCellState(true);
        cell.OnHoverEnter += OnHoverEnter;
        cell.OnHoverExit += OnHoverExit;
    }

    private void OnHoverExit(GameObject gameObject)
    {
        Cell currentCell = gameObject.GetComponent<Cell>();
        highlightedCells.Clear();
        currentCell.OnClick -= OnSelectedInvoke;
    }

    private void OnHoverEnter(GameObject gameObject)
    {
        Cell currentCell = gameObject.GetComponent<Cell>();

        Vector2 currentCellIndex = currentCell.CellIndex;
        float borderX = Mathf.Floor(range.x / 2);
        float borderY = Mathf.Floor(range.y / 2);

        float xStart = currentCellIndex.x - borderX;
        float xEnd = currentCellIndex.x + borderX;
        float yStart = currentCellIndex.y - borderY;
        float yEnd = currentCellIndex.y + borderY;

        currentCell.OnClick += OnSelectedInvoke;
        battleSystem.FieldController.InvokeActionOnField(x =>
        {          
            Vector2 cellIndex = x.CellIndex;
            if (cellIndex.x >= xStart && cellIndex.x<= xEnd && cellIndex.y >= yStart && cellIndex.y <= yEnd)
            {               
                x.SetColor("attack");               
                highlightedCells.Add(x);
            }
            else
            {
                x.SetColor("normal");
            }
        });
    }

    public void UnSubscribe(Cell cell)
    {
        cell.OnHoverEnter -= OnHoverEnter;
        cell.OnHoverExit -= OnHoverExit;
    }

    public void CancelSelection()
    {
        battleSystem.FieldController.InvokeActionOnField(x=> {
            x.SetCellState(true);
            x.SetColor("normal");
            UnSubscribe(x);
        });
        OnCancelSelection?.Invoke();
    }
}
