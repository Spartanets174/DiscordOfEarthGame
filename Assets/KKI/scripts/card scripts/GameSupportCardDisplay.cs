using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSupportCardDisplay : OutlineClicableUI
{
    [SerializeField]
    private Image supportImage;
    [SerializeField]
    private Image supportCardRarity;
    [SerializeField]
    private TextMeshProUGUI supportAbility;
    [SerializeField]
    private TextMeshProUGUI supportName;
    [SerializeField]
    private DragAndDropComponent m_dragAndDropComponent;
    public DragAndDropComponent DragAndDropComponent => m_dragAndDropComponent;

    private CardSupport m_currentCardSupport;
    public CardSupport CurrentCardSupport => m_currentCardSupport;
    private BaseSupport—ardAbility m_gameSupport—ardAbility;
    public BaseSupport—ardAbility GameSupport—ardAbility => m_gameSupport—ardAbility;
    private void Awake()
    {
        m_dragAndDropComponent.OnBeginDragEvent += OnBeginDrag;
        m_dragAndDropComponent.OnEndDragEvent += OnEndDrag;
        m_dragAndDropComponent.OnDropEvent += OnDrop;
    }
    private void OnDrop(GameObject gameObject)
    {
        m_gameSupport—ardAbility.SelectCard();
    }
    private void OnBeginDrag(GameObject gameObject)
    {
        SetBlockerState(true);
    }
    private void OnEndDrag(GameObject gameObject)
    {
        transform.DOLocalMove(DragAndDropComponent.StartPos, 0.5f);
        SetBlockerState(false);
    }

    private void OnDestroy()
    {
        m_dragAndDropComponent.OnBeginDragEvent -= OnBeginDrag;
        m_dragAndDropComponent.OnEndDragEvent -= OnEndDrag;
    }

    public void SetData(CardSupport cardSupport)
    {
        m_currentCardSupport = cardSupport;
        supportImage.sprite = cardSupport.image;

        if (cardSupport.rarity == enums.Rarity.ÃËÙË˜ÂÒÍ‡ˇ)
        {
            supportCardRarity.color = new Color(126, 0, 255);
        }
        else
        {
            supportCardRarity.color = Color.gray;
        }

        supportAbility.text = cardSupport.abilityText;
        supportName.text = cardSupport.cardName;

        if (cardSupport.GameSupport—ardAbility != null)
        {

            if (cardSupport.GameSupport—ardAbility.Type != null)
            {
                gameObject.AddComponent(cardSupport.GameSupport—ardAbility.Type);
                m_gameSupport—ardAbility = (BaseSupport—ardAbility)gameObject.GetComponent(cardSupport.GameSupport—ardAbility.Type);
            }

        }

    }
}
