using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum possibleEvents {
    bounty,
    substitution,
    combo,
    shackle,
    handcuffs,
    calamity,
    amnesty,
    rubberBand,
    blindTrade,
    blackHole
}
public abstract class EventToken
{
    public static possibleEvents PickRandom(BoardState board) {
        var index = board.random.Next(0, 100);
        if (index >= 0 && index < 18)
        {
            return possibleEvents.bounty;
        }
        if (index >= 18 && index < 24)
        {
            return possibleEvents.substitution;
        }
        if (index >= 24 && index < 32)
        {
            return possibleEvents.combo;
        }
        if (index >= 32 && index < 50)
        {
            return possibleEvents.calamity;
        }
        if (index >= 50 && index < 65)
        {
            return possibleEvents.shackle;
        }
        if (index >= 65 && index < 78)
        {
            return possibleEvents.handcuffs;
        }
        if (index >= 78 && index < 88)
        {
            return possibleEvents.blackHole;
        }
        if (index >= 88 && index < 94)
        {
            return possibleEvents.rubberBand;
        }
        if (index >= 94 && index < 96)
        {
            return possibleEvents.blindTrade;
        }
        if (index >= 96 && index <= 100)
        {
            return possibleEvents.amnesty;
        }
        return possibleEvents.amnesty;
        //var entries = Enum.GetValues(typeof(possibleEvents));
        //var index = board.random.Next(0, entries.Length);
        ////var index = (int)Math.Floor((float)UnityEngine.Random.Range(0, entries.Length));
        //return (possibleEvents)entries.GetValue(index);
    }
    public static EventToken Init(possibleEvents eve) {
        if (eve == possibleEvents.bounty) {
            return new Bounty();
        }
        if (eve == possibleEvents.substitution) {
            return new Substitution();
        }
        if (eve == possibleEvents.combo) {
            return new Combo();
        }
        if (eve == possibleEvents.calamity) {
            return new Calamity();
        }
        if (eve == possibleEvents.amnesty) {
            return new Amnesty();
        }
        if (eve == possibleEvents.shackle) {
            return new Shackle();
        }
        if (eve == possibleEvents.handcuffs) {
            return new Handcuffs();
        }
        if (eve == possibleEvents.blindTrade) {
            return new BlindTrade();
        }
        if(eve == possibleEvents.blackHole)
        {
            return new BlackHole();
        }
        if(eve == possibleEvents.rubberBand)
        {
            return new RubberBand();
        }
        return null;
    }
    public abstract bool canEffect(PlayerState playedBy);
}

public class Bounty : EventToken {
    public override bool canEffect(PlayerState playedBy)
    {
        PlayerState mate = playedBy.board.GetTeammate(playedBy);
        if (playedBy.ghost)
        {
            mate.AddRandomCard();
            mate.AddRandomCard();
            return true;
        }
        else
        {           
            if (!mate.ghost)
            {
                playedBy.AddRandomCard();
                mate.AddRandomCard();
                return true;
            }
            else if (mate.ghost)
            {
                playedBy.AddRandomCard();
                playedBy.AddRandomCard();
                return true;
            }
        }
        return false;
    }
}

public class Substitution : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        PlayerState mate = playedBy.board.GetTeammate(playedBy);
        if (playedBy.ghost)
        {
            if (mate.items.Count < 5)
            {
                mate.AddRandomItem();
                return true;
            }
        }
        else
        {
            if (playedBy.items.Count < 5)
            {
                playedBy.AddRandomItem();
                return true;
            }
        }
        return false;
    }
}

public class Combo : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        PlayerState mate = playedBy.board.GetTeammate(playedBy);
        if (playedBy.ghost)
        {
            mate.AddRandomCard();
            if (mate.items.Count < 5)
            {
                mate.AddRandomItem();
            }
            return true;
        }
        else {
            playedBy.AddRandomCard();
            if (playedBy.items.Count < 5)
            {
                playedBy.AddRandomItem();
            }
            else if (!mate.ghost && mate.items.Count < 5) {
                mate.AddRandomItem();
            }
            return true;
        }
    }
}

public class Calamity : EventToken
{
    public override bool canEffect(PlayerState playedBy) {
        if (!playedBy.ghost)
        {
            foreach (var player in playedBy.board.players)
            {
                if (!player.ghost)
                {
                    player.DiscardCard(player.PickRandomCard());
                }
            }
            return true;
        }
        //List<PlayerState> opponents = new List<PlayerState>();
        //foreach (var player in playedBy.board.players) {
        //    if (player.team != playedBy.team && !player.ghost) {
        //        opponents.Add(player);
        //    }
        //}
        //if (opponents.Count > 1)
        //{
        //    if (opponents[0].movementCards.Count < opponents[1].movementCards.Count)
        //    {
        //        opponents[1].DiscardCard(opponents[1].PickRandomCard());
        //        return true;
        //    }

        //}
        //else
        //{
        //    opponents[0].DiscardCard(opponents[0].PickRandomCard());
        //    return true;
        //}
        return false;
    }
}

public class Amnesty : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        if (playedBy.ghost)
        {
            foreach (var player in playedBy.board.players)
            {
                if (player.ghost)
                {
                    player.AddRandomCard();
                }
            }
            return true;
        }
        else return false;
    }
}

public class Shackle : EventToken
{

    public override bool canEffect(PlayerState playedBy)
    {
        List<PlayerState> opponents = new List<PlayerState>();
        foreach (var player in playedBy.board.players)
        {
            if (player.team != playedBy.team)
            {
                opponents.Add(player);
            }
        }
        foreach (var player in opponents)
        {
            player.lessAction = true;
        }
        return true;
    }
}

public class Handcuffs : EventToken {
    public override bool canEffect(PlayerState playedBy)
    {
        List<PlayerState> opponents = new List<PlayerState>();
        foreach (var player in playedBy.board.players)
        {
            if (player.team != playedBy.team)
            {
                opponents.Add(player);
            }
        }
        foreach (var player in opponents)
        {
            player.noItem = true;
        }
        return true;
    }
}

public class RubberBand : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        BoardState board = playedBy.board;
        var teammate = board.GetTeammate(playedBy);
        int newRow = (12 - playedBy.row) + (int)Math.Truncate((double)(playedBy.row - teammate.row)/2);
        int newCol = playedBy.col + (int)Math.Truncate((double)(teammate.col - playedBy.col) / 2);
        GridInfo pos = board.GetGrid(newCol, newRow);
        GridInfo currentPos = playedBy.currentCell;
        if (!pos.obstacle && pos.player == null)
        {
            playedBy.currentCell = board.GetGrid(newCol, newRow);
            if (TeammateMove(playedBy) != null) {
                teammate.currentCell = TeammateMove(playedBy);
                return true;
            }
        }
        if (pos.obstacle) {
            playedBy.currentCell = board.GetGrid(newCol, newRow);
            foreach (var grid in playedBy.GetAdjacentGrids()) {
                if (!grid.obstacle && grid.player == null) {
                    playedBy.currentCell = grid;
                    if (TeammateMove(playedBy) != null) {
                        teammate.currentCell = TeammateMove(playedBy);
                        return true;
                    }
                }
            }
            foreach (var grid in playedBy.GetDiagonalGrids())
            {
                if (!grid.obstacle && grid.player == null)
                {
                    playedBy.currentCell = grid;
                    if (TeammateMove(playedBy) != null)
                    {
                        teammate.currentCell = TeammateMove(playedBy);
                        return true;
                    }
                }
            }
        }
        playedBy.currentCell = currentPos;
        return false;
    }

    public GridInfo TeammateMove(PlayerState playedBy) {
        foreach (var grid in playedBy.GetAdjacentGrids())
        {
            if (!grid.obstacle && grid.player == null)
            {
                return grid;
            }
        }
        foreach (var grid in playedBy.GetDiagonalGrids())
        {
            if (!grid.obstacle && grid.player == null)
            {
                return grid;
            }
        }
        return null;
    }
}

public class BlackHole : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        List<PlayerState> opponents = new List<PlayerState>();
        foreach (var player in playedBy.board.players)
        {
            if (player.team != playedBy.team)
            {
                opponents.Add(player);
            }
        }
        int index = playedBy.board.random.Next(0, 2);
        foreach(var grid in opponents[index].GetAdjacentGrids())
        {
            if (!grid.obstacle && grid.player == null)
            {
                playedBy.currentCell = grid;
                return true;
            }
        }
        foreach(var grid in opponents[index].GetDiagonalGrids())
        {
            if(!grid.obstacle && grid.player == null)
            {
                playedBy.currentCell = grid;
                return true;
            }
        }
        return false;
    }
}

public class BlindTrade : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        List<Card> cards = playedBy.movementCards;
        BoardState board = playedBy.board;
        List<PlayerState> tradingList = new List<PlayerState>(board.players);
        tradingList.Remove(playedBy);
        int index = board.random.Next(0, 3);
        PlayerState tradingPlayer = tradingList[index];
        playedBy.movementCards = tradingPlayer.movementCards;
        tradingPlayer.movementCards = cards;
        tradingList.Remove(tradingPlayer);
        cards = tradingList[0].movementCards;
        tradingList[0].movementCards = tradingList[1].movementCards;
        tradingList[1].movementCards = cards;
        return true;
    }
}