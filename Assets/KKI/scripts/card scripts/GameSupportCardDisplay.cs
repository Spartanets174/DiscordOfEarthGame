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
    private BaseSupport�ardAbility m_gameSupport�ardAbility;
    public BaseSupport�ardAbility GameSupport�ardAbility => m_gameSupport�ardAbility;
    private void Awake()
    {
        m_dragAndDropComponent.OnBeginDragEvent += OnBeginDrag;
        m_dragAndDropComponent.OnEndDragEvent += OnEndDrag;
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

    public void SetData(CardSupport cardSupport, BattleSystem battleSystem)
    {
        m_currentCardSupport = cardSupport;
        supportImage.sprite = cardSupport.image;

        if (cardSupport.rarity == Enums.Rarity.����������)
        {
            supportCardRarity.color = new Color(126, 0, 255);
        }
        else
        {
            supportCardRarity.color = Color.gray;
        }

        supportAbility.text = cardSupport.abilityText;
        supportName.text = cardSupport.cardName;

        if (cardSupport.GameSupport�ardAbility != null)
        {
            m_gameSupport�ardAbility = cardSupport.GameSupport�ardAbility;
            m_gameSupport�ardAbility.Init(battleSystem);
        }

    }
}
