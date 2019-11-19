﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {

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
                manager.RecoveringGrids();
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
        this.manager.MouseHoverCard(this.transform, localPos, card);
        manager.tempInfo.text = string.Format("Current Card: " + this.card.GetType().Name);
    }

}
