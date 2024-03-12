using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsController : MonoBehaviour, ILoadable
{
    private DbManager dbManager;

    private List<OutlineInteractableObject> outlineInteractableObjects;
    public List<OutlineInteractableObject> OutlineInteractableObjects => outlineInteractableObjects;
    public void Init()
    {
        dbManager = FindObjectOfType<DbManager>();
        outlineInteractableObjects = FindObjectsOfType<OutlineInteractableObject>().ToList();
    }

    public void ExitAccount()
    {
        dbManager.SavePlayer();
        SaveSystem.DeletePlayer();
        SceneController.ToCreatePlayer();
    }
}
