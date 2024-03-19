using System;
using System.Collections.Generic;
using UniRx;
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

    private ReactiveCollection<PlayerCharacter> m_playerCharactersObjects = new();
    public ReactiveCollection<PlayerCharacter> PlayerCharactersObjects => m_playerCharactersObjects;

    private PlayerCharacter currentPlayerCharacter;
    public PlayerCharacter CurrentPlayerCharacter => currentPlayerCharacter;

    public event Action OnPlayerCharacterSpawned;

    public PlayerTurn PlayerTurn { get; set; }

    private CompositeDisposable disposables = new CompositeDisposable();

    private KeyCode[] keyCodes = new KeyCode[5] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };
    public void Init()
    {
        Observable.EveryUpdate().Subscribe(x =>
        {
            foreach (var item in PlayerCharactersObjects)
            {
                if (Input.GetKey(keyCodes[item.Index]))
                {
                    item.OnClickInvoke();
                }
            }
        }).AddTo(disposables);

    }

    private void OnDestroy()
    {
        foreach (var playerCharacter in m_playerCharactersObjects)
        {
            playerCharacter.OnClick -= SetCurrentPlayerChosenCharacter;
        }
    }

    public PlayerCharacter InstasiatePlayerCharacter(CharacterCard characterCard, Transform parent)
    {
        PlayerCharacter prefab = Instantiate(CharPrefab, Vector3.zero, Quaternion.identity, parent);
        prefab.transform.localPosition = new Vector3(0, 1, 0);
        
        prefab.SetData(characterCard, null, m_playerCharactersObjects.Count);
        prefab.OnClick += SetCurrentPlayerChosenCharacter;
        prefab.OnDeath += OnCharacterDeath;

        m_playerCharactersObjects.Add(prefab);
        OnPlayerCharacterSpawned?.Invoke();
        return prefab;
    }

    private void OnCharacterDeath(Character character)
    {
        PlayerCharactersObjects.Remove((PlayerCharacter)character);
        Destroy(character.gameObject);
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
            Debug.LogError("��� ���������");
        }
    }
    public void ClearDisposables()
    {
        disposables.Dispose();
        disposables.Clear();
        disposables = new();
    }

    public void SetPlayerState(bool state, Action<PlayerCharacter> subAction = null)
    {
        foreach (var playerCharacter in m_playerCharactersObjects)
        {
            playerCharacter.IsEnabled = state;
            subAction?.Invoke(playerCharacter);
        }
    }

    public void SetPlayerChosenState(bool state, Action<PlayerCharacter> subAction = null)
    {
        foreach (var playerCharacter in m_playerCharactersObjects)
        {
            playerCharacter.IsChosen = state;
            subAction?.Invoke(playerCharacter);
        }
    }

    public void ResetAllPlayerCharacters()
    {
        foreach (var playerCharacter in m_playerCharactersObjects)
        {
            playerCharacter.ResetCharacter();
        }
    }

    public void RemoveDebuffsAllPlayerCharacters()
    {
        foreach (var playerCharacter in m_playerCharactersObjects)
        {
            playerCharacter.RemoveDebuffs();
        }
    }
}
