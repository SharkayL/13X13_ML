﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum possibleCards
{
    //adjacentBy1,
    diagonalBy1,
    adjacentBy2,
    diagonalBy2,
    cross1Obstacle,
    straightBy3,
}
public abstract class Card
{
    //public Image displayImg;
    public PlayerState heldBy;
    public BoardState board;

    public static bool baseCheck(PlayerState playedBy, GridInfo moveTo)
    {
        return moveTo.player == null && !moveTo.obstacle;
    }

    //public static Card Convert(possibleCards card)
    //{
    //    if (card == possibleCards.adjacentBy1)
    //    {
    //        return new AdjacentBy1();
    //    }
    //    return null;
    //}
    public static bool Ajacent1Check(PlayerState playedBy, GridInfo moveTo) {
        if (Card.baseCheck(playedBy, moveTo))
        {
            if ((moveTo.row == playedBy.row && Math.Abs(moveTo.column - playedBy.col) == 1) || (moveTo.column == playedBy.col && Math.Abs(moveTo.row - playedBy.row) == 1))
            {
                return true;
            }
        }
        return false;
    }
    public abstract bool canMove(PlayerState playedBy, GridInfo moveTo);

    public bool move(PlayerState playedBy, GridInfo moveTo)
    {
        if (!canMove(playedBy, moveTo))
        {
            return false;
        }
        playedBy.currentCell = moveTo;
        if (moveTo.exit) {
            board.NotifyGameover(playedBy.team);
        }
        playedBy.DiscardCard(this);
        playedBy.UseAction();
        if (playedBy.ghost)
        {
            if (board.gameOver)
            {
                int winnerTeam = Math.Abs(playedBy.team - 1);
                board.NotifyGameover(winnerTeam);
            }
        }
        return true;
    }

    public static possibleCards PickRandom()
    {
        var entries = Enum.GetValues(typeof(possibleCards));
        var index = (int)Math.Floor((float)UnityEngine.Random.Range(0, entries.Length));
        return (possibleCards)entries.GetValue(index);
    }

    public static Card Init(possibleCards card)
    {
        //if (card == possibleCards.adjacentBy1)
        //{
        //    return new AdjacentBy1();
        //}
        //else 
        if (card == possibleCards.adjacentBy2)
        {
            return new AdjacentBy2();
        }
        else if (card == possibleCards.diagonalBy1)
        {
            return new DiagonalBy1();
        }
        else if (card == possibleCards.diagonalBy2)
        {
            return new DiagonalBy2();
        }
        else if (card == possibleCards.cross1Obstacle)
        {
            return new Cross1Obstacle();
        }
        else if (card == possibleCards.straightBy3) {
            return new StraightBy3();
        }
        return null;
    }
    public static Card PickRandomObj()
    {
        return Init(PickRandom());
    }
    public static bool LineCheck(int x, int y, int dis)
    {
        if (Math.Abs(x - y) == dis)
        {
            return true;
        }
        return false;
    }
    public bool ObstacleCheck(PlayerState playedBy, GridInfo moveTo)
    {
        GridInfo midGrid = board.GetGrid(playedBy.col + (moveTo.column - playedBy.col) / 2, playedBy.row + (moveTo.row - playedBy.row) / 2);
        Debug.Log(midGrid.obstacle + "," + midGrid.row + "," + midGrid.column);
        return midGrid.obstacle;
    }
}

//public class AdjacentBy1 : Card
//{

//    public override bool canMove(PlayerState playedBy, GridInfo moveTo)
//    {
//        if (Card.baseCheck(playedBy, moveTo))
//        {
//            if ((moveTo.row == playedBy.row && Math.Abs(moveTo.column - playedBy.col) == 1) || (moveTo.column == playedBy.col && Math.Abs(moveTo.row - playedBy.row) == 1))
//            {
//                return true;
//            }
//        }
//        return false;
//    }
//}

public class AdjacentBy2 : Card
{
    public override bool canMove(PlayerState playedBy, GridInfo moveTo)
    {
        if (Card.baseCheck(playedBy, moveTo))
        {
            if ((moveTo.row == playedBy.row && Math.Abs(moveTo.column - playedBy.col) == 2) || (moveTo.column == playedBy.col && Math.Abs(moveTo.row - playedBy.row) == 2))
            {
                if (!ObstacleCheck(playedBy, moveTo)) {
                    return true;
                }

            }
        }
        return false;
    }
}

public class DiagonalBy1 : Card
{
    public override bool canMove(PlayerState playedBy, GridInfo moveTo)
    {
        if (Card.baseCheck(playedBy, moveTo))
        {
            if (LineCheck(moveTo.row, playedBy.row, 1) && LineCheck(moveTo.column, playedBy.col, 1))
            {
                return true;
            }
        }
        return false;
    }
}

public class DiagonalBy2 : Card
{
    public override bool canMove(PlayerState playedBy, GridInfo moveTo)
    {
        if (Card.baseCheck(playedBy, moveTo))
        {
            if (LineCheck(moveTo.row, playedBy.row, 2) && LineCheck(moveTo.column, playedBy.col, 2))
            {
                if (!ObstacleCheck(playedBy, moveTo)) {
                    return true;
                }
            }
        }
        return false;
    }
}

public class Cross1Obstacle : Card
{
    public override bool canMove(PlayerState playedBy, GridInfo moveTo)
    {
        if (Card.baseCheck(playedBy, moveTo))
        {
            if ((moveTo.row == playedBy.row && Math.Abs(moveTo.column - playedBy.col) == 2) || (moveTo.column == playedBy.col && Math.Abs(moveTo.row - playedBy.row) == 2))
            {
                return true;
            }
        }
        return false;
    }
}

public class StraightBy3 : Card
{
    public override bool canMove(PlayerState playedBy, GridInfo moveTo)
    {
        if (Card.baseCheck(playedBy, moveTo))
        {
            if (moveTo.row == playedBy.row && Math.Abs(moveTo.column - playedBy.col) == 3) {
                int g = moveTo.column - playedBy.col;
                if (board.GetGrid(playedBy.col + g / 3, playedBy.row).obstacle) { return false; }
                if (board.GetGrid(playedBy.col + g / 3 * 2, playedBy.row).obstacle) { return false; }
                else {
                    Debug.Log(playedBy.team);
                    return true; }
            }
            else if (moveTo.column == playedBy.col && Math.Abs(moveTo.row - playedBy.row) == 3) {
                int g = moveTo.row - playedBy.row;
                if (board.GetGrid(playedBy.col, playedBy.row + g/3).obstacle) { return false; }
                if (board.GetGrid(playedBy.col, playedBy.row + g/3*2).obstacle) { return false; }
                else return true;
            }
            
        }
        return false;
    }
}