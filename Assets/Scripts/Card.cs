using System;
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
    public int id;
    public PlayerState heldBy;
    public BoardState board;

    public Card(BoardState board)
    {
        this.board = board;
        this.id = board.NextCardId();
        board.cardDatabase.Add(this.id,this);
    }

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

    public bool move(PlayerState playedBy, GridInfo moveTo,bool msg = false)
    {
        if (!canMove(playedBy, moveTo))
        {
            return false;
        }
        this.board.NotifyPlayerUsedCard(playedBy, this, moveTo, msg);
        //playedBy.currentCell = moveTo;
        playedBy.Move(moveTo);
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
    public abstract string getName();
    public abstract string getDescription();
    public static possibleCards PickRandom(BoardState state)
    {
        var entries = Enum.GetValues(typeof(possibleCards));
        var index = state.random.Next(0, entries.Length);
        //var index = (int)Math.Floor((float)UnityEngine.Random.Range(0, entries.Length));
        return (possibleCards)entries.GetValue(index);
    }

    public static Card Init(BoardState board,possibleCards card)
    {
        //if (card == possibleCards.adjacentBy1)
        //{
        //    return new AdjacentBy1();
        //}
        //else 
        if (card == possibleCards.adjacentBy2)
        {
            return new AdjacentBy2(board);
        }
        else if (card == possibleCards.diagonalBy1)
        {
            return new DiagonalBy1(board);
        }
        else if (card == possibleCards.diagonalBy2)
        {
            return new DiagonalBy2(board);
        }
        else if (card == possibleCards.cross1Obstacle)
        {
            return new Cross1Obstacle(board);
        }
        else if (card == possibleCards.straightBy3) {
            return new StraightBy3(board);
        }
        return null;
    }

    public static Type ExtractClass(possibleCards type)
    {
        if (type == possibleCards.adjacentBy2)
        {
            return typeof(AdjacentBy2);
        }
        if(type == possibleCards.cross1Obstacle)
        {
            return typeof(Cross1Obstacle);
        }
        if(type == possibleCards.diagonalBy1)
        {
            return typeof(DiagonalBy1);
        }
        if(type == possibleCards.diagonalBy2)
        {
            return typeof(DiagonalBy2);
        }
        if(type == possibleCards.straightBy3)
        {
            return typeof(StraightBy3);
        }
        return null;
    }

    public static int Count(List<Card> cards, possibleCards cardType)
    {
        var type = ExtractClass(cardType);
        int count = 0;
        foreach (var card in cards)
        {
            if (card.GetType() == type)
            {
                count++;
            }
        }
        return count;
    }

    public static IEnumerable<possibleCards> ListCards()
    {
        var entries = Enum.GetValues(typeof(possibleCards));
        foreach (var entry in entries)
        {
            yield return (possibleCards)entry;
        }
    }

    public static Card PickRandomObj(BoardState board)
    {
        return Init(board,PickRandom(board));
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
    public AdjacentBy2(BoardState board) : base(board)
    {
    }

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
    public override string getName()
    {
        return "Charge Ahead";
    }
    public override string getDescription()
    {
        return "Allow you to move orthoganally by 2 grids.";
    }
}

public class DiagonalBy1 : Card
{
    public DiagonalBy1(BoardState board) : base(board)
    {
    }

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
    public override string getName()
    {
        return "Dodge Ahead";
    }
    public override string getDescription()
    {
        return "Allow you to move diagonally by 1 grid.";
    }
}

public class DiagonalBy2 : Card
{
    public DiagonalBy2(BoardState board) : base(board)
    {
    }

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
    public override string getName()
    {
        return "Flank Ahead";
    }
    public override string getDescription()
    {
        return "Allow you to move diagonally by 2 grids.";
    }
}

public class Cross1Obstacle : Card
{
    public Cross1Obstacle(BoardState board) : base(board)
    {
    }

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
    public override string getName()
    {
        return "Wrap Ahead";
    }
    public override string getDescription()
    {
        return "Allow you to skip over 1 obstacle.";
    }
}

public class StraightBy3 : Card
{
    public StraightBy3(BoardState board) : base(board)
    {
    }

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
    public override string getName()
    {
        return "Lead Ahead";
    }
    public override string getDescription()
    {
        return "Allow you to move orthoganally by 3 grids.";
    }
}