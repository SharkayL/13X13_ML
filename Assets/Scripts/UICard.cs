using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler {

    public Card card;
    Vector3 localPos;
    Transform parent;
    public MatureManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager.board.over) {
            return;
        }
        if (manager.board.currentPlayer.ghost) {
            return;
        }
        var player = manager.TargetPlayer();
        if (player.actionCount > 0)
        {
            if (player.playingCard != this.card)
            {
                player.playingCard = this.card;
                manager.RecoveringGrids(false);
                manager.HighlightingPossibilities(card);
                manager.instruction.text = string.Format("You selected the" + card.getName() + "!");
            }
            else
            {
                //info panel shows: you already chose the current card.
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager.board.currentPlayer.ghost)
        {
            return;
        }
        localPos = this.transform.localPosition;
        localPos.y += 10;
        manager.popLayer.gameObject.SetActive(true);
        manager.popLayer.transform.localPosition = localPos;
        var popC = manager.popLayer.transform.GetChild(0);        
        Image img = popC.GetComponent<Image>();
        img.sprite = manager.GetCardSprite(card);
        popC.transform.GetChild(0).GetComponent<Text>().text = card.getName();
        popC.transform.GetChild(1).GetComponent<Text>().text = card.getDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        manager.popLayer.gameObject.SetActive(false);
    }
}
