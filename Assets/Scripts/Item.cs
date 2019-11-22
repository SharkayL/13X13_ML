﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    
public enum PossibleItems
{
    magnetRed,
    magnetBlue,
    //bookworm,
    longArm,
    longMug,
    blade,
    //shield
    //ice,
    //earth
}

public abstract class Item
{
    //public GameObject itemToken;
    public int id;
    public BoardState board;

    public Item(BoardState board) {
        this.board = board;
        this.id = board.NextItemId();
        board.itemDatabase.Add(this.id, this);
    }
    public abstract bool canPlay(PlayerState playedBy, PlayerState targetPlayer); // highlight the grid

    public virtual bool play(PlayerState playedBy, PlayerState targetPlayer,bool msg = false)
    {
        this.board.NotifyPlayerUsedItem(playedBy,targetPlayer, this, msg);
        board.currentPlayer.DiscardItem(this);
        return false;
    }

    public abstract string getName();
    public abstract string getDescription();

    public static IEnumerable<PossibleItems> ListItems()
    {
        var entries = Enum.GetValues(typeof(PossibleItems));
        foreach(var entry in entries)
        {
            yield return (PossibleItems)entry;
        }
    }

    public static PossibleItems PickRandom(BoardState board)
    {
        var entries = Enum.GetValues(typeof(PossibleItems));
        var index = board.random.Next(0, entries.Length);
        //var index = (int)Math.Floor((float)UnityEngine.Random.Range(0, entries.Length));
        return (PossibleItems)entries.GetValue(index);
    }

    public static Item Init(BoardState board, PossibleItems item)
    {
        if (item == PossibleItems.magnetRed) {
            return new MagnetRed(board);
        }
        if (item == PossibleItems.magnetBlue)
        {
            return new MagnetBlue(board);
        }
        if (item == PossibleItems.longArm) {
            return new LongArm(board);
        }
        if (item == PossibleItems.longMug) {
            return new LongMug(board);
        }
        if (item == PossibleItems.blade) {
            return new Blade(board);
        }
        return null;
    }

    public static Type ExtractClass(PossibleItems type)
    {
        if(type == PossibleItems.blade)
        {
            return typeof(Blade);
        }
        if(type == PossibleItems.longArm)
        {
            return typeof(LongArm);
        }
        if(type == PossibleItems.longMug)
        {
            return typeof(LongMug);
        }
        if(type == PossibleItems.magnetBlue)
        {
            return typeof(MagnetBlue);
        }
        if(type == PossibleItems.magnetRed)
        {
            return typeof(MagnetRed);
        }
        return null;
    }

    public static int Count(List<Item> items,PossibleItems itemType)
    {
        var type = ExtractClass(itemType);
        int count = 0;
        foreach(var item in items)
        {
            if(item.GetType() == type)
            {
                count++;
            }
        }
        return count;
    }

    public static Item PickRandomObj(BoardState board)
    {
        return Init(board, PickRandom(board));
    }

    //public abstract GameObject DisplayItem(MatureManager manager);

    public bool UsedItem() {
        return false;
    }
}

