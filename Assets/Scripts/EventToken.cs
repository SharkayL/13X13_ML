using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum possibleEvents {
    adding2Cards,
    adding1Item,
    adding1Both,
    //decreasingMoves,
    //banning1Item,
    losing1card,
    becomingRegular
}
public abstract class EventToken
{
    //public GameObject eventToken;

    public static possibleEvents PickRandom() {
        var entries = Enum.GetValues(typeof(possibleEvents));
        var index = (int)Math.Floor((float)UnityEngine.Random.Range(0, entries.Length));
        return (possibleEvents)entries.GetValue(index);
    }
    public static EventToken Init(possibleEvents eve) {
        if (eve == possibleEvents.adding2Cards) {
            return new Adding2Cards();
        }
        if (eve == possibleEvents.adding1Item) {
            return new Adding1Item();
        }
        if (eve == possibleEvents.adding1Both) {
            return new Adding1Both();
        }
        if (eve == possibleEvents.losing1card) {
            return new Losing1Card();
        }
        if (eve == possibleEvents.becomingRegular) {
            return new BecomingRegular();
        }
        return null;
    }
    public abstract bool canEffect(PlayerState playedBy);
}

public class Adding2Cards : EventToken {
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

public class Adding1Item : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        PlayerState mate = playedBy.board.GetTeammate(playedBy);
        if (playedBy.ghost)
        {
            if (mate.items.Count < 7)
            {
                mate.AddRandomItem();
                return true;
            }
        }
        else
        {
            if (playedBy.items.Count < 7)
            {
                playedBy.AddRandomItem();
                return true;
            }
        }
        return false;
    }
}

public class Adding1Both : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        PlayerState mate = playedBy.board.GetTeammate(playedBy);
        if (playedBy.ghost)
        {
            mate.AddRandomCard();
            if (mate.items.Count < 7)
            {
                mate.AddRandomItem();
            }
            return true;
        }
        else {
            playedBy.AddRandomCard();
            if (playedBy.items.Count < 7)
            {
                playedBy.AddRandomItem();
            }
            else if (!mate.ghost && mate.items.Count < 7) {
                mate.AddRandomItem();
            }
            return true;
        }
    }
}

public class Losing1Card : EventToken
{
    public override bool canEffect(PlayerState playedBy) {
        List<PlayerState> opponents = new List<PlayerState>();
        foreach (var player in playedBy.board.players) {
            if (player.team != playedBy.team && !player.ghost) {
                opponents.Add(player);
            }
        }
        if (opponents.Count > 1)
        {
            if (opponents[0].movementCards.Count < opponents[1].movementCards.Count)
            {
                opponents[1].DiscardCard(opponents[1].PickRandomCard());
                return true;
            }
            
        }
        else
        {
            opponents[0].DiscardCard(opponents[0].PickRandomCard());
            return true;
        }
        return false;
    }
}

public class BecomingRegular : EventToken
{
    public override bool canEffect(PlayerState playedBy)
    {
        foreach (var player in playedBy.board.players) {
            if (player.ghost) {
                player.AddRandomCard();
            }
        }
        return true;
    }
}
