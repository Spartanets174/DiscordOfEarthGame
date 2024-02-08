using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour, ILoadable
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
    private GameObject blur;
    
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
    private SceneController sceneController;
    [SerializeField]
    private PlayerManager PlayerManager;


    private Camera cam;
    public void Init()
    {
        cam = FindObjectOfType<Camera>();

        playButton.onClick.AddListener(ToGame);
        exitbutton.onClick.AddListener(sceneController.Exit);
        bookOfCardsButton.onClick.AddListener(TurnOnBookOfCards);

        shopObject.OnClick += TurnOnShop;
        shopObject.OnHover += MoveShopCaption;
        shopObject.OnHoverEnter += TurnOnShopText;
        shopObject.OnHoverExit += TurnOffShopText;

        settingsObject.OnClick += TurnOnSettings;
        settingsObject.OnHover += MoveSettingsCaption ;
        settingsObject.OnHoverEnter += TurnOnSettingsText;
        settingsObject.OnHoverExit += TurnOffSettingsText;

        shopObject.IsEnabled = true;
        settingsObject.IsEnabled = true;

        playerNick.text = $"Приветсвуем тебя, {PlayerManager.CharacterName}!";
        SetState(shopText, false);
        SetState(settingsText,false);
    }
    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(ToGame);
        exitbutton.onClick.RemoveListener(sceneController.Exit);
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
            sceneController.ToGame();
        }
    }

    private void TurnOnBookOfCards()
    {
        shopObject.Collider.enabled = false;
        settingsObject.Collider.enabled = false;
        SetState(bookOfCards, true); 
    }
    private void MoveSettingsCaption()
    {
        Rect settingsRect = settingsText.GetComponent<RectTransform>().rect;
        settingsText.transform.localPosition = new Vector3(Input.mousePosition.x - cam.scaledPixelWidth / 2 + settingsRect.width/2, Input.mousePosition.y - cam.scaledPixelHeight / 2 + settingsRect.height / 2, 0);
    }

    private void MoveShopCaption()
    {
        Rect shopRect = shopText.GetComponent<RectTransform>().rect;
        shopText.transform.localPosition = new Vector3(Input.mousePosition.x - cam.scaledPixelWidth / 2 - shopRect.width/2, Input.mousePosition.y - cam.scaledPixelHeight / 2 + shopRect.height / 2, 0);
    }
    private void TurnOnShop(GameObject gameObject)
    {
        shopObject.Collider.enabled = false;
        settingsObject.Collider.enabled = false;
        SetState(shop,true);
    }
    private void TurnOnSettings(GameObject gameObject)
    {
        shopObject.Collider.enabled = false;
        settingsObject.Collider.enabled = false;
        SetState(settings, true);
    }
    private void TurnOnShopText()
    {
        SetState(shopText, true);
    }
    private void TurnOffShopText()
    {
        SetState(shopText, false);
    }
    private void TurnOnSettingsText()
    {
        SetState(settingsText, true);
    }
    private void TurnOffSettingsText()
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
