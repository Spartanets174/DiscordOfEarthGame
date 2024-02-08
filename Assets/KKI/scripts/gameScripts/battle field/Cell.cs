using Mono.Cecil;
using Org.BouncyCastle.Bcpg.Sig;
using Renci.SshNet.Security;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;

public class Cell : InteractableObject
{
    [SerializeField]
    private ColorDictioanary colorDictionary;

    private MeshRenderer meshRenderer;
    public MeshRenderer MeshRenderer => meshRenderer;

    private BoxCollider boxCollider;
    public BoxCollider BoxCollider => boxCollider;

    public Vector2 CellIndex
    {
        get;
        set;
    }

    public bool IsSwamp
    {
        get;
        set;
    }
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SetCellState(bool state)
    {
        IsEnabled = state;
        IsSwamp = false;
    }
    public void SetCellSwamp(bool state)
    {
        IsEnabled = state;
        IsSwamp = true;      
    }

    public void SetColor(string key, bool isOffset)
    {
        meshRenderer.material = colorDictionary[key].GetColor(isOffset);
    }
    public void SetCellMovable()
    {
        if (IsSwamp)
        {
            SetCellSwamp(true);
            SetColor("swamp",(CellIndex.x + CellIndex.y) % 2 == 0);
        }
        else
        {
            SetCellState(true);          
            SetColor("allowed", (CellIndex.x + CellIndex.y) % 2 == 0);
        }      
    }
}

[Serializable]
public class ColorDictioanary : SerializableDictionaryBase<string, SwapColor> { }

[Serializable]
public class SwapColor
{
    [SerializeField]
    private Material baseColor ;
    [SerializeField]
    private Material offsetColor;

    public Material GetColor(bool isOffset)
    {
        return isOffset ? offsetColor : baseColor;
    }
}