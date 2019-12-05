using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalBoards
{
    PlaceCard[] cardPlacements;
    PlaceItem[] itemPlacements;
    PlaceEvent[] eventPlacements;
    Placement[] obstaclePlacements;
    public List<Placement> pendingObstacles = new List<Placement>();
    public Placement exit;

    internal AdditionalBoards(PlaceCard[] cardPlacements, PlaceItem[] itemPlacements, PlaceEvent[] eventPlacements, Placement[] obstaclePlacements, Placement exit) {
        this.cardPlacements = cardPlacements;
        this.itemPlacements = itemPlacements;
        this.eventPlacements = eventPlacements;
        this.obstaclePlacements = obstaclePlacements;
        this.exit = exit;
    }

    public void InitLayout(BoardState board, int roundsToFlip)
    {
        pendingObstacles.Clear();
        for (int i = 0; i < roundsToFlip; ++i)
        {
            Placement selectedObs = null;
            do
            {
                int index = board.random.Next(0, obstaclePlacements.Length);
                selectedObs = obstaclePlacements[index];
                pendingObstacles.Add(selectedObs);
                if (pendingObstacles.Count >= (roundsToFlip - 1) * 2) {
                    break;
                }
            }
            while (!CheckObstaclesList(selectedObs));
        }

        foreach (var eve in eventPlacements)
        {
            board.GetGrid(eve).containsEve = true;
        }
        foreach (var card in cardPlacements)
        {
            board.GetGrid(card).containsCard = true;
        }
        foreach (var item in itemPlacements)
        {
            board.GetGrid(item).containsItem = true;
        }
        foreach (var obstacle in obstaclePlacements)
        {
            board.GetGrid(obstacle).obstacle = true;
        }
        foreach (var pendingObs in pendingObstacles)
        {
            board.GetGrid(pendingObs).obstacle = false;
        }

        board.GetGrid(exit).exit = true;
    }
    bool CheckObstaclesList(Placement selectedObs) {
        if(selectedObs == null) {
            return false;
        }
        if (pendingObstacles.Count > 0) {
            foreach (var obs in pendingObstacles) {
                if (selectedObs.row == obs.row && selectedObs.col == obs.col) {
                    return false;
                }
            }
        }
        return true;
    }
    public static List<AdditionalBoards> Boards()
    {
        List<AdditionalBoards> boards = new List<AdditionalBoards>();
        boards.Add(Layout1.NewBoard());
        boards.Add(Layout2.NewBoard());
        boards.Add(Layout3.NewBoard());
        boards.Add(Layout4.NewBoard());
        boards.Add(Layout5.NewBoard());
        boards.Add(Layout6.NewBoard());
        boards.Add(Layout7.NewBoard());
        return boards;
    }
}
//row col
internal class Layout1
{
    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(2, 3),
        new PlaceCard(2, 9),
        new PlaceCard(6, 3),
        new PlaceCard(6, 9),
        new PlaceCard(7, 2),
        new PlaceCard(7, 10),
        new PlaceCard(8, 4),
        new PlaceCard(8, 8),
        new PlaceCard(10, 0),
        new PlaceCard(10, 1),
        new PlaceCard(10, 5),
        new PlaceCard(10, 7),
        new PlaceCard(10, 11),
        new PlaceCard(10, 12),
        new PlaceCard(11, 4),
        new PlaceCard(11, 8),
        new PlaceCard(12, 2),
        new PlaceCard(12, 4),
        new PlaceCard(12, 5),
        new PlaceCard(12, 7),
        new PlaceCard(12, 8),
        new PlaceCard(12, 10)
    };
    static PlaceItem[] itemPlacements = new PlaceItem[] {
        new PlaceItem(1, 1),
        new PlaceItem(1, 11),
        new PlaceItem(4, 2),
        new PlaceItem(4, 10),
        new PlaceItem(11, 0),
        new PlaceItem(11, 2),
        new PlaceItem(11, 10),
        new PlaceItem(11, 12),
        new PlaceItem(12, 1),
        new PlaceItem(12, 3),
        new PlaceItem(12, 6),
        new PlaceItem(12, 9),
        new PlaceItem(12, 11)
    };
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(0, 3),
        new PlaceEvent(0, 9),
        new PlaceEvent(2, 4),
        new PlaceEvent(2, 8),
        new PlaceEvent(4, 5),
        new PlaceEvent(4, 7),
        new PlaceEvent(9, 2),
        new PlaceEvent(9, 10)
    };
    static Placement[] obstaclePlacements = new Placement[]
    {
        new Placement(3, 3),
        new Placement(3, 9),
        new Placement(6, 6),
        new Placement(8, 2),
        new Placement(8, 10),
        new Placement(9, 0),
        new Placement(9, 1),
        new Placement(9, 3),
        new Placement(9, 9),
        new Placement(9, 11),
        new Placement(9, 12),
        new Placement(10, 2),
        new Placement(10, 3),
        new Placement(10, 4),
        new Placement(10, 8),
        new Placement(10, 9),
        new Placement(10, 10),
        new Placement(11, 5),
        new Placement(11, 7)
    };
    static Placement exit = new Placement(0, 6);

    internal static AdditionalBoards NewBoard()
    {
        return new AdditionalBoards(cardPlacements, itemPlacements, eventPlacements, obstaclePlacements, exit);
    }
}

internal class Layout2
{
    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(1, 1),
        new PlaceCard(1, 6),
        new PlaceCard(1, 11),
        new PlaceCard(5, 0),
        new PlaceCard(5, 12),
        new PlaceCard(9, 2),
        new PlaceCard(9, 6),
        new PlaceCard(9, 10),
        new PlaceCard(11, 0),
        new PlaceCard(11, 12),
        new PlaceCard(12, 4),
        new PlaceCard(12, 8)
    };
    static PlaceItem[] itemPlacements = new PlaceItem[] {
        new PlaceItem(0, 5),
        new PlaceItem(0, 7),
        new PlaceItem(4, 1),
        new PlaceItem(4, 11),
        new PlaceItem(5, 4),
        new PlaceItem(5, 8)
    };
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(3, 6),
        new PlaceEvent(8, 3),
        new PlaceEvent(8, 9),
        new PlaceEvent(11, 1),
        new PlaceEvent(11, 6),
        new PlaceEvent(11, 11)
    };
    static Placement[] obstaclePlacements = new Placement[] {
        new Placement(0, 6),
        new Placement(2, 2),
        new Placement(2, 3),
        new Placement(2, 9),
        new Placement(2, 10),
        new Placement(5, 6),
        new Placement(6, 0),
        new Placement(6, 12),
        new Placement(7, 3),
        new Placement(7, 4),
        new Placement(7, 8),
        new Placement(7, 9),
        new Placement(8, 1),
        new Placement(8, 5),
        new Placement(8, 6),
        new Placement(8, 7),
        new Placement(8, 11),
        new Placement(9, 3),
        new Placement(9, 9),
        new Placement(10, 3),
        new Placement(10, 5),
        new Placement(10, 6),
        new Placement(10, 7),
        new Placement(10, 9),
        new Placement(11, 3),
        new Placement(11, 5),
        new Placement(11, 7),
        new Placement(11, 9),
        new Placement(12, 2),
        new Placement(12, 5),
        new Placement(12, 7),
        new Placement(12, 10)
    };
    static Placement exit = new Placement(12, 6);
    internal static AdditionalBoards NewBoard()
    {
        return new AdditionalBoards(cardPlacements, itemPlacements, eventPlacements, obstaclePlacements, exit);
    }
}