public class MagnetRed : Item {
    public MagnetRed(BoardState board) : base(board)
    {
    }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.movementCards.Count > 0)
        {
            return true;
        }
        else // nothing will happen.
            return false;
    }
    public override bool play(PlayerState playedBy, PlayerState targetPlayer,bool msg = false)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            base.play(playedBy, targetPlayer, msg);
            foreach (var item in targetPlayer.items) {
                if (item is MagnetRed) {
                    targetPlayer.DiscardItem(item);
                    playedBy.UseAction();
                    return true;
                }
            }
            Card discarded = targetPlayer.PickRandomCard();
            targetPlayer.DiscardCard(discarded);
            playedBy.AddCard(discarded);
            playedBy.UseAction();
            return true;
        }
        else
            return false;
    }

    public override string getName()
    {
        return "Magnet Red";
    }
    public override string getDescription()
    {
        return "Use Magnet Red to steal a movement card from a random opponent. If the opponent has the same item too, no card will be stole but items will be discarded.";
    }


    //public override GameObject DisplayItem(MatureManager manager)
    //{
    //    itemToken = manager.item; 
    //    this.itemToken = MatureManager.Instantiate(manager.item);
    //    return itemToken;
    //}
}
public class MagnetBlue : Item
{
    public MagnetBlue(BoardState board) : base(board)
    {
    }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.movementCards.Count > 0)
        {
            return true;
        }
        else // nothing will happen.
            return false;
    }
    public override bool play(PlayerState playedBy, PlayerState targetPlayer,bool msg=false)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            base.play(playedBy, targetPlayer, msg);
            foreach (var item in targetPlayer.items)
            {
                if (item is MagnetBlue)
                {
                    targetPlayer.DiscardItem(item);
                    playedBy.UseAction();
                    return true;
                }
            }
            Card discarded = targetPlayer.PickRandomCard();
            targetPlayer.DiscardCard(discarded);
            playedBy.AddCard(discarded);
            playedBy.UseAction();
            return true;
        }
        else
            return false;
    }

    public override string getName()
    {
        return "Magnet Blue";
    }
    public override string getDescription()
    {
        return "Use Magnet Red to steal a movement card from a random opponent. If the opponent has the same item too, no card will be stole but items will be discarded."; 
    }
}

public class LongArm : Item {
    public LongArm(BoardState board) : base(board)
    {
    }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.items.Count > 0)
        {
            return true;
        }
        else 
        return false;
    }

    public override bool play(PlayerState playedBy, PlayerState targetPlayer,bool msg = false)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            base.play(playedBy, targetPlayer, msg);
            Item discarded = targetPlayer.PickRandomItem();
            targetPlayer.DiscardItem(discarded);
            playedBy.AddItem(discarded);
            playedBy.UseAction();
            return true;
        }
        else
            return false;
    }
    public override string getName()
    {
        return "Long Arm";
    }
    public override string getDescription()
    {
        return "Use Long Arm to steal an item form a random opponent. You may not see the item and only choose blindly.";
    }
}

public class LongMug : Item
{
    public LongMug(BoardState board) : base(board)
    {
    }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.items.Count > 0 && playedBy.items.Count>1)
        {
            return true;
        }
        else
            return false;
    }

    public override bool play(PlayerState playedBy, PlayerState targetPlayer,bool msg = false)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            base.play(playedBy, targetPlayer, msg);
            Item discardedT = targetPlayer.PickRandomItem();
            Item discardedP = playedBy.PickRandomItem();
            targetPlayer.DiscardItem(discardedT);
            targetPlayer.AddItem(discardedP);
            //playedBy.DiscardItem(discardedP);
            playedBy.AddItem(discardedT);
            playedBy.UseAction();
            return true;
        }
        else
            return false;
    }
    public override string getName()
    {
        return "Long Mug";
    }
    public override string getDescription()
    {
        return "Use Long Mug to swap a random enemy’s item with one of yours.";
    }
}
public class Blade : Item
{
    public Blade(BoardState board) : base(board)
    {
    }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && !targetPlayer.ghost)
        {
            return true;
        }
        else
            return false;
    }

    public override bool play(PlayerState playedBy, PlayerState targetPlayer,bool msg = false)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            base.play(playedBy, targetPlayer, msg);
            GridInfo currentPos = playedBy.currentCell;
            playedBy.currentCell = targetPlayer.currentCell;
            targetPlayer.currentCell = currentPos;
            playedBy.UseAction(false);
            return true;
        }
        else
            return false;
    }
    public override string getName()
    {
        return "Teleportation Blade";
    }
    public override string getDescription()
    {
        return "Use Teleporation Blade to forcefully swap your current position with a human player's.";
    }
}
