using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIGrid : MonoBehaviour, IPointerClickHandler
{
    public GridInfo info;

public void OnPointerClick(PointerEventData eventData)
    {
        var board = info.board;
        var player = board.currentPlayer;
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
                player.Move(info);
                if (info.exit)
                {
                    board.NotifyGameover(player.team);
                }
                player.UseAction();
                return;
            }
            if (player.playingCard != null)
            {
                if (player.playingCard.move(player, info)){
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
}
