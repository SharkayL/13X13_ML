using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// obstacles and exits being marked in GirdInfo
public class Placement {
    public readonly int row;
    public readonly int col;

    public Placement(int row,int col)
    {
        this.row = row;
        this.col = col;
    }
}

class PlacePlayer : Placement
{
    public readonly int player;
    public PlacePlayer(int row, int col, int player): base(row, col)
    {
        this.player = player;
    }
}

class PlaceItem : Placement {

    public PlaceItem(int row, int col) : base(row, col) {
    }
}

class PlaceCard : Placement {
    public PlaceCard(int row, int col) : base(row, col) {
    }
}

class PlaceEvent : Placement
{
    public PlaceEvent(int row, int col) : base(row, col)
    {
    }
}
