using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : InteractableObject
{
    [SerializeField] private GameObject hightLight;
    public Material baseColor, offsetColor, swampColor;
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
        OnHoverEnter += EnableHiglight;
        OnHoverExit += DisableHiglight;
        OnClick += OnClickCell;
    }

    private void OnDestroy()
    {
        OnHoverEnter -= EnableHiglight;
        OnHoverExit -= DisableHiglight;
    }

    public void SetCellState(bool isOffset, bool state)
    {
        IsEnabled = state;
        IsSwamp = false;
        meshRenderer.material = isOffset ? offsetColor : baseColor;
    }
    public void SetCellSwamp(bool state)
    {
        IsEnabled = state;
        IsSwamp = true;
        MeshRenderer.material = swampColor;
    }
    public void SetActivatedCell(bool isActivated)
    {
        IsEnabled = isActivated;
        if ((CellIndex.x + CellIndex.y) % 2 == 0)
        {
            meshRenderer.material.color = new Color(0, 1, 0);
        }
        else
        {
            meshRenderer.material.color = new Color(0, 0.5f, 0);
        }
    }

    private void EnableHiglight()
    {
        if (transform.childCount < 2)
        {
            hightLight.SetActive(true);
        }

    }

    private void OnClickCell(GameObject gameObject)
    {
        DisableHiglight();
    }

    private void DisableHiglight()
    {
        if (transform.childCount < 2)
        {
            hightLight.SetActive(false);
        }
    }
}
