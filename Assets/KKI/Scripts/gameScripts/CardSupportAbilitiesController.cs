using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class CardSupportAbilitiesController : MonoBehaviour, ILoadable
{
    [Header("Model")]
    [SerializeField]
    private PlayerController playerController;

    [Space, Header("Parents")]
    [SerializeField]
    private Transform gameSupportCardsParent;

    [Space, Header("Prefabs")]
    [SerializeField]
    private GameSupportCardDisplay gameSupportCardPrefab;


    private List<GameSupportCardDisplay> m_gameSupportCards = new();
    public List<GameSupportCardDisplay> GameSupportCards => m_gameSupportCards;


    public void Init()
    {
        foreach (var SupportCard in playerController.PlayerDataController.DeckUserSupportCards)
        {
            GameSupportCardDisplay cardDisplay = Instantiate(gameSupportCardPrefab, Vector3.zero, Quaternion.identity, gameSupportCardsParent);
            cardDisplay.transform.localPosition = Vector3.zero;


            cardDisplay.SetData(SupportCard);
            cardDisplay.DragAndDropComponent.StartPos = cardDisplay.transform.localPosition;

            StartCoroutine(SetPosDelayed(cardDisplay.DragAndDropComponent));

            m_gameSupportCards.Add(cardDisplay);
            cardDisplay.IsEnabled = false;
        }
    }

    private IEnumerator SetPosDelayed(DragAndDropComponent dragAndDropComponent)
    {
        yield return new WaitForEndOfFrame();
        dragAndDropComponent.StartPos = dragAndDropComponent.transform.localPosition;
    }

}
