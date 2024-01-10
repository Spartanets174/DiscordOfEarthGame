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

    void GenerateField()
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
                //Ессли чётная, то один цвет, нечетная - другой
                if ((i + j) % 2 == 1)
                {
                    spawnedTile.Init(true);
                }
                else
                {
                    spawnedTile.Init(false);
                }
                if ((i == 2 && j == 2)|| (i == 4 && j == 3)|| (i == 2 && j == 7)|| (i == 4 && j == 8))
                {
                    spawnedTile.IsSwamp = true;
                    spawnedTile.gameObject.GetComponent<MeshRenderer>().material = spawnedTile.swampColor;
                }
                spawnedTile.Enabled = true;
                //Запись клеток в списки
                spawnedTile.CellIndex = new Vector2(i, j);
                CellsOfFieled[i,j] = spawnedTile;
            }
        }
    }

    public Cell GetCell(int i, int j)
    {
        return CellsOfFieled[i, j];
    }
}
