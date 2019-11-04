using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState
{
    private GridInfo _currentCell;
    public List<Item> items = new List<Item>();
    public List<Card> movementCards = new List<Card>();
    public Card playingCard;
    public BoardState board;
    public GameObject playerOG;
    public Sprite playerSprite;
    public int actionCount;
    public int itemsCount;
    public int maxItems;
    int initCards = 5;
    public int team;

    public PlayerState(BoardState board)
    {
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
                this.board.NotifyPlayerMoved(this, oldCell, value);

                if (!this.ghost)
                {
                    if (value.containsItem)
                    {
                        if (this.items.Count < 7)
                            AddRandomItem();
                    }
                    if (value.containsCard)
                    {
                        AddRandomCard();
                    }
                    //if (value.exit)
                    //{
                    //    board.NotifyGameover(this.team);
                    //}
                }
                if (this.ghost) {
                    if (board.gameOver) {
                        int winnerTeam = Math.Abs(this.team - 1);
                        board.NotifyGameover(winnerTeam);
                    }
                }
            }
        }
    }

    public void InitPlayerCards() {
        for (int i = 0; i < initCards; ++i) {
            Card card = Card.PickRandomObj();
            AddCard(card);
        }
    } 

    public void UseAction()
    {
        --actionCount;
        board.manager.actionsCount.text = "Actions left: " + string.Format("<b>{0}</b>", actionCount);
        if (actionCount <= 0)
        {
            board.NextTurn();
        }
    }
    
    public Card PickRandomCard()
    {
        int index = (int)Math.Round((float)UnityEngine.Random.Range(1, this.movementCards.Count));
        var card = movementCards[index - 1];
        //this.playingCard = card;
        //Debug.Log(card.GetType().Name);
        return card;
    }

    public Item PickRandomItem()
    {
        int index = (int)Math.Round((float)UnityEngine.Random.Range(1, this.items.Count));
        var item = items[index - 1];
        return item;
    }

    public void AddRandomCard() {
        this.AddCard(Card.PickRandomObj());
    }

    public void AddCard(Card card) {
        card.heldBy = this;
        card.board = this.board;
        movementCards.Add(card);
        board.NotifyCardGotten(this, card);
        //Debug.Log(card.GetType().Name);
    }
        
    public int CardIndex(Card card)
    {
        return this.movementCards.IndexOf(card);
    }

    public void AddRandomItem() {
        this.AddItem(Item.PickRandomObj());
    }

    public void AddItem(Item item)
    {
        if (board.manager)
        {
            //var obj = item.DisplayItem(this.board.manager);
            //this.board.manager.itemAdded(obj, this.board.manager.itemInventory);            
        }
        this.board.NotifyItemGotten(this, item);
        
        items.Add(item);
        Debug.Log(item.GetType().Name);
    }

    public void DiscardCard(Card card) {
        movementCards.Remove(card);
        board.NotifyCardLost(this, card);
    }
    public void DiscardItem(Item item) {
        items.Remove(item);
        board.NotifyItemLost(this, item);
    }

    public bool ghost {
        get { return this.movementCards.Count<=0;  }
    }
}
