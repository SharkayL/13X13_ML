using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState
{
    public int id;
    private GridInfo _currentCell;
    public List<Item> items = new List<Item>();
    public List<Card> movementCards = new List<Card>();
    public Card playingCard;
    public BoardState board;
    public GameObject playerOG;
    public Sprite playerSprite;
    public Sprite humanSprite;
    public Sprite ghostSprite;
    public int actionCount;
    public int itemsCount;
    public int maxItems;
    int initCards = 5;
    public int team;

    public bool lessAction = false;
    public bool noItem = false;

    public PlayerState(BoardState board,int id)
    {
        this.id = id;
        this.board = board;
    }

    public int row
    {
        get { return _currentCell.row; }
    }

    public int col
    {
        get { return _currentCell.column; }
    }

    public GridInfo currentCell
    {
        get
        {
            return _currentCell;
        }
        set
        {
            if(_currentCell != value)
            {
                if(_currentCell != null) 
                {
                    _currentCell.player = null;
                }
                value.player = this;
                var oldCell = _currentCell;
                _currentCell = value;
                if (value.containsEve)
                {
                    TriggerRandomEvent();
                }
                if (!this.ghost)
                {
                    if (value.containsItem)
                    {
                        if (this.items.Count < 5)
                            AddRandomItem();
                    }
                    if (value.containsCard)
                    {
                        AddRandomCard();
                    }
                }
                
            }
        }
    }

    public void InitPlayerCards() {
        for (int i = 0; i < initCards; ++i) {
            Card card = Card.PickRandomObj(this.board);
            AddCard(card);
        }
    } 

    public IEnumerable<GridInfo> GetAdjacentGrids()
    {
        if(this.row > 0)
        {
            yield return board.GetGrid(this.col, this.row - 1);
        }
        if(this.row < 12)
        {
            yield return board.GetGrid(this.col, this.row + 1);
        }
        if(this.col > 0)
        {
            yield return board.GetGrid(this.col-1, this.row);
        }
        if (this.col < 12)
        {
            yield return board.GetGrid(this.col + 1, this.row);
        }
    }

    public void Move(GridInfo grid,bool msg = false)
    {
        var oldCell = this.currentCell;
        this.currentCell = grid;
        this.board.NotifyPlayerMoved(this, oldCell, grid, msg);
        if (grid.exit)
        {
            board.NotifyGameover(this.team);
        }
    }

    public void UseAction(bool msg = false)
    {
        --actionCount;
        //board.manager.actionsCount.text = "Actions left: " + string.Format("<b>{0}</b>", actionCount);
        //Vicent modify
        board.NotifyPlayerUsedAction(this,msg);
        board.manager.actionsCount.text = actionCount.ToString();
        //Animation
        board.manager.UpdateAnimation(this,ghost,actionCount>0);

        if (actionCount <= 0)
        {
            board.manager.ghost.SetActive(false);
            board.NextTurn();
        }
        if (ghost) {
            playerOG.GetComponent<SpriteRenderer>().sprite = ghostSprite;
        }
        else if (ghost)
        {
            board.manager.ghost.GetComponent<Image>().sprite = ghostSprite;
            board.manager.ghost.SetActive(ghost);
        }
    }
    
    public Card PickRandomCard()
    {
        //int index = (int)Math.Round((float)UnityEngine.Random.Range(1, this.movementCards.Count));
        var index = board.random.Next(0, this.movementCards.Count);
        var card = movementCards[index];
        //Debug.Log(card.GetType().Name);
        return card;
    }

    public Item PickRandomItem()
    {
        //int index = (int)Math.Round((float)UnityEngine.Random.Range(1, this.items.Count));
        var index = board.random.Next(0, this.items.Count);
        var item = items[index];
        return item;
    }

    public void AddRandomCard(bool msg=false) {
        this.AddCard(Card.PickRandomObj(this.board),msg);
    }

    public void AddCard(Card card,bool msg=false) {
        card.heldBy = this;
        card.board = this.board;
        movementCards.Add(card);
        board.NotifyCardGotten(this, card,msg);
        //Debug.Log(card.GetType().Name);
    }
        
    public int CardIndex(Card card)
    {
        return this.movementCards.IndexOf(card);
    }

    public void AddRandomItem(bool msg=false) {
        this.AddItem(Item.PickRandomObj(this.board),msg);
    }

    public void AddItem(Item item,bool msg=false)
    {
        if (board.manager)
        {
            //var obj = item.DisplayItem(this.board.manager);
            //this.board.manager.itemAdded(obj, this.board.manager.itemInventory);            
        }
        this.board.NotifyItemGotten(this, item,msg);
        
        items.Add(item);
        Debug.Log(item.GetType().Name);
    }

    public void TriggerRandomEvent(bool msg=false)
    {
        while (true)
        {
            EventToken eve = EventToken.Init(EventToken.PickRandom(this.board));
            if (eve.canEffect(this))
            {
                board.NotifyEventHappened(this, eve, msg);
                return;
            }
        }
    }

    public void DiscardCard(Card card,bool msg=false) {
        if(playingCard == card)
        {
            playingCard = null;
        }
        movementCards.Remove(card);
        board.NotifyCardLost(this, card,msg);
    }
    public void DiscardItem(Item item,bool msg=false) {
        items.Remove(item);
        board.NotifyItemLost(this, item,msg);
    }

    public bool ghost {
        get { Debug.Log("Ghost"); return this.movementCards.Count<=0;}

    }

}