internal class Layout3
{
    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(0, 9),
        new PlaceCard(0, 12),
        new PlaceCard(1, 1),
        new PlaceCard(3, 7),
        new PlaceCard(3, 12),
        new PlaceCard(4, 4),
        new PlaceCard(5, 0),
        new PlaceCard(5, 9),
        new PlaceCard(6, 11),
        new PlaceCard(7, 4),
        new PlaceCard(9, 6),
        new PlaceCard(10, 10),
        new PlaceCard(11, 1),
        new PlaceCard(11, 8),
        new PlaceCard(12, 3),
        new PlaceCard(12, 11)
    };
    static PlaceItem[] itemPlacements = new PlaceItem[] {
        new PlaceItem(0, 3),
        new PlaceItem(1, 11),
        new PlaceItem(2, 4),
        new PlaceItem(5, 6),
        new PlaceItem(6, 2),
        new PlaceItem(7, 7),
        new PlaceItem(8, 0),
        new PlaceItem(8, 5),
        new PlaceItem(8, 9),
        new PlaceItem(9, 2),
        new PlaceItem(9, 11),
        new PlaceItem(11, 5)
    };
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(0, 1),
        new PlaceEvent(2, 7),
        new PlaceEvent(2, 9),
        new PlaceEvent(3, 11),
        new PlaceEvent(4, 0),
        new PlaceEvent(4, 6),
        new PlaceEvent(6, 8),
        new PlaceEvent(6, 12),
        new PlaceEvent(9, 9),
        new PlaceEvent(10, 1)
    };
    static Placement[] obstaclePlacements = new Placement[] {
        new Placement(0, 6),
        new Placement(1, 2),
        new Placement(1, 8),
        new Placement(2, 8),
        new Placement(2, 10),
        new Placement(2, 11),
        new Placement(3, 5),
        new Placement(4, 7),
        new Placement(4, 8),
        new Placement(4, 10),
        new Placement(5, 1),
        new Placement(5, 11),
        new Placement(7, 10),
        new Placement(8, 4),
        new Placement(9, 7)
    };
    static Placement exit = new Placement(3, 9);
    internal static AdditionalBoards NewBoard()
    {
        return new AdditionalBoards(cardPlacements, itemPlacements, eventPlacements, obstaclePlacements, exit);
    }
}

internal class Layout4
{
    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(0, 0),
        new PlaceCard(0, 5),
        new PlaceCard(0, 9),
        new PlaceCard(1, 2),
        new PlaceCard(2, 12),
        new PlaceCard(3, 10),
        new PlaceCard(4, 2),
        new PlaceCard(5, 4),
        new PlaceCard(5, 8),
        new PlaceCard(6, 6),
        new PlaceCard(6, 12),
        new PlaceCard(7, 1),
        new PlaceCard(8, 9),
        new PlaceCard(9, 5),
        new PlaceCard(9, 11),
        new PlaceCard(11, 0),
        new PlaceCard(11, 4),
        new PlaceCard(12, 2),
        new PlaceCard(12, 6),
        new PlaceCard(12, 10)
    };
    static PlaceItem[] itemPlacements = new PlaceItem[] {
        new PlaceItem(0, 7),
        new PlaceItem(0, 12),
        new PlaceItem(1, 5),
        new PlaceItem (1, 10),
        new PlaceItem(2, 1),
        new PlaceItem(3, 6),
        new PlaceItem(3, 9),
        new PlaceItem(4, 4),
        new PlaceItem(4, 11),
        new PlaceItem(5, 0),
        new PlaceItem(6, 10),
        new PlaceItem(7, 12),
        new PlaceItem(9, 0),
        new PlaceItem(10, 2),
        new PlaceItem(10, 6),
        new PlaceItem(10, 9),
        new PlaceItem (11, 11),
        new PlaceItem(12, 5)
    };
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(6, 7),
        new PlaceEvent(7, 5),
        new PlaceEvent(9, 6),
        new PlaceEvent(11, 2)
    };
    static Placement[] obstaclePlacements = new Placement[] {
        new Placement(1, 9),
        new Placement (2, 4),
        new Placement(2, 5),
        new Placement(2, 6),
        new Placement(2, 10),
        new Placement(3, 2),
        new Placement(3, 3),
        new Placement(3, 11),
        new Placement(4, 1),
        new Placement(4, 7),
        new Placement(4, 8),
        new Placement(5, 1),
        new Placement(5, 9),
        new Placement(6, 1),
        new Placement(6, 4),
        new Placement(6, 11),
        new Placement(7, 3),
        new Placement(7,6),
        new Placement (7, 7),
        new Placement(8, 2),
        new Placement(9, 2),
        new Placement(9, 3),
        new Placement(9, 4),
        new Placement(9, 8),
        new Placement(10, 5),
        new Placement(10, 8),
        new Placement(11, 3),
        new Placement(11, 6),
        new Placement(11, 8)
    };
    static Placement exit = new Placement(8, 4);
    internal static AdditionalBoards NewBoard()
    {
        return new AdditionalBoards(cardPlacements, itemPlacements, eventPlacements, obstaclePlacements, exit);
    }
}

internal class Layout5
{
    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(0, 0),
        new PlaceCard(0, 9),
        new PlaceCard(0, 12),
        new PlaceCard(1, 7),
        new PlaceCard(2, 9),
        new PlaceCard(3, 0),
        new PlaceCard(3, 11),
        new PlaceCard(5, 9),
        new PlaceCard(7, 8),
        new PlaceCard(8, 12),
        new PlaceCard(9, 1),
        new PlaceCard(9, 7),
        new PlaceCard(9, 10),
        new PlaceCard(10, 5),
        new PlaceCard(10, 12),
        new PlaceCard(11, 9),
        new PlaceCard(12, 6),
        new PlaceCard(12, 11)
    };
    static PlaceItem[] itemPlacements = new PlaceItem[] {
        new PlaceItem(0, 4),
        new PlaceItem(3, 5),
        new PlaceItem(5, 12),
        new PlaceItem(7, 9),
        new PlaceItem(8, 0),
        new PlaceItem(8, 5),
        new PlaceItem(12, 0),
        new PlaceItem(12, 12)
    };
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(1, 1),
        new PlaceEvent(1, 10),
        new PlaceEvent(2, 7),
        new PlaceEvent(4, 0),
        new PlaceEvent(4, 3),
        new PlaceEvent(4, 8),
        new PlaceEvent(4, 11),
        new PlaceEvent(5, 5),
        new PlaceEvent(7, 4),
        new PlaceEvent(7, 12),
        new PlaceEvent(8, 2),
        new PlaceEvent(10, 6),
        new PlaceEvent(10, 9),
        new PlaceEvent(11, 4),
        new PlaceEvent(12, 1),
        new PlaceEvent(12, 8)
    };
    static Placement[] obstaclePlacements = new Placement[] {
        new Placement(0, 2),
        new Placement(0, 5),
        new Placement(0, 6),
        new Placement(1, 2),
        new Placement(1, 4),
        new Placement(1, 5),
        new Placement(2, 1),
        new Placement(2, 5),
        new Placement(2, 6),
        new Placement(3, 1),
        new Placement(3, 2),
        new Placement(3, 6),
        new Placement(3, 10),
        new Placement(4, 2),
        new Placement(4, 10),
        new Placement(5, 3),
        new Placement(5, 4),
        new Placement(5, 7),
        new Placement(5, 11),
        new Placement(6, 6),
        new Placement(6, 11),
        new Placement(7, 6),
        new Placement(7, 11),
        new Placement(8, 9),
        new Placement(8, 10),
        new Placement(8, 11),
        new Placement(9, 3),
        new Placement(9, 4),
        new Placement (9, 5),
        new Placement(9, 9),
        new Placement(10, 0),
        new Placement(10, 8),
        new Placement(11,1),
        new Placement(11, 8),
        new Placement(12, 2),
        new Placement(12, 7)
    };
    static Placement exit = new Placement(2, 3);
    internal static AdditionalBoards NewBoard()
    {
        return new AdditionalBoards(cardPlacements, itemPlacements, eventPlacements, obstaclePlacements, exit);
    }
}

internal class Layout6
{
    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(0, 0),
        new PlaceCard(0, 3),
        new PlaceCard(0, 11),
        new PlaceCard(0, 5),
        new PlaceCard(0, 9),
        new PlaceCard(2, 0),
        new PlaceCard(3, 2),
        new PlaceCard(4, 8),
        new PlaceCard(4, 11),
        new PlaceCard(5,1),
        new PlaceCard(5, 11),
        new PlaceCard(7, 1),
        new PlaceCard(9, 2),
        new PlaceCard(10, 0),
        new PlaceCard(10, 12),
        new PlaceCard(11, 5),
        new PlaceCard(11, 9),
        new PlaceCard(12, 3)
    };
    static PlaceItem[] itemPlacements = new PlaceItem[] {
        new PlaceItem(1, 1),
        new PlaceItem(2, 12),
        new PlaceItem(3, 4),
        new PlaceItem(5, 6),
        new PlaceItem(7, 12),
        new PlaceItem(9, 4),
        new PlaceItem(11, 1),
        new PlaceItem(11, 6),
        new PlaceItem(12, 11)
    };
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(0, 7),
        new PlaceEvent(1, 10),
        new PlaceEvent(4, 1),
        new PlaceEvent(6, 9),
        new PlaceEvent(8, 0),
        new PlaceEvent(8, 7),
        new PlaceEvent(10, 10),
        new PlaceEvent(11, 3),
        new PlaceEvent(12, 6),
        new PlaceEvent(12, 9)
    };
    static Placement[] obstaclePlacements = new Placement[] {
        new Placement(3, 7),
        new Placement(3, 8),
        new Placement(3, 11),
        new Placement(3, 12),
        new Placement(4, 5),
        new Placement(4, 6),
        new Placement(5, 5),
        new Placement(6, 4),
        new Placement(6, 10),
        new Placement(7, 3),
        new Placement(7, 8),
        new Placement(7, 11),
        new Placement(8, 6),
        new Placement(8, 8),
        new Placement(8, 9),
        new Placement(9, 3),
        new Placement(9, 6),
        new Placement(9, 9),
        new Placement(10, 3),
        new Placement(10, 6),
        new Placement(10, 7),
        new Placement(10, 11),
        new Placement(11, 7),
        new Placement(11, 8),
        new Placement(11, 11),
        new Placement(12, 8)
    };
    static Placement exit = new Placement(9, 8);
    internal static AdditionalBoards NewBoard()
    {
        return new AdditionalBoards(cardPlacements, itemPlacements, eventPlacements, obstaclePlacements, exit);
    }
}

internal class Layout7
{
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(6, 3),
        new PlaceEvent(6, 6),
        new PlaceEvent(6, 9),

    };
    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(0, 3),
        new PlaceCard(1, 6),
        new PlaceCard(2, 1),
        new PlaceCard(2, 5),
        new PlaceCard(2, 9),
        new PlaceCard(3, 12),
        new PlaceCard(5, 0),
        new PlaceCard(5, 3),
        new PlaceCard(5, 5),
        new PlaceCard(5, 7),
        new PlaceCard(5, 10),
        new PlaceCard(7, 0),
        new PlaceCard(7, 3),
        new PlaceCard(7, 5),
        new PlaceCard(7, 7),
        new PlaceCard(7, 10),
        new PlaceCard(9, 12),
        new PlaceCard(10, 1),
        new PlaceCard(10, 5),
        new PlaceCard(10, 9),
        new PlaceCard(11, 6),
        new PlaceCard(12, 3),

    };
    static PlaceItem[] itemPlacements = new PlaceItem[]
    {
        new PlaceItem(2, 3),
        new PlaceItem(4, 10),
        new PlaceItem(6, 7),
        new PlaceItem(10, 3),
        new PlaceItem(9, 10),

    };
    static Placement[] obstaclePlacements = new Placement[] {
        new Placement(0, 0),
        new Placement(0, 6),
        new Placement(0,11),
        new Placement(1, 1),
        new Placement(1, 3),
        new Placement(1, 5),
        new Placement(1, 8),
        new Placement(1, 10),
        new Placement(2, 7),
        new Placement(3, 2),
        new Placement(3, 2),
        new Placement(3, 5),
        new Placement(3, 8),
        new Placement(3, 11),
        new Placement(9, 8),
        new Placement(4, 9),
        new Placement(5, 12),
        new Placement(6, 1),
         new Placement(6,5),
         new Placement(6,8),
         new Placement(6,10),
         new Placement(6,11),
         new Placement(6,12),
         new Placement(7,12),
         new Placement(8,9),
         new Placement(9,2),
         new Placement(9,4),
         new Placement(9,8),
         new Placement(9,11),
         new Placement(10,7),
         new Placement(11,1),
         new Placement(11,3),
         new Placement(11,5),
         new Placement(11,8),
         new Placement(11,10),
         new Placement(12,0),
         new Placement(12,6),
         new Placement(12,11),

    };
    static Placement exit = new Placement(6, 0);
    internal static AdditionalBoards NewBoard()
    {
        return new AdditionalBoards(cardPlacements, itemPlacements, eventPlacements, obstaclePlacements, exit);
    }
}