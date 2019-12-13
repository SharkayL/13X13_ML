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
    public int startingPlayer = -1; //players' icons will be different.
    public bool containsEve = false;
    public bool containsCard = false; //they are movement cards.
    public bool containsItem = false;
    public bool obstacle = false;
    public bool exit = false;
    public BoardState board;
    public int roundsToRecoverItem = 0;
    public int roundsToRecoverCard = 0;

    public GridInfo(int column,int row)
    {
        this.column = column;
        this.row = row;
    }

    public void Clear()
    {
        this.containsEve = false;
        this.containsCard = false;
        this.containsItem = false;
        this.obstacle = false;
        this.exit = false;
    }

    public int index
    {
        get
        {
            return row * 13 + column;
        }
    }

    public float Distance(GridInfo other)
    {
        return (new Vector2(this.column, this.row) - new Vector2(other.column, other.row)).magnitude;
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

    public void CleanUp()
    {
        if(this.cell != null)
        {
            GameObject.Destroy(this.cell.gameObject);
        }
    }

    public void InitGameObject(MatureManager manager,bool start, bool isLight)
    {
        GameObject obj = null;
        if (start && this.startingPlayer != -1) {
            obj = InitPlayer(manager, this.startingPlayer);
        }
        if (this.containsCard) {
            if (isLight)
            {
                obj = GameObject.Instantiate(manager.card);
            }
            else obj = GameObject.Instantiate(manager.darkCard);
        }
        if (this.containsEve) {
            if (isLight)
            {
                obj = GameObject.Instantiate(manager.eve);
            }
            else obj = GameObject.Instantiate(manager.darkEve);
        }
        if (this.containsItem) {
            if (isLight)
            {
                obj = GameObject.Instantiate(manager.item);
            }
            else obj = GameObject.Instantiate(manager.darkItem);
        }
        if (this.obstacle) {
            if (isLight)
            {
                obj = GameObject.Instantiate(manager.obstacle);
            }
            else obj = GameObject.Instantiate(manager.darkObstacle);
        }
        if (this.exit) {
            obj = GameObject.Instantiate(manager.exit);
        }
        PositionToSelf(obj);

    }

    public void KillChild()
    {
        GameObject.Destroy(this.cell.transform.GetChild(0).gameObject);
    }
}
