using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    
public enum PossibleItems
{
    magnetRed,
    magnetBlue,
    longArm,
    longMug,
    blade,
    dagger,
    shield,
    landWrath,
    earthPower,
    tableFlip,
    oracle
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
    public abstract bool canPlay(PlayerState playedBy, PlayerState targetPlayer); 

    public virtual bool play(PlayerState playedBy, PlayerState targetPlayer,bool msg = false)
    {
        this.board.NotifyPlayerUsedItem(playedBy,targetPlayer, this, msg);
        board.currentPlayer.DiscardItem(this);
        return false;
    }
    public virtual bool play(PlayerState playedBy, GridInfo grid, bool msg = false)
    {
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

    //public static PossibleItems PickRandom(BoardState board)
    //{
    //    var entries = Enum.GetValues(typeof(PossibleItems));
    //    var index = board.random.Next(0, entries.Length);
    //    //var index = (int)Math.Floor((float)UnityEngine.Random.Range(0, entries.Length));
    //    return (PossibleItems)entries.GetValue(index);
    //}

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
        if (item == PossibleItems.dagger) {
            return new Dagger(board);
        }
        if (item == PossibleItems.shield) {
            return new Shield(board);
        }
        if (item == PossibleItems.landWrath)
        {
            return new LandWrath(board);
        }
        if (item == PossibleItems.earthPower)
        {
            return new EarthPower(board);
        }
        if (item == PossibleItems.tableFlip) {
            return new TableFlip(board);
        }
        if (item == PossibleItems.oracle) {
            return new Oracle(board);
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
        if (type == PossibleItems.dagger) {
            return typeof(Dagger);
        }
        if (type == PossibleItems.shield) {
            return typeof(Shield);
        }
        if (type == PossibleItems.landWrath)
        {
            return typeof(LandWrath);
        }
        if (type == PossibleItems.earthPower)
        {
            return typeof(EarthPower);
        }
        if (type == PossibleItems.tableFlip) {
            return typeof(TableFlip);
        }
        if (type == PossibleItems.oracle) {
            return typeof(Oracle);
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
        var index = board.random.Next(0, 100);
        if (index >= 0 && index < 12)
        {
            return Init(board, PossibleItems.magnetRed);
        }
        if (index >= 12 && index < 24)
        {
            return Init(board, PossibleItems.magnetBlue);
        }
        if (index >= 24 && index < 36)
        {
            return Init(board, PossibleItems.longArm);
        }
        if (index >= 36 && index < 51)
        {
            return Init(board, PossibleItems.longMug);
        }
        if (index >= 51 && index < 61) {
            return Init(board, PossibleItems.blade);
        }
        if (index >= 61 && index < 69) {
            return Init(board, PossibleItems.landWrath);
        }
        if (index >= 69 && index < 79) {
            return Init(board, PossibleItems.earthPower);
        }
        if (index >= 79 && index < 86) {
            return Init(board, PossibleItems.dagger);
        }
        if (index >= 86 && index < 97) {
            return Init(board, PossibleItems.shield);
        }
        if (index >= 97 && index <= 100) {
            return Init(board, PossibleItems.tableFlip);
        }
        return null;
        //return Init(board, PickRandom(board));
    }

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
        if (targetPlayer != null && targetPlayer.movementCards.Count > 0 && targetPlayer != playedBy)
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
                if (item is Shield) {
                    targetPlayer.DiscardItem(item);
                    playedBy.UseAction();
                    return true;
                }
            }
            if (targetPlayer.movementCards.Count >= 2) {
                Card discarded1 = targetPlayer.PickRandomCard();
                targetPlayer.DiscardCard(discarded1);
                playedBy.AddCard(discarded1);
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

}
public class MagnetBlue : Item
{
    public MagnetBlue(BoardState board) : base(board)
    {
    }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.movementCards.Count > 0 && targetPlayer != playedBy)
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
                if (item is Shield)
                {
                    targetPlayer.DiscardItem(item);
                    playedBy.UseAction();
                    return true;
                }
            }
            if (targetPlayer.movementCards.Count >= 2)
            {
                Card discarded1 = targetPlayer.PickRandomCard();
                targetPlayer.DiscardCard(discarded1);
                playedBy.AddCard(discarded1);
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
        if (targetPlayer != null && targetPlayer.items.Count > 0 && targetPlayer != playedBy)
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
            foreach (var item in targetPlayer.items)
            {
                if (item is Shield)
                {
                    targetPlayer.DiscardItem(item);
                    playedBy.UseAction();
                    return true;
                }
            }
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
    public LongMug(BoardState board) : base(board){}

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.items.Count > 0 && playedBy.items.Count>1 && targetPlayer != playedBy)
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
            foreach (var item in targetPlayer.items)
            {
                if (item is Shield)
                {
                    targetPlayer.DiscardItem(item);
                    playedBy.UseAction();
                    return true;
                }
            }
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
        if (targetPlayer != null && !targetPlayer.ghost && targetPlayer != playedBy)
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

public class Dagger : Item
{
    public Dagger(BoardState board) : base(board)
    {
    }
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && !targetPlayer.ghost) {
            if ((playedBy.currentCell.row == targetPlayer.currentCell.row && Mathf.Abs(playedBy.currentCell.column - targetPlayer.currentCell.column) == 1)
                || (playedBy.currentCell.column == targetPlayer.currentCell.column && Mathf.Abs(playedBy.currentCell.row - targetPlayer.currentCell.row) == 1))
                {
                return true;
            }
            else return false;
        }
        else return false;
    }
    public override bool play(PlayerState playedBy, PlayerState targetPlayer, bool msg = false)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            base.play(playedBy, targetPlayer, msg);
            foreach (var item in targetPlayer.items)
            {
                if (item is Shield)
                {
                    targetPlayer.DiscardItem(item);
                    playedBy.UseAction();
                    return true;
                }
            }
            if (targetPlayer.movementCards.Count >= 3)
            {
                for (int i = 0; i < 3; ++i)
                {
                    Card discarded = targetPlayer.PickRandomCard();
                    targetPlayer.DiscardCard(discarded);
                    playedBy.AddCard(discarded);
                }
            }
            else if (targetPlayer.movementCards.Count < 3) {
                for (int i = 0; targetPlayer.movementCards.Count > 0; ++i) {
                    Card discarded = targetPlayer.PickRandomCard();
                    targetPlayer.DiscardCard(discarded);
                    playedBy.AddCard(discarded);
                }
            }
            playedBy.UseAction(false);
            return true;
        }
        else
            return false;
    }

    public override string getDescription()
    {
        return "Use Dagger to forcefullly rob a player standing next to your of 2 cards.";
    }

    public override string getName()
    {
        return "Dagger";
    }
}

