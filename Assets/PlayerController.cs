using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ILoadable
{
    [Header("Controllers")]
    [SerializeField]
    private PlayerDataController m_playerDataController;
    public PlayerDataController PlayerDataController => m_playerDataController;

    [Header("Prefabs")]
    [SerializeField]
    private PlayerCharacter charPrefab;
    public PlayerCharacter CharPrefab => charPrefab;

    private List<PlayerCharacter> m_playerCharactersObjects = new();
    public List<PlayerCharacter> PlayerCharactersObjects => m_playerCharactersObjects;

    private PlayerCharacter currentPlayerCharacter;
    public PlayerCharacter CurrentPlayerCharacter => currentPlayerCharacter;

    public event Action OnPlayerCharacterSpawned;

    public void Init()
    {
        
    }

    public PlayerCharacter InstasiatePlayerCharacter(CharacterCard characterCard, Transform parent)
    {
        PlayerCharacter prefab = Instantiate(CharPrefab, Vector3.zero, Quaternion.identity, parent);
        prefab.transform.localPosition = new Vector3(0, 1, 0);
        m_playerCharactersObjects.Add(prefab);

        prefab.SetData(characterCard, null, m_playerCharactersObjects.Count - 1);
       
        OnPlayerCharacterSpawned?.Invoke();
        return prefab;
    }


    public void SetCurrentPlayerChosenCharacter(GameObject character)
    {
        if (character != null)
        {
            if (currentPlayerCharacter != null)
            {
                currentPlayerCharacter.IsCurrentPlayerCharacter = false;
            }
            currentPlayerCharacter = character.GetComponent<PlayerCharacter>();
            currentPlayerCharacter.IsCurrentPlayerCharacter = true;
        }
        else
        {
            Debug.LogError("Нет персонажа");
        }
    }

}
