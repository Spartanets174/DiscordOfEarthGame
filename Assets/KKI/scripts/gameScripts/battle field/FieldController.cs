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
                spawnedTile.SetCellState(true);
                spawnedTile.SetColor("normal",(i + j) % 2 == 0);

                if ((i == 2 && j == 2)|| (i == 4 && j == 3)|| (i == 2 && j == 7)|| (i == 4 && j == 8))
                {
                    spawnedTile.SetColor("swamp", (i + j) % 2 == 0);
                    spawnedTile.SetCellSwamp(true);                   
                }
                spawnedTile.IsEnabled = true;
 
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

    private void SetCellsState(bool state)
    {
        foreach (var cell in CellsOfFieled)
        {
            cell.SetCellState(state);
            cell.SetColor("normal", (cell.CellIndex.y + cell.CellIndex.x) % 2 == 0);

            if (cell.IsSwamp)
            {
                cell.SetColor("swamp", (cell.CellIndex.y + cell.CellIndex.x) % 2 == 0);
                cell.SetCellSwamp(state);
            }
        }
    }

    public Cell GetCell(int i, int j)
    {
        return CellsOfFieled[i, j];
    }
}