public class Shield : Item
{
    public Shield(BoardState board) : base(board) { }
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        return false;
    }

    public override string getDescription()
    {
        return "Automatically shield from other's steal of cards/items once and then disappear.";
    }

    public override string getName()
    {
        return "Shield";
    }
}

public class LandWrath : Item
{
    public LandWrath(BoardState board) : base(board) { }
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        return false;
    }
    public override bool play(PlayerState playedBy, GridInfo grid, bool msg = false)
    {
        base.play(playedBy, grid, msg);
        if (grid.obstacle) {
            board.RemoveObstacle(grid);
            playedBy.UseAction();
            return true;
        }
        else return false;
    }
    public override string getDescription()
    {
        return "Use it to destroy an obstacle on board.";
    }

    public override string getName()
    {
        return "Land Wrath";
    }
}

public class EarthPower : Item
{
    public EarthPower(BoardState board) : base(board) { }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        return false;
    }
    public override bool play(PlayerState playedBy, GridInfo grid, bool msg = false)
    {
        if(grid.player != null)
        {
            return false;
        }
        if (!grid.containsCard && !grid.containsEve && !grid.containsItem && !grid.exit && !grid.obstacle && !grid.hole)
        {
            base.play(playedBy, grid, msg);
            board.AddObstacle(grid);
            playedBy.UseAction();
            return true;
        }
        else return false;
    }
    public override string getDescription()
    {
        return "Use it to place an extra obstacle on board at a valid position.";
    }

    public override string getName()
    {
        return "Earth Power";
    }
}

public class TableFlip : Item
{
    public TableFlip(BoardState board) : base(board) { }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        return false;
    }

    public override bool play(PlayerState playedBy, GridInfo grid, bool msg = false) {
        base.play(playedBy, grid, msg);
        board.BoardFlip();
        board.manager.RecoveringGrids(true);
        board.manager.HighlightingBasicMoves();
        return true;
    }

    public override string getDescription()
    {
        return "Use it anywhere on the board to forcefully change the current board.";
    }

    public override string getName()
    {
        return "Table Flip";
    }
}

public class Oracle : Item
{
    public Oracle(BoardState board) : base(board) { }

    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        return false;
    }

    public override string getDescription()
    {
        return "The exlusive item. Holding this item will give you one extra card every time your turn starts.";
    }

    public override string getName()
    {
        return "Oracle";
    }
}