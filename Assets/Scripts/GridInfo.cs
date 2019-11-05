using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridInfo
{
    private PlayerState _player;
    public GameObject cell;
    public int column;
    public int row;
    public Item item;
    public EventToken eventToken;
    public int startingPlayer = -1; //players' icons will be different.
    public bool containsEve = false;
    public bool containsCard = false; //they are movement cards.
    public bool containsItem = false;
    public bool obstacle = false;
    public bool exit = false;
    public BoardState board;

    public GridInfo(int column,int row)
    {
        this.column = column;
        this.row = row;
    }

    public PlayerState player
    {
        get
        {
            return _player;
        }
        set
        {
            if(value != null)
            {
                _player = value;
                PositionToSelf(value.playerOG);
            } else 
            _player = null;
        }
    }

    public void PositionToSelf(GameObject obj)
    {
        if (obj)
        {
            obj.transform.SetParent(cell.transform);
            obj.transform.localPosition = Vector3.zero;
        }
    }

    public GridInfo(int column,int row, Item item, EventToken eventToken, bool obstacle, bool exit)
    {
        this.column = column;
        this.row = row;
        this.item = item;
        this.eventToken = eventToken;
        this.obstacle = obstacle;
        this.exit = exit;
    }

    public GameObject InitPlayer(MatureManager manager,int number)
    {
        var player = board.GetPlayer(this.startingPlayer);
        player.currentCell = this;
        GameObject obj = null;
        switch (number) {
            case 1:
                obj = GameObject.Instantiate(manager.player1);
                break;
            case 2:
                obj = GameObject.Instantiate(manager.player2);
                break;
            case 3:
                obj = GameObject.Instantiate(manager.player3);
                break;
            case 4:
                obj = GameObject.Instantiate(manager.player4);
                break;
        }
        
        player.playerOG = obj;
        return obj;
    }

    public void InitGameObject(MatureManager manager,bool start)
    {
        GameObject obj = null;
        if (start && this.startingPlayer != -1) {
            obj = InitPlayer(manager, this.startingPlayer);
        }
        if (this.containsCard) {
            obj = GameObject.Instantiate(manager.card);
        }
        if (this.containsEve) {
            obj = GameObject.Instantiate(manager.eve);
        }
        if (this.containsItem) {
            obj = GameObject.Instantiate(manager.item);
        }
        if (this.obstacle) {
            obj = GameObject.Instantiate(manager.obstacle);
        }
        if (this.exit) {
            obj = GameObject.Instantiate(manager.exit);
        }
        PositionToSelf(obj);

    }
}
