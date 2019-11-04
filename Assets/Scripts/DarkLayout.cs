using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


public class DarkLayout
{
       // events = red
    static PlaceEvent[] eventPlacements = new PlaceEvent[] {    
        new PlaceEvent(6, 3),
        new PlaceEvent(6, 6),
        new PlaceEvent(6, 9),
     
    };
    //card = yellow

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
          //green = item
    static PlaceItem[] itemPlacements = new PlaceItem[]
    {
        new PlaceItem(2, 3),
        new PlaceItem(4, 10),
        new PlaceItem(6, 0),
        new PlaceItem(6, 7),
        new PlaceItem(10, 3),
        new PlaceItem(9, 10),
        
    };
    // obstacle = X
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

    public static void InitLayout(BoardState board)
    {
        //foreach (var player in playerPlacements)
        //{
        //    board.GetGrid(player).startingPlayer = player.player;
        //}
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

