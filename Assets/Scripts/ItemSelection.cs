using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Item item = null;
    public BoardState board;
    public Image tooltip;
    public void OnPointerClick(PointerEventData eventData)
    {
        Image mark = board.manager.markedDiscard;
        mark.gameObject.SetActive(false);

        board.manager.toBeRemovedItem = item;
        mark.gameObject.SetActive(true);
        mark.transform.position = this.transform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
        Vector3 pos = tooltip.transform.position;
        tooltip.transform.position = new Vector3(this.transform.position.x, pos.y, pos.z);
        tooltip.transform.GetChild(0).GetComponent<Text>().text = item.getName();
        tooltip.transform.GetChild(1).GetComponent<Text>().text = item.getDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }

}
