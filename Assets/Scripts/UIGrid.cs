using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGrid : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GridInfo info;
    Collider2D collider;

    void Start()
    {
        collider = this.GetComponent<Collider2D>();
    }
public void OnPointerClick(PointerEventData eventData)
    {
        var board = info.board;
        var player = board.currentPlayer;
        if (board.manager.displayPlayerId != -1 && board.currentPlayer.id != board.manager.displayPlayerId)
        {
            return;
        }
        if (board.over) {
            return;
        }
        if (player.actionCount > 0)
        {
            //if (player.ghost) {
            //    player.playingCard = Card.Convert(possibleCards.adjacentBy1);
            //}
            if (Card.Ajacent1Check(player, info)) {
                //player.currentCell = info;
                player.Move(info,true);
                player.UseAction(true);
                return;
            }
            if (player.playingCard != null)
            {
                if (player.playingCard.move(player, info,true)){
                    board.manager.instruction.text = "You moved to a new position";
                    Debug.Log("moved");
                }
                
            }
            if (info.player != null && info.player != player)
            {
                Debug.Log(info.player.team);
                if (info.player.team == player.team && info.player.ghost)
                {
                    if ((info.row == player.row && (int)Mathf.Abs(info.column - player.col) == 1) || (info.column == player.col && (int)Mathf.Abs(info.row - player.row) == 1))
                    {
                        int sum = player.movementCards.Count;
                        int givingNum = (int)Mathf.FloorToInt(sum / 2);
                        if (givingNum > 0)
                        {
                            for (int i = 0; i < givingNum; ++i)
                            {
                                Card randomCard = player.PickRandomCard();
                                player.DiscardCard(randomCard);
                                info.player.AddCard(randomCard);
                                info.player.playerOG.GetComponent<SpriteRenderer>().sprite = info.player.humanSprite;
                                //Animation
                                board.manager.UpdateAnimation(info.player,false,false);
                                Debug.Log("You gave away " + randomCard.GetType().Name + "!");
                            }
                            player.UseAction();
                        }
                        else return;
                    }
                }
            }
        }
        Debug.Log(this.info.row + "," + this.info.column);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltip = info.board.manager.tooltip;
        
        Text tooltipText = tooltip.transform.GetComponentInChildren<Text>();
        if (info.containsCard)
        {
            Positioning(tooltip); ;
            tooltipText.text = "Card";
        }
        else if (info.containsEve)
        {
            Positioning(tooltip);
            tooltipText.text = "Event";
        }
        else if (info.containsItem)
        {
            Positioning(tooltip);
            tooltipText.text = "Item";
        }
        else if (info.obstacle)
        {
            Positioning(tooltip);
            tooltipText.text = "Obstacle";
        }
        else if (info.exit) {
            Positioning(tooltip);
            tooltipText.text = "Exit";
        }
        else tooltip.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        info.board.manager.tooltip.gameObject.SetActive(false);
    }

    void Positioning(Image tooltip) {
        tooltip.gameObject.SetActive(true);
        Vector2 pos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        tooltip.transform.position = pos;
    }
    public bool IsDropped() {
        if (collider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            return true;
        }
        return false;
    }
    //public void RemoveObstacle()
    //{
    //    GameObject obs = this.transform.GetChild(0).gameObject;
    //    Destroy(obs);
    //}
}
