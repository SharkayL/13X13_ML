﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MatureManager : MonoBehaviour {
    public BoardState board;
    public GameObject cellPrefab;
    int gridSize = 70;
    public GameObject parent;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public GameObject eve;
    public GameObject card;
    public GameObject item;
    public GameObject obstacle;
    public GameObject exit;

    public GameObject itemInventory;
    public Image imgPrefab;
    public Sprite sword;
    public Sprite sampleCard;
    public GameObject cardInventory;
    public Image cardPrefab; //since card and item images will have different sizes; for test purpose, I'll still use the same sprite.
    public Transform ratioPanel;
    public Dictionary<Card, Image> displayedCards = new Dictionary<Card, Image>();
    public Dictionary<Item, Image> displayedItems = new Dictionary<Item, Image>();

    public GameObject poppedCard;
    bool destroyCard;
    float t = 0;

    public Image currentPlayer;
    public Image nextPlayer1;
    public Image nextPlayer2;
    public Image nextPlayer3;
    public List<Sprite> playerSprites;

    public Text instruction;
    public Text actionsCount;
    public Text tempInfo;
    public Text status;
    public string defaultText;
    public Text winText;

    public Button temp;
    public Button hide;
    bool h = false;
    public GameObject i;

    // Use this for initialization
    void Start () {
		board = new BoardState();       
        board.cellPrefab = cellPrefab;
        board.parent = parent;
        board.manager = this;
        defaultText = instruction.text;

        this.board.teamWins = (team) =>
        {
            //GameOver Scene: team (int) wins;
            //cant take any more actions;
            winText.text = "Team " + team + " wins!";
        };

        this.board.playerMoves = (player, from, to) =>
        {
            //Debug.Log("Player Moved");
            //animation maybe;
            return false;
        };

        this.board.playerGetsItem = (player, item) =>
        {
            if (player != board.currentPlayer) {
                return;
            }
            DisplayItem(item);
        };
        this.board.playerGetsCard = (player, card) =>
        {
            if(player != board.currentPlayer)
            {
                return;
            }
            DisplayCard(card);
        };
        this.board.playerLosesCard = (player, card) =>
        {
            Image image = null;
            displayedCards.TryGetValue(card, out image);
            if(image != null)
            {
                Destroy(image.gameObject);
                displayedCards.Remove(card);
                ArrangeCard(player, displayedCards);
            }
        };
        this.board.playerLosesItem = (player, item) =>
        {
            Image image = null;
            displayedItems.TryGetValue(item, out image);
            if (image != null) {
                Destroy(image.gameObject);
                displayedItems.Remove(item);
            }
        };
        this.board.playerTurnStart = (newPlayer, oldPlayer) =>
        {
            currentPlayer.sprite = playerSprites[board.currentTurn % board.players.Count];
            status.text = "is ghost? " + board.currentPlayer.ghost;
            nextPlayer1.sprite = playerSprites[(board.currentTurn + 1) % board.players.Count];
            nextPlayer2.sprite = playerSprites[(board.currentTurn +2) % board.players.Count];
            nextPlayer3.sprite = playerSprites[(board.currentTurn +3) % board.players.Count];
            currentPlayer.gameObject.GetComponent<UIPlayer>().player = newPlayer;
            nextPlayer1.GetComponent<UIPlayer>().player = board.players[(board.currentTurn + 1) % board.players.Count];
            nextPlayer2.GetComponent<UIPlayer>().player = board.players[(board.currentTurn + 2) % board.players.Count];
            nextPlayer3.GetComponent<UIPlayer>().player = board.players[(board.currentTurn + 3) % board.players.Count];
            if(oldPlayer != null)
            {
                UpdateDisplayedCards(newPlayer);
                UpdateDisplayedItems(newPlayer);
            }
            instruction.text = defaultText;
        };
        board.Init();

        temp.GetComponent<Button>().onClick.AddListener(() => SkipAction());
        hide.GetComponent<Button>().onClick.AddListener(() => HideInventory());
    }

    public void SkipAction() {
        //for playtesting.
        board.currentPlayer.UseAction();
    }
    public void HideInventory()
    {
        h = !h;
        i.SetActive(h);
        cardInventory.SetActive(h);
    }

    public void BoardLayout(bool start)
    {
        for (int i = 0; i < 13; ++i)
        {
            for (int j = 0; j < 13; ++j)
            {
                if (board.isLight)
                {
                    CellDisplay(board.lightGrids[i, j]);
                }
                if (!board.isLight) {
                    CellDisplay(board.darkGrids[i, j]);
                }

            }
        }
        if (board.isLight)
        {
            LightLayout.InitLayout(board, false);
            foreach (var cell in board.lightGrids)
            {
                cell.InitGameObject(this,start);
            }
        }
        if (!board.isLight)
        {
            DarkLayout.InitLayout(board);
            foreach (var cell in board.darkGrids)
            {
                cell.InitGameObject(this,start);
            }
        }
    }
    public void CellDisplay(GridInfo g)
    {
        var obj = GameObject.Instantiate(cellPrefab);
        obj.transform.SetParent(parent.transform);

            obj.GetComponent<UIGrid>().info = g;
            g.cell = obj;

   
        obj.transform.localPosition = new Vector3(gridSize * g.column, gridSize * g.row, 0);
    }
    private void Awake()
    {
        poppedCard.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void ArrangeCard(PlayerState player, Dictionary<Card, Image> displayedCards) {
        if (player.movementCards.Count > 5)
        {
            foreach (var entry in displayedCards)
            {
                var key = entry.Key;
                var obj = entry.Value;
                int space = (560 - 10 * 2 - 110) / (player.movementCards.Count - 1);
                obj.transform.localPosition = new Vector3(10 + space * player.CardIndex(key), -10);
            }
        }
        else
        {
            foreach (var entry in displayedCards) {
                var key = entry.Key;
                var obj = entry.Value;
                obj.transform.localPosition = new Vector3(10 + 110 * player.CardIndex(key), -10);
            }

        }
    }

    public void DisplayCard(Card card)
    {
        Image img = Image.Instantiate(this.board.manager.cardPrefab);
        img.sprite = this.sampleCard;
        img.GetComponent<UICard>().card = card;
        img.transform.SetParent(this.cardInventory.transform);
        img.transform.localScale = new Vector3(1, 1, 1);
        displayedCards.Add(card, img);
        ArrangeCard(board.currentPlayer, displayedCards);
        img.GetComponent<UICard>().manager = this;
    }
    public void DisplayItem(Item item) {
        Image img = Image.Instantiate(this.board.manager.imgPrefab);
        img.sprite = this.sword;
        img.transform.SetParent(this.itemInventory.transform);
        img.transform.localScale = new Vector3(1, 1, 1);
        displayedItems.Add(item, img);
        img.GetComponent<UIItem>().item = item;
        img.GetComponent<UIItem>().manager = this;
    }

    public void UpdateDisplayedCards(PlayerState currentPlayer) {
        foreach (var entry in displayedCards) {
            Destroy(entry.Value.gameObject);
        }
        displayedCards.Clear();
        foreach (var card in currentPlayer.movementCards) {
            DisplayCard(card);
        }
    }
    public void UpdateDisplayedItems(PlayerState currentPlayer) {
        foreach (var entry in displayedItems)
        {
            Destroy(entry.Value.gameObject);
        }
        displayedItems.Clear();
        foreach (var item in currentPlayer.items)
        {
            DisplayItem(item);
        }
    }

    public void MouseHoverCard(Transform card, Vector3 localPos) {
        poppedCard.transform.localPosition = localPos;
        poppedCard.transform.GetChild(0).gameObject.SetActive(true);
        Image img = poppedCard.GetComponentInChildren<Image>();
        img.sprite = this.sampleCard;
        destroyCard = true;
    }
    private void Update()
    {
        if (destroyCard) {
            t += Time.deltaTime;
            if (t >= 3) {
                poppedCard.transform.GetChild(0).gameObject.SetActive(false);
                destroyCard = false;
                t = 0;
                tempInfo.text = "Please take an action.";
            }
        }
    }
    public PlayerState DroppedOnPlayer() {
        UIPlayer[] tokens = FindObjectsOfType<UIPlayer>();
        foreach (var token in tokens)
        {
            if (token.IsDropped()) {
                return token.player;
            }
        }
        return null;
    }

    public void DestroyCurrentBoard() {

        UIGrid[] cells = FindObjectsOfType<UIGrid>();
        foreach (var cell in cells) {
            Destroy(cell.gameObject);
        }
    }
}
