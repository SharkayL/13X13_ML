using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


public class LightLayout
{
    static PlacePlayer[] playerPlacements = new PlacePlayer[]
    {
        new PlacePlayer(0,0,1),
        new PlacePlayer(10,0,2),
        new PlacePlayer(2,0,3),
        new PlacePlayer(12,0,4)
    };

    static PlaceEvent[] eventPlacements = new PlaceEvent[] {
        new PlaceEvent(3, 3),
        new PlaceEvent(6, 6),
        new PlaceEvent(6, 10),
        new PlaceEvent(3, 9),
        new PlaceEvent(9, 3),
        new PlaceEvent(9, 9)
    };

    static PlaceCard[] cardPlacements = new PlaceCard[] {
        new PlaceCard(0, 2),
        new PlaceCard(1, 3),
        new PlaceCard(1, 8),
        new PlaceCard(2, 2),
        new PlaceCard(2, 4),
        new PlaceCard(2, 10),
        new PlaceCard(4, 1),
        new PlaceCard(4, 3),
        new PlaceCard(4, 5),
        new PlaceCard(4, 7),
        new PlaceCard(4, 12),
        new PlaceCard(5, 0),
        new PlaceCard(5, 2),
        new PlaceCard(5, 6),
        new PlaceCard(6, 8),
        new PlaceCard(7, 0),
        new PlaceCard(7, 2),
        new PlaceCard(7, 6),
        new PlaceCard(8, 1),
        new PlaceCard(8, 3),
        new PlaceCard(8, 5),
        new PlaceCard(8, 7),
        new PlaceCard(8, 12),
        new PlaceCard(10, 2),
        new PlaceCard(10, 4),
        new PlaceCard(10, 10),
        new PlaceCard(11, 3),
        new PlaceCard(11, 8),
        new PlaceCard(12, 2)
    };

    static PlaceItem[] itemPlacements = new PlaceItem[]
    {
        new PlaceItem(0, 3),
        new PlaceItem(0, 7),
        new PlaceItem(0, 12),
        new PlaceItem(1, 10),
        new PlaceItem(2, 6),
        new PlaceItem(2, 8),
        new PlaceItem(3, 11),
        new PlaceItem(4, 2),
        new PlaceItem(4, 10),
        new PlaceItem(6, 3),
        new PlaceItem(6, 4),
        new PlaceItem(8, 2),
        new PlaceItem(8, 10),
        new PlaceItem(9, 11),
        new PlaceItem(10, 6),
        new PlaceItem(10, 8),
        new PlaceItem(11, 10),
        new PlaceItem(12, 3),
        new PlaceItem(12, 7),
        new PlaceItem(12, 12)
    };
    static Placement[] obstaclePlacements = new Placement[] {
        new Placement(0, 4),
        new Placement(1, 4),
        new Placement(2, 5),
        new Placement(3, 6),
        new Placement(3, 8),
        new Placement(4, 4),
        new Placement(4, 9),
        new Placement(5, 10),
        new Placement(6, 0),
        new Placement(6, 1),
        new Placement(7, 10),
        new Placement(8, 4),
        new Placement(8, 9),
        new Placement(9, 6),
        new Placement(9, 8),
        new Placement(10, 5),
        new Placement(11, 4),
        new Placement(12, 4)
    };

    static Placement exit = new Placement(6, 12);

    public static void InitLayout(BoardState board)
    {
        foreach (var player in playerPlacements)
        {
            board.GetGrid(player).startingPlayer = player.player;
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

        board.GetGrid(exit).exit = true;
    }
};

