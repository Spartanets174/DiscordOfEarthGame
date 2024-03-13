using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectBehaviour : ICardUsable
{
    public GameObject prefab;
    public GameObject spawnedPrefab;
    private BattleSystem battleSystem;
    public Vector3 rotation;

    public SpawnObjectBehaviour(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
    }

    public event Action OnCardUse;
    public void UseAbility(GameObject gameObject)
    {
        spawnedPrefab = GameObject.Instantiate(prefab,Vector3.zero,Quaternion.identity, gameObject.transform);
        spawnedPrefab.transform.localPosition =new Vector3(0, 1, 0);
        spawnedPrefab.transform.DOLocalRotate(rotation, 0);
        OnCardUse?.Invoke();
    }

}
