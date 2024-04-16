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

    [Header("Setup data")]
    [SerializeField]
    private bool fullWindow;

    private DbManager dbManager;

    private Dictionary<int, Vector2> resolutions = new();
    public Dictionary<int, Vector2> Resolutions => resolutions;

    private List<OutlineInteractableObject> outlineInteractableObjects;
    public List<OutlineInteractableObject> OutlineInteractableObjects => outlineInteractableObjects;
    public void Init()
    {
        dbManager = FindObjectOfType<DbManager>();
        outlineInteractableObjects = FindObjectsOfType<OutlineInteractableObject>().ToList();

        resolutions.Clear();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width <= 1920 && Screen.resolutions[i].height <= 1080)
            {
                if (resolutions.Where(x=>x.Value.x == Screen.resolutions[i].width && x.Value.y == Screen.resolutions[i].height).ToList().Count==0)
                {
                    resolutions.Add(i, new Vector2(Screen.resolutions[i].width, Screen.resolutions[i].height));
                }
                
            }           
        }
        if (PlayerPrefs.GetString("isFirst") != "1")
        {
            ChangeSoundLevel(1);
            ChangeMusicLevel(1);
            ChangeScreenSize(resolutions.Last().Key);
            ChangeFullScreenState(fullWindow);
            PlayerPrefs.SetString("isFirst", "1");
        }

        foreach (var interactableObject in outlineInteractableObjects)
        {
            interactableObject.OnClick += x => PlayClickSound();
        }

        MusicAudioSource.loop = true;
        MusicAudioSource.clip = menuMusic;
        MusicAudioSource.Play();
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
        FullScreenMode fullScreenMode = GetFullScreenState() ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution((int)vector2.x, (int)vector2.y, fullScreenMode);
    }
}
