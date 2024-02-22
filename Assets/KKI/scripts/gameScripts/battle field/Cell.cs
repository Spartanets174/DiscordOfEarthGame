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

    [SerializeField]
    private int transitionCost;

    [SerializeField]
    private bool m_isSwamp;
    public bool IsSwamp => m_isSwamp;

    private int m_transitionCostEnemy;
    public int TransitionCostEnemy
    {
        get => m_transitionCostEnemy;
        set => m_transitionCostEnemy = value;
    }
    private int m_transitionCostPlayer;
    public int TransitionCostPlayer
    {
        get => m_transitionCostPlayer;
        set => m_transitionCostPlayer = value;
    }

    private MeshRenderer meshRenderer;
    public MeshRenderer MeshRenderer => meshRenderer;

    private BoxCollider boxCollider;
    public BoxCollider BoxCollider => boxCollider;

    public Vector2 CellIndex
    {
        get;
        set;
    }

    private void Awake()
    {
        TransitionCostEnemy = transitionCost;
        TransitionCostPlayer = transitionCost;

        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SetCellState(bool state)
    {
        IsEnabled = state;
    }

    public void SetColor(string key, bool isOffset)
    {
        meshRenderer.material = colorDictionary[key].GetColor(isOffset);
    }
    public void SetCellMovable()
    {
        SetCellState(true);
        SetColor("allowed", (CellIndex.x + CellIndex.y) % 2 == 0);
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