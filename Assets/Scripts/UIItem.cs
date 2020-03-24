using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;
    Vector3 pos;
    Transform parent;
    public MatureManager manager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!ProcessBeingDrag())
        {
            eventData.pointerDrag = null;
        }
    }

    public bool ProcessBeingDrag()
    {
        if (manager.displayPlayerId != -1 && manager.board.currentPlayer.id != manager.displayPlayerId)
        {
            return false;
        }
        if (manager.board.over)
        {
            return false;
        }
        if (manager.board.currentPlayer.ghost)
        {
            return false;
        }
        if (manager.board.currentPlayer.noItem)
        {
            return false;
        }
        parent = this.transform.parent;
        this.transform.SetParent(manager.ratioPanel.transform);
        this.transform.position = Input.mousePosition;
        return true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(manager.ratioPanel.transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            this.transform.position = globalMousePos;
            this.transform.rotation = manager.ratioPanel.transform.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this) {
            this.transform.SetParent(parent);
            PlayerState targetPlayer = manager.DroppedOnPlayer();
            GridInfo grid = manager.DroppedOnGrid();
            if (targetPlayer != null && item.canPlay(manager.board.currentPlayer, targetPlayer))
            {
                if (item.play(manager.board.currentPlayer, targetPlayer, true))
                {
                }
            }
            else if (grid != null) {
                if (item.play(manager.board.currentPlayer, grid, true)) {
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.itemName.transform.parent.gameObject.SetActive(true);
        manager.itemName.text = string.Format("<b>{0}</b>", this.item.getName());
        manager.itemDescrib.text = string.Format(this.item.getDescription());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        manager.itemName.transform.parent.gameObject.SetActive(false);
        manager.itemName.text = null;
        manager.itemDescrib.text = null;
    }
}
