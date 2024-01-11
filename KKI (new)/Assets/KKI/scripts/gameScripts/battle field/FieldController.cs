using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour, ILoadable
{
    //Скрипт для создания игрового поля
    [SerializeField] private int width, height;
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private GameObject mountainPrefab;
    public Cell[,] CellsOfFieled ;
    public void Init()
    {
        GenerateField();
    }

    private void GenerateField()
    {
        CellsOfFieled = new Cell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cell spawnedTile = Instantiate(cellPrefab, Vector3.zero,Quaternion.identity,transform);
                spawnedTile.name = $"Cell {i} {j}";
                spawnedTile.transform.localPosition = new Vector3(j * -0.27f,0, i * -0.27f);
                if ((j==4 || j==6)&&i%2!=0)
                {
                    GameObject Mountain = Instantiate(mountainPrefab, Vector3.zero, Quaternion.identity, spawnedTile.transform);
                    Mountain.transform.localPosition = new Vector3(0, 0.5f, 0);
                }

                if ((i + j) % 2 == 1)
                {
                    spawnedTile.SetCellState(true, true);
                }
                else
                {
                    spawnedTile.SetCellState(false, true);
                }
                if ((i == 2 && j == 2)|| (i == 4 && j == 3)|| (i == 2 && j == 7)|| (i == 4 && j == 8))
                {
                    spawnedTile.SetCellSwamp(true);                   
                }
                spawnedTile.Enabled = true;
 
                spawnedTile.CellIndex = new Vector2(i, j);
                CellsOfFieled[i,j] = spawnedTile;
            }
        }
    }

    public void InvokeActionOnField(Action<Cell> action)
    {
        for (int i = 0; i < CellsOfFieled.GetLength(0); i++)
        {
            for (int j = 0; j < CellsOfFieled.GetLength(1); j++)
            {
                Cell cell = CellsOfFieled[i, j];
                float x = cell.CellIndex.x;
                float y = cell.CellIndex.y;
                if ((x + y) % 2 == 1)
                {
                    cell.SetCellState(true, true);
                }
                else
                {
                    cell.SetCellState(false, false);
                }
                if ((x == 2 && y == 2) || (x == 4 && y == 3) || (x == 2 && y == 7) || (x == 4 && y == 8))
                {
                    cell.SetCellSwamp(true);
                }
                action?.Invoke(CellsOfFieled[i,j]);
            }
        }
    }

    public void TurnOnCells(GameObject gameObject)
    {
        TurnOnCells();
    }

    public void TurnOffCells(GameObject gameObject)
    {
        TurnOffCells();
    }

    public void TurnOffCells()
    {
        SetCellsState(false);
    }

    public void TurnOnCells()
    {
        SetCellsState(true);
    }

    public void SetCellsState(bool state)
    {
        foreach (var cell in CellsOfFieled)
        {
            float x = cell.CellIndex.x;
            float y = cell.CellIndex.y;
            if ((x + y) % 2 == 1)
            {
                cell.SetCellState(true, state);
            }
            else
            {
                cell.SetCellState(false, state);
            }
            if ((x == 2 && y == 2) || (x == 4 && y == 3) || (x == 2 && y == 7) || (x == 4 && y == 8))
            {
                cell.SetCellSwamp(state);
            }
        }
    }

    public Cell GetCell(int i, int j)
    {
        return CellsOfFieled[i, j];
    }
}
