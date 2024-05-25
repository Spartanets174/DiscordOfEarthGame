using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsController : MonoBehaviour, ILoadable
{
    [Header("Audio sources")]
    [SerializeField]
    private AudioSource SoundAudioSource;
    [SerializeField]
    private AudioSource MusicAudioSource;

    [Header("Sounds and music")]
    [SerializeField]
    private AudioClip clickSound;
    [SerializeField]
    private AudioClip menuMusic;

    [Header("Settings")]
    [SerializeField]
    private KeyCode settingsButton;
    [SerializeField]
    private int maxFPS=60;
    
    private DbManager dbManager;

    private Dictionary<int, Vector2> resolutions = new();
    public Dictionary<int, Vector2> Resolutions => resolutions;

    private List<OutlineInteractableObject> outlineInteractableObjects;
    public List<OutlineInteractableObject> OutlineInteractableObjects => outlineInteractableObjects;

    private bool isPaused;
    public bool CanPause { get; set; }

    public event Action<bool> OnPauseStateChanged;
    public virtual void Init()
    {
        Application.targetFrameRate = maxFPS;
        CanPause = true;
        dbManager = FindObjectOfType<DbManager>();
        outlineInteractableObjects = FindObjectsOfType<OutlineInteractableObject>().ToList();
        resolutions.Clear();
        int count = 0;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width <= 1920 && Screen.resolutions[i].height <= 1080)
            {
                if (resolutions.Where(x => x.Value.x == Screen.resolutions[i].width && x.Value.y == Screen.resolutions[i].height).ToList().Count == 0)
                {
                    resolutions.Add(count, new Vector2(Screen.resolutions[i].width, Screen.resolutions[i].height));
                    count++;
                }
            }
        }
        if (PlayerPrefs.GetString("isFirst") != "1")
        {
            ChangeSoundLevel(1);
            ChangeMusicLevel(1);
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

            PlayerPrefs.SetFloat("screenSizeX", 1920);
            PlayerPrefs.SetFloat("screenSizeY", 1080);
            PlayerPrefs.SetInt("fullScreen", 1);
            PlayerPrefs.SetString("isFirst", "1");
        }

        MusicAudioSource.loop = true;
        MusicAudioSource.clip = menuMusic;
        MusicAudioSource.Play();
    }
    private void Update()
    {
        if (Input.GetKeyDown(settingsButton)&&CanPause)
        {
            TogglePausedState();
        }
    }

    public void TogglePausedState()
    {

        isPaused = !isPaused;
        foreach (var item in outlineInteractableObjects)
        {
            item.IsEnabled = !isPaused;
        }
        Time.timeScale = isPaused ? 0 : 1;
        OnPauseStateChanged?.Invoke(isPaused);


    }
    public void ExitAccount()
    {
        dbManager.SavePlayer();
        SaveSystem.DeletePlayer();
        SceneController.ToCreatePlayer();
    }

    public void PlayClickSound()
    {
        SoundAudioSource.PlayOneShot(clickSound);
    }

    public void ChangeScreenSize(int id)
    {
        PlayerPrefs.SetFloat("screenSizeX", resolutions[id].x);
        PlayerPrefs.SetFloat("screenSizeY", resolutions[id].y);
        SetScreenSettings();
    }

    public void ChangeFullScreenState(bool state)
    {
        PlayerPrefs.SetInt("fullScreen", state ? 1 : 0);
        SetScreenSettings();
    }

    public void ChangeSoundLevel(float value)
    {
        PlayerPrefs.SetFloat("sound", value);
        SoundAudioSource.volume = PlayerPrefs.GetFloat("sound");
    }
    public void ChangeMusicLevel(float value)
    {
        PlayerPrefs.SetFloat("music", value);
        MusicAudioSource.volume = PlayerPrefs.GetFloat("music");
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("music");
    }

    public float GetSoundVolume()
    {
        return PlayerPrefs.GetFloat("sound");
    }

    public bool GetFullScreenState()
    {
        bool state = PlayerPrefs.GetInt("fullScreen") == 1 ? true : false;
        return state;
    }

    public int GetScreenSize()
    {
        float x = PlayerPrefs.GetFloat("screenSizeX");
        float y = PlayerPrefs.GetFloat("screenSizeY");
        Vector2 res = new Vector2(x, y);
        return resolutions.Where(x => x.Value == res).FirstOrDefault().Key;
    }

    private void SetScreenSettings()
    {
        Vector2 vector2 = resolutions[GetScreenSize()];
        if (vector2.x>1920||vector2.y>1080)
        {
            vector2 = new Vector2(1920,1080);
        }
        FullScreenMode fullScreenMode = GetFullScreenState() ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution((int)vector2.x, (int)vector2.y, fullScreenMode);
    }
}
