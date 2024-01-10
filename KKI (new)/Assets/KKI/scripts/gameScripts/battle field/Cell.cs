using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : InteractableObject
{
    [SerializeField] private GameObject hightLight;
    public Material baseColor, offsetColor, swampColor;
    private MeshRenderer renderer;
    private BoxCollider boxCollider;
    public BoxCollider BoxCollider => boxCollider;
    public Vector2 CellIndex
    {
        get;
        set;
    }
    public bool Enabled
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
        renderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        OnHoverEnter += EnableHiglight;
        OnHoverExit += DisableHiglight;
        OnClick += OnCellClick;
    }

    private void OnDestroy()
    {
        OnHoverEnter -= EnableHiglight;
        OnHoverExit -= DisableHiglight;
        OnClick -= OnCellClick;
    }
    public void Init(bool isOffset)
    {
        renderer.material = isOffset?offsetColor:baseColor;
    }
    private void OnCellClick()
    {
        /*//Проверка на то, включена ли клетка
        if (Enabled)
        {
            //Передача данных о текущей клетки в battleSystem            
            if (this.transform.childCount!=1)
            {
                if (this.transform.GetChild(1).GetComponent<character>().isEnemy|| this.transform.GetChild(1).GetComponent<character>().isStaticEnemy)
                {
                    battleSystem.cahngeCardWindow(this.transform.GetChild(1).gameObject, true);                  
                }
                else
                {
                    battleSystem.cahngeCardWindow(this.transform.GetChild(1).gameObject, false);                   
                }
                
            }
            if (!battleSystem.isEnemyTurn)
            {
                battleSystem.OnMoveButton(this.gameObject);
            }        
        }   */     
    }

    private void EnableHiglight()
    {
        if (Enabled)
        {
            hightLight.SetActive(true);
        }

    }
    private void DisableHiglight()
    {
        if (Enabled)
        {
            hightLight.SetActive(false);
        }
    }
}
