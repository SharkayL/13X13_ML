using System;
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

    public abstract bool canPlay(PlayerState playedBy, PlayerState targetPlayer); // highlight the grid

    public abstract bool play(PlayerState playedBy, PlayerState targetPlayer);

    public abstract string getName();
    public abstract string getDescription();

    public static PossibleItems PickRandom()
    {
        var entries = Enum.GetValues(typeof(PossibleItems));
        var index = (int)Math.Floor((float)UnityEngine.Random.Range(0, entries.Length));
        return (PossibleItems)entries.GetValue(index);
    }

    public static Item Init(PossibleItems item)
    {
        if (item == PossibleItems.magnetRed) {
            return new MagnetRed();
        }
        if (item == PossibleItems.magnetBlue)
        {
            return new MagnetBlue();
        }
        if (item == PossibleItems.longArm) {
            return new LongArm();
        }
        if (item == PossibleItems.longMug) {
            return new LongMug();
        }
        if (item == PossibleItems.blade) {
            return new Blade();
        }
        return null;
    }

    public static Item PickRandomObj()
    {
        return Init(PickRandom());
    }

    //public abstract GameObject DisplayItem(MatureManager manager);

    public bool UsedItem() {
        return false;
    }
}

public class MagnetRed : Item {
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.movementCards.Count > 0)
        {
            return true;
        }
        else // nothing will happen.
            return false;
    }
    public override bool play(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (canPlay(playedBy, targetPlayer))
        {
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
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.movementCards.Count > 0)
        {
            return true;
        }
        else // nothing will happen.
            return false;
    }
    public override bool play(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (canPlay(playedBy, targetPlayer))
        {
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
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.items.Count > 0)
        {
            return true;
        }
        else 
        return false;
    }

    public override bool play(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (canPlay(playedBy, targetPlayer))
        {
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
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && targetPlayer.items.Count > 0)
        {
            return true;
        }
        else
            return false;
    }

    public override bool play(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            Item discardedT = targetPlayer.PickRandomItem();
            Item discardedP = playedBy.PickRandomItem();
            targetPlayer.DiscardItem(discardedT);
            targetPlayer.AddItem(discardedP);
            playedBy.DiscardItem(discardedP);
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
    public override bool canPlay(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (targetPlayer != null && !targetPlayer.ghost)
        {
            return true;
        }
        else
            return false;
    }

    public override bool play(PlayerState playedBy, PlayerState targetPlayer)
    {
        if (canPlay(playedBy, targetPlayer))
        {
            GridInfo currentPos = playedBy.currentCell;
            playedBy.currentCell = targetPlayer.currentCell;
            targetPlayer.currentCell = currentPos;
            playedBy.UseAction();
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
