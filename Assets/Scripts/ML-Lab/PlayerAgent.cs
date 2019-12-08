using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class PlayerAgent : Agent
{
    private PlayerAcademy academy;
    BoardState board;
    public PlayerState player;

    public override void InitializeAgent()
    {
        this.agentParameters.onDemandDecision = true;
        academy = FindObjectOfType<PlayerAcademy>();
        board = academy.board;
        player = academy.GetSlot(this);
    }

    public float FeedPlayerInfo(GridInfo grid)
    {
        var gridPlayer = grid.player;

        if(gridPlayer != null)
        {
            if(player == gridPlayer)
            {
                //ME
                return 10;
            }
            if(player.team == gridPlayer.team)
            {
                //FRIEND
                return 5;
            }
            //BAD GUY
            var teammate = board.GetTeammate(gridPlayer);
            if(teammate.id > gridPlayer.id)
            {
                return 2;
            }
            return 3;
        }
        return 0;
    }

    public float FeedGridInfo(GridInfo grid)
    {
        if(grid.exit)
        {
            //EXIT
            //if(board.isLight)
            //{
            //    if(!player.canWin)
            //    {
            //        return 10;
            //    }
            //}
            //else
            //{
            //    if(player.canWin)
            //    {
            //        return 10;
            //    }
            //}
        }
        if(grid.obstacle)
        {
            return 1;
        }
        if(grid.containsCard)
        {
            return 2;
        }
        if(grid.containsItem)
        {
            return 3;
        }
        if(grid.containsEve)
        {
            return 4;
        }
        return 0;
    }

    public override void CollectObservations()
    {
        //Players
        foreach (var grid in board.GetCurrentGrids())
        {
            this.AddVectorObs(FeedPlayerInfo(grid));
        }
        //Board layout
        foreach (var grid in board.GetCurrentGrids())
        {
            this.AddVectorObs(FeedGridInfo(grid));
        }
        //Cards
        foreach(var card in Card.ListCards())
        {
            this.AddVectorObs(player.CountCards(card));
        }
        //Items
        foreach (var item in Item.ListItems())
        {
            this.AddVectorObs(player.CountItems(item));
        }
        //Other player card count
        var teammate = board.GetTeammate(player);
        this.AddVectorObs(teammate.movementCards.Count);
        foreach(PlayerState opposing in board.players)
        {
            if(opposing.team == player.team)
            {
                continue;
            }
            this.AddVectorObs(opposing.movementCards.Count);
        }
        //Other player item count
        this.AddVectorObs(teammate.items.Count);
        foreach (PlayerState opposing in board.players)
        {
            if (opposing.team == player.team)
            {
                continue;
            }
            this.AddVectorObs(opposing.items.Count);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var minAction = vectorAction[0];
        var originalPosition = player.currentCell;
        int boardSize = 13 * 13;
        //bool couldWin = player.canWin;
        for (int i = 1; i < boardSize;i++)
        {
            minAction = Math.Min(minAction, vectorAction[i]);
        }
        float decisionWeight = minAction;
        int decisionIndex = 0;
        int index = 0;
        for (; index < boardSize; index++)
        {
            var value = vectorAction[index];
            if(value > decisionWeight)
            {
                decisionWeight = value;
                decisionIndex = index;
            }
        }
        float itemDecisionWeight = 0;

        bool usingItem = false;
        PossibleItems selectedItem = PossibleItems.blade;
        foreach(PossibleItems item in Item.ListItems())
        {
            var value = vectorAction[index++];
            if(value > itemDecisionWeight && player.CountItems(item) > 0)
            {
                usingItem = true;
                selectedItem = item;
                itemDecisionWeight = value;
            }
        }
        float cardDecisionWeight = 0;
        bool usingCard = false;
        possibleCards selectedCard = possibleCards.adjacentBy2;
        foreach(possibleCards card in Card.ListCards())
        {
            var value = vectorAction[index++];
            if(value > cardDecisionWeight && player.CountCards(card) > 0)
            {
                usingCard = true;
                selectedCard = card;
                cardDecisionWeight = value;
            }
        }
        var selectedGrid = board.GetGrid(decisionIndex);
        if(!UseCardOrItem(selectedGrid,usingCard,selectedCard,usingItem,selectedItem))
        {
            float adjacentMoveWeight = minAction;
            GridInfo adjacentGrid = null;
            foreach(var grid in player.GetAdjacentGrids())
            {
                if(grid.obstacle || grid.player != null)
                {
                    continue;
                }
                var value = vectorAction[grid.index];
                if(value >= adjacentMoveWeight)
                {
                    adjacentGrid = grid;
                    adjacentMoveWeight = value;
                }
            }
            if (adjacentGrid != null)
            {
                player.Move(adjacentGrid, true);
            }
            player.UseAction(true);
        }
        var exit = board.GetExit();
        var reverseDistance = 20-player.currentCell.Distance(exit);
        var diff =  originalPosition.Distance(exit)-player.currentCell.Distance(exit);
        diff *= reverseDistance;
        if(diff < 0)
        {
            diff = diff * 1.5f;
        }
        //if(board.isLight && !player.canWin || !board.isLight && player.canWin)
        //    AddReward(diff);
        //academy.busy = false;
        //if(player.currentCell.exit && !player.ghost && player.canWin != couldWin && board.isLight)
        //{
        //    AddReward(10 * (board.maxTurns - board.currentTurn));
        //}
        //if (player.currentCell.exit && !player.ghost && player.canWin && !board.isLight)
        //{
        //    AddReward(100*(board.maxTurns-board.currentTurn));
        //    //Done();
        //    academy.AcademyDone();
        //    Debug.Log("Won: " + player.id);
        //}
    }



    public bool UseCardOrItem(GridInfo grid,bool useCard,possibleCards card,bool useItem,PossibleItems item)
    {
        if(useCard)
        {
            var cardObj = this.player.GetCardByType(card);
            if(cardObj != null && cardObj.move(this.player,grid,true))
            {
                return true;
            }
        }
        if(useItem && grid.player != null && !player.ghost)
        {
            var itemObj = this.player.GetItemByType(item);
            if(itemObj != null && itemObj.play(this.player,grid.player,true)) {
                return true;
            }
        }
        return false;
    }
}
