using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public abstract class MSG 
{
    public int id;
    public int clientId;
    public MSG(int clientId, int id) {
        this.clientId = clientId;
        this.id = id;
    }
}

[System.Serializable]
public class PlayerUsedAction : MSG
{
    public int playerNo;
    public PlayerUsedAction(int clientId,int id,PlayerState player) : base(clientId,id)
    {
        this.playerNo = player.id;
    }
}

[System.Serializable]
public class PlayerMoved : MSG {

    public int playerNo;
    public int row;
    public int col;
    public PlayerMoved(int clientId, int id, PlayerState player, GridInfo grid) : base(clientId, id) {
        this.playerNo = player.id;
        this.row = 12-grid.row;
        this.col = grid.column;
    }    
}
[System.Serializable]
public class PlayerUsedCard : MSG
{
    public int cardId;
    public PlayerUsedCard(int clientId, int id, PlayerState player, Card card) : base(clientId, id)
    {
        this.cardId = card.id;
    }
}
[System.Serializable]
public class PlayerUsedItem : MSG {
    public int itemId;
    public int playerId;
    public int targetId;
    public PlayerUsedItem(int clientId, int id, PlayerState player, Item item, PlayerState target): base(clientId, id)
    {
        this.itemId = item.id;
        this.playerId = player.id;
        this.targetId = target.id;
    }
}

[System.Serializable]
public class PlayerTurnStarts : MSG {
    public int playerId;
    public PlayerTurnStarts(int clientId, int id, PlayerState player) : base(clientId, id) {
        this.playerId = player.id;
    }
}
[System.Serializable] 
public class PlayerSkipsAction : MSG
{
    public int playerId;
    public PlayerSkipsAction(int clientId, int id, PlayerState player) : base(clientId, id)
    {
        this.playerId = player.id;
    }
}