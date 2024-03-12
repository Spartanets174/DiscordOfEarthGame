using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICoordinator : MonoBehaviour, ILoadable
{
    [Space, Header("Buttons")]
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button bookOfCardsButton;
    [SerializeField]
    private Button exitbutton;

    [Space, Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI playerNick;
    [SerializeField]
    private TextMeshProUGUI warningText;

    [Space, Header("Game objects")]   
    [SerializeField]
    private GameObject bookOfCards;
    [SerializeField]
    private GameObject shop;
    [SerializeField]
    private GameObject settings;
    [SerializeField]
    private GameObject shopText;
    [SerializeField]
    private GameObject settingsText;

    [Space, Header("Interactable objects")]
    [SerializeField]
    private OutlineInteractableObject shopObject;
    [SerializeField]
    private OutlineInteractableObject settingsObject;

    [Space, Header("Controllers")]
    [SerializeField]
    private PlayerDataController PlayerManager;


    private Camera cam;
    public void Init()
    {
        cam = FindObjectOfType<Camera>();

        playButton.onClick.AddListener(ToGame);
        exitbutton.onClick.AddListener(SceneController.Exit);
        bookOfCardsButton.onClick.AddListener(TurnOnBookOfCards);

        shopObject.OnClick += TurnOnShop;
        shopObject.OnHover += MoveShopCaption;
        shopObject.OnHoverEnter += TurnOnShopText;
        shopObject.OnHoverExit += TurnOffShopText;
        shopObject.OnEnableChanged += x => { if (!x) TurnOffShopText(null); };


        settingsObject.OnClick += TurnOnSettings;
        settingsObject.OnHover += MoveSettingsCaption;
        settingsObject.OnHoverEnter += TurnOnSettingsText;
        settingsObject.OnHoverExit += TurnOffSettingsText;
        settingsObject.OnEnableChanged += x => { if (!x) TurnOffSettingsText(null); };

        shopObject.IsEnabled = true;
        settingsObject.IsEnabled = true;

        playerNick.text = $"Приветсвуем тебя, {PlayerManager.CharacterName}!";
        SetState(shopText, false);
        SetState(settingsText,false);
    }
    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(ToGame);
        exitbutton.onClick.RemoveListener(SceneController.Exit);
        bookOfCardsButton.onClick.RemoveListener(TurnOnBookOfCards);

        shopObject.OnClick -= TurnOnShop;
        shopObject.OnHover -= MoveShopCaption;
        shopObject.OnHoverEnter -= TurnOnShopText;
        shopObject.OnHoverExit -= TurnOffShopText;

        settingsObject.OnClick -= TurnOnSettings;
        settingsObject.OnHover -= MoveSettingsCaption;
        settingsObject.OnHoverEnter -= TurnOnSettingsText;
        settingsObject.OnHoverExit -= TurnOffSettingsText;

        StopAllCoroutines();
    }

    public void ToGame()
    {
        if (PlayerManager.DeckUserCharCards.Count < 5 || PlayerManager.DeckUserSupportCards.Count < 7)
        {
            SetState(warningText.gameObject,true);
            StartCoroutine(TurnOffWarnningText());
        }
        else
        {
            SceneController.ToGame();
        }
    }

    private void TurnOnBookOfCards()
    {
        shopObject.IsEnabled = false;
        settingsObject.IsEnabled = false;
        SetState(bookOfCards, true); 
    }
    private void MoveSettingsCaption(GameObject gameObject)
    {
        Rect settingsRect = settingsText.GetComponent<RectTransform>().rect;
        settingsText.transform.localPosition = new Vector3(Input.mousePosition.x - cam.scaledPixelWidth / 2 + settingsRect.width/2, Input.mousePosition.y - cam.scaledPixelHeight / 2 + settingsRect.height / 2, 0);
    }

    private void MoveShopCaption(GameObject gameObject)
    {
        Rect shopRect = shopText.GetComponent<RectTransform>().rect;
        shopText.transform.localPosition = new Vector3(Input.mousePosition.x - cam.scaledPixelWidth / 2 - shopRect.width/2, Input.mousePosition.y - cam.scaledPixelHeight / 2 + shopRect.height / 2, 0);
    }
    private void TurnOnShop(GameObject gameObject)
    {
        shopObject.IsEnabled = false;
        settingsObject.IsEnabled = false;
        SetState(shop,true);
    }
    private void TurnOnSettings(GameObject gameObject)
    {
        shopObject.IsEnabled = false;
        settingsObject.IsEnabled = false;
        SetState(settings, true);
    }
    private void TurnOnShopText(GameObject gameObject)
    {
        SetState(shopText, true);
    }
    private void TurnOffShopText(GameObject gameObject)
    {
        SetState(shopText, false);
    }
    private void TurnOnSettingsText(GameObject gameObject)
    {
        SetState(settingsText, true);
    }
    private void TurnOffSettingsText(GameObject gameObject)
    {
        SetState(settingsText, false);
    }
    private IEnumerator TurnOffWarnningText()
    {
        yield return new WaitForSecondsRealtime(2);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(warningText.DOFade(0, 2f))
        .OnComplete(() => {
            warningText.color = Color.red;
            warningText.gameObject.SetActive(false);
            mySequence.Kill();
        });
        mySequence.Play();
    }
    private void SetState(GameObject obj, bool state)
    {
        obj.SetActive(state);
    }
}
