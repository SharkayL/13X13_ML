using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

    public Item item;
    Vector3 pos;
    Transform parent;
    public MatureManager manager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (manager.displayPlayerId != -1 && manager.board.currentPlayer.id != manager.displayPlayerId) {
            return;
        }
        if (manager.board.over) {
            return;
        }
        if (manager.board.currentPlayer.ghost)
        {
            return;
        }
        if (manager.board.currentPlayer.noItem) {
            return;
        }
        parent = this.transform.parent;
        this.transform.SetParent(manager.ratioPanel.transform);
        this.transform.position = Input.mousePosition;
        manager.instruction.text = string.Format("You selected the" + item.GetType().Name + "!");
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
            if (targetPlayer != null) {
                if (item.play(manager.board.currentPlayer, targetPlayer, true))
                {
                    manager.instruction.text = "current player used " + item.GetType().Name + " on target player.";
                    //Debug.Log("current player used " + item.GetType().Name + " on target player." );
                }
            }
            
            //Destroy(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.itemName.text = string.Format("<b>{0}</b>", this.item.getName());
        manager.itemDescrib.text = string.Format(this.item.getDescription());
    }
}
