using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum GameStart
{
    single,
    server,
    client
}

public class MatureManager : MonoBehaviour {
    #region Visual objects
    public BoardState board;
    public GameObject cellPrefab;
    int gridSize = 70;
    public GameObject parent;

    [Header("Players")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public GameObject ghost;

    public List<Sprite> ghostSprites;
    public List<Sprite> humanSprites;

    [Header("BoardElements")]
    public GameObject eve;
    public GameObject card;
    public GameObject item;
    public GameObject obstacle;
    public GameObject exit;

    public GameObject darkEve;
    public GameObject darkCard;
    public GameObject darkItem;
    public GameObject darkObstacle;

    public GameObject background;

    [Header("Panels")]
    public GameObject itemInventory;
    public Image imgPrefab;
    public GameObject cardInventory;
    public Image cardPrefab; //since card and item images will have different sizes; for test purpose, I'll still use the same sprite.
    public Transform ratioPanel;
    public Dictionary<Card, Image> displayedCards = new Dictionary<Card, Image>();
    public Dictionary<Item, Image> displayedItems = new Dictionary<Item, Image>();

    public GameObject poppedCard;
    bool destroyCard;
    float t = 0;

    [Header("Avatars")]
    public Image currentPlayer;
    public Image nextPlayer1;
    public Image nextPlayer2;
    public Image nextPlayer3;
    public List<Sprite> playerSprites;

    [Header("UI_Info")]
    public Text instruction;
    public Text actionsCount;
    public Text tempInfo;
    public string defaultText;
    public Text winText;
    public GameObject winPanel;
    public Text eveText;

    public Button temp;
    public Button hide;
    bool h = false;
    public GameObject i;

    [Header("Items")]
    public Sprite magnetRed;
    public Sprite magnetBlue;
    public Sprite longArm;
    public Sprite longMug;
    public Sprite teleBlade;
    public Text itemName;
    public Text itemDescrib;

    [Header("Events")]
    public Sprite add2C;
    public Sprite add1I;
    public Sprite add1B;
    public Sprite decM;
    public Sprite ban1I;
    public Sprite los1C;
    public Sprite beR;
    public SpriteRenderer eveImg;

    [Header("Cards")]
    public Sprite dia1;
    public Sprite adj2;
    public Sprite dia2;
    public Sprite cro1;
    public Sprite str3;

    public Sprite whiteCell;
    public Sprite greenCell;
    #endregion 
    public List<GridInfo> highlightedGrids = new List<GridInfo>();
    protected GameClient client;

    public static GameStart startType = GameStart.server;
    public static string serverIp = "127.0.0.1";
    public static int playerID = -1;
    bool started = false;
    public int displayPlayerId;

    // Use this for initialization
    public void Start () {
        if(started)
        {
            return;
        }
        started = true;
        poppedCard.transform.GetChild(0).gameObject.SetActive(false);
        board = new BoardState();
        this.displayPlayerId = MatureManager.playerID;
        board.cellPrefab = cellPrefab;
        board.parent = parent;
        board.manager = this;
        defaultText = instruction.text;

        this.board.teamWins = (team) =>
        {
            //GameOver Scene: team (int) wins;
            //cant take any more actions;
            winPanel.SetActive(true);
            winText.text = "Team " + team + " wins!";
        };

        this.board.playerMoves = (player, from, to,msg) =>
        {
            if(client != null && msg)
            {
                client.SendPlayerMoved(player, to);
            }
            //Debug.Log("Player Moved");
            //animation maybe;
            return false;
        };

        this.board.playerUsedCard = (player, card, grid, msg) =>
        {
            if(client != null && msg)
            {
                client.SendPlayerUsedCard(player, card, grid);
            }

        };
        this.board.playerUsedItem = (player, target, item, msg) => {
            if(client != null && msg)
            {
                client.SendPlayerUsedItem(player, target, item);
            }
        };
        this.board.playerGetsItem = (player, item,msg) =>
        {
            if (player != board.currentPlayer) {
                return;
            }
            DisplayItem(player,item);
        };
        this.board.playerGetsCard = (player, card,msg) =>
        {
            if(player != board.currentPlayer)
            {
                return;
            }
            DisplayCard(player,card);
        };
        this.board.playerLosesCard = (player, card, msg) =>
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
        this.board.playerLosesItem = (player, item, msg) =>
        {
            Image image = null;
            displayedItems.TryGetValue(item, out image);
            if (image != null) {
                Destroy(image.gameObject);
                displayedItems.Remove(item);
            }
        };
        this.board.PlayerTriggersEve = (player, eve,msg) => {
            //Debug.Log(eve.GetType().Name);
            DisplayEve(eve);          
            eveText.text = "Player"+(player.id +1) + " trigered Event: " + eve.GetType().Name;
        };
        this.board.playerUsedAction = (player,msg) =>
        {
            if (client != null && msg)
            {
                client.SendPlayerUsedAction(player);
            }
        };

        this.board.playerTurnStart = (newPlayer, oldPlayer,msg) =>
        {
            //newPlayer.playerOG.GetComponent<Animator>().SetBool("isTurn",true);
            currentPlayer.sprite = playerSprites[board.currentTurn % board.players.Count];
            //status.text = "is ghost? " + board.currentPlayer.ghost;            
            nextPlayer1.sprite = playerSprites[(board.currentTurn + 1) % board.players.Count];
            nextPlayer2.sprite = playerSprites[(board.currentTurn +2) % board.players.Count];
            nextPlayer3.sprite = playerSprites[(board.currentTurn +3) % board.players.Count];
            currentPlayer.gameObject.GetComponent<UIPlayer>().player = newPlayer;

            UpdateAnimation(newPlayer, newPlayer.ghost, true);


            if (board.currentPlayer.ghost)
            {
                ghost.GetComponent<Image>().sprite = board.currentPlayer.ghostSprite;
                ghost.SetActive(board.currentPlayer.ghost);
            }
            
            nextPlayer1.GetComponent<UIPlayer>().player = PlayerInfoChanged(1);
            nextPlayer2.GetComponent<UIPlayer>().player = PlayerInfoChanged(2);
            nextPlayer3.GetComponent<UIPlayer>().player = PlayerInfoChanged(3);
            UpdatePlayerInfo();
            
            if (oldPlayer != null)
            {
                if(displayPlayerId == -1)
                {
                    UpdateDisplayedCards(newPlayer);
                    UpdateDisplayedItems(newPlayer);
                }
            }
            else
            {
                if (displayPlayerId != -1)
                {
                    var player = board.GetPlayerById(displayPlayerId);
                    UpdateDisplayedCards(player);
                    UpdateDisplayedItems(player);
                }
            }
            instruction.text = defaultText;
            itemName.text = null;
            itemDescrib.text = null;
        };
        //board.Init();
        temp.GetComponent<Button>().onClick.AddListener(() => SkipAction());
        hide.GetComponent<Button>().onClick.AddListener(() => HideInventory());
        if(MatureManager.startType == GameStart.server)
        {
            StartServer();
        }
        else if(MatureManager.startType == GameStart.client)
        {
            JoinServer(MatureManager.serverIp);
        }
    }

    public void Reset()
    {
        foreach(var player in board.players)
        {
            player.CleanUp();
        }
        foreach(var grid in board.GetCurrentGrids())
        {
            grid.CleanUp();
        }
        foreach(var entry in displayedCards)
        {
            GameObject.Destroy(entry.Value.gameObject);
        }
        displayedCards.Clear();
        foreach (var entry in displayedItems)
        {
            GameObject.Destroy(entry.Value.gameObject);
        }
        displayedItems.Clear();
        board.Init(new System.Random().Next());
    }

    public void PullMessages()
    {
        while(this.client.pending.Count != 0)
        {
            var message = this.client.pending.Take();
            if(message is PlayerUsedCard)
            {
                var playedCard = (PlayerUsedCard)message;
                var player = board.GetPlayerById(playedCard.playerNo);
                var gridlet = board.GetGrid(playedCard.col, playedCard.row);
                var card = board.GetCardById(playedCard.cardId);
                card.move(player, gridlet,false);
            }
            else if(message is PlayerMoved)
            {
                var playerMoved = (PlayerMoved)message;
                var player = board.GetPlayerById(playerMoved.playerNo);
                var gridlet = board.GetGrid(playerMoved.col,playerMoved.row);
                //player.currentCell = gridlet;
                player.Move(gridlet, false);
            }
            else if(message is PlayerUsedAction)
            {
                var playerMsg = (PlayerUsedAction)message;
                board.GetPlayerById(playerMsg.playerNo).UseAction(false);
            }
            else if (message is PlayerUsedItem) {
                var usedItem = (PlayerUsedItem)message;
                var player = board.GetPlayerById(usedItem.playerId);
                var target = board.GetPlayerById(usedItem.targetId);
                var item = board.GetItemById(usedItem.itemId);
                item.play(player, target, false);
            }
        }
    }

    public void StartServer()
    {
        board.Init(new System.Random().Next());
        var server = new Server(board);
        server.Start();
        this.client = server;
    }

    public async void JoinServer(string ip)
    {
        var client = new Client();
        await client.JoinGame(ip,this.board);
        this.client = client;
    }

    public void SkipAction() {
        //for playtesting.
        board.currentPlayer.UseAction(true);
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
        background.transform.GetChild(0).gameObject.SetActive(board.isLight);
        background.transform.GetChild(1).gameObject.SetActive(!board.isLight);
        if (board.isLight)
        {
            LightLayout.InitLayout(board, false);
            foreach (var cell in board.lightGrids)
            {
                cell.InitGameObject(this,start, board.isLight);
            }
        }
        if (!board.isLight)
        {
            DarkLayout.InitLayout(board);
            foreach (var cell in board.darkGrids)
            {
                cell.InitGameObject(this,start, board.isLight);
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
    public void ArrangeCard(PlayerState player, Dictionary<Card, Image> displayedCards) {
        if (displayPlayerId != -1 && displayPlayerId != player.id)
        {
            return;
        }
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

    public PlayerState TargetPlayer()
    {
        if(displayPlayerId == -1)
        {
            return board.currentPlayer;
        }
        else
        {
            return board.GetPlayerById(displayPlayerId);
        }
    }

    public void DisplayCard(PlayerState player,Card card)
    {
        if(displayPlayerId != -1 && displayPlayerId != player.id)
        {
            return;
        } 
        Image img = Image.Instantiate(this.board.manager.cardPrefab);
        img.sprite = this.GetCardSprite(card);
        img.GetComponent<UICard>().card = card;
        img.transform.SetParent(this.cardInventory.transform);
        img.transform.localScale = new Vector3(1, 1, 1);
        displayedCards.Add(card, img);
        ArrangeCard(player, displayedCards);
        img.GetComponent<UICard>().manager = this;
    }
    public void DisplayItem(PlayerState player,Item item) {
        if (displayPlayerId != -1 && displayPlayerId != player.id)
        {
            return;
        }
        Image img = Image.Instantiate(this.board.manager.imgPrefab);
        img.sprite = GetItemSprite(item);
        img.transform.SetParent(this.itemInventory.transform);
        img.transform.localScale = new Vector3(1, 1, 1);
        displayedItems.Add(item, img);
        img.GetComponent<UIItem>().item = item;
        img.GetComponent<UIItem>().manager = this;
    }

    public void UpdateDisplayedCards(PlayerState currentPlayer) {
        foreach (var entry in displayedCards) {
            try
            {
                Destroy(entry.Value.gameObject);
            }
            catch(Exception err)
            {

            }
        }
        displayedCards.Clear();
        foreach (var card in currentPlayer.movementCards) {
            DisplayCard(currentPlayer,card);
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
            DisplayItem(currentPlayer,item);
        }
    }

    //Animation

    public void UpdateAnimation(PlayerState currentPlayer,bool isGhost, bool isTurn)
    {
        currentPlayer.playerOG.GetComponent<Animator>().SetBool("isGhost",isGhost);
        currentPlayer.playerOG.GetComponent<Animator>().SetBool("isTurn", isTurn);
    }


    public void MouseHoverCard(Transform card, Vector3 localPos, Card c) {
        poppedCard.transform.localPosition = localPos;
        GameObject popC = poppedCard.transform.GetChild(0).gameObject;
        popC.SetActive(true);
        Image img = poppedCard.GetComponentInChildren<Image>();
        img.sprite = GetCardSprite(c);
        popC.transform.GetChild(0).GetComponent<Text>().text = c.getName();
        popC.transform.GetChild(1).GetComponent<Text>().text = c.getDescription();
        destroyCard = true;
    }
    private void Update()
    {
        if (this.client != null)
        {
            this.PullMessages();
        }
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

    public void DisplayEve(EventToken eve) {
        var img = GameObject.Instantiate(this.board.manager.eveImg);
        img.sprite = GetEveSprite(eve);

        img.transform.SetParent(this.board.currentPlayer.playerOG.transform);
        img.transform.localPosition = new Vector3(0,0.5f,0);
        img.transform.localScale = new Vector3(1, 1, 1);
        img.GetComponent<EveNotification>().manager = this;
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

    public Sprite GetItemSprite(Item item)
    {
        if (item is MagnetRed) return magnetRed;
        if (item is MagnetBlue) return magnetBlue;
        if (item is LongArm) return longArm;
        if (item is LongMug) return longMug;
        if (item is Blade) return teleBlade;

        return null;
    }
    public Sprite GetEveSprite(EventToken eve) {
        if (eve is Adding2Cards) return add2C;
        if (eve is Adding1Item) return add1I;
        if (eve is Adding1Both) return add1B;
        if (eve is DecreasingMoves) return decM;
        if (eve is BanninngItems) return ban1I;
        if (eve is Losing1Card) return los1C;
        if (eve is BecomingRegular) return beR;
        return null;
    }
    public Sprite GetCardSprite(Card card) {
        if (card is DiagonalBy1) return dia1;
        if (card is AdjacentBy2) return adj2;
        if (card is DiagonalBy2) return dia2;
        if (card is Cross1Obstacle) return cro1;
        if (card is StraightBy3) return str3;
        return null;
    }

    public void HighlightingPossibilities(Card card) {
        foreach(GridInfo grid in board.GetCurrentGrids())
        {
            if (card.canMove(TargetPlayer(), grid)) {
                SpriteRenderer renderer = grid.cell.GetComponent<SpriteRenderer>();
                renderer.sprite = greenCell;
                Color c = renderer.color;
                c.a = 1;
                renderer.color = c;
                highlightedGrids.Add(grid);
            }
        }
    }

    public void RecoveringGrids() {
        foreach (GridInfo grid in highlightedGrids) {
            SpriteRenderer renderer = grid.cell.GetComponent<SpriteRenderer>();
            renderer.sprite = whiteCell;
            Color c = renderer.color;
            c.a = 60/255f;
            renderer.color = c;
        }
        highlightedGrids.Clear();
    }

    public PlayerState PlayerInfoChanged(int i)
    {
        PlayerState playerInfo = board.players[(board.currentTurn + i) % board.players.Count];
        return playerInfo;
    }
    public void UpdatePlayerInfo() {
        nextPlayer1.GetComponentInChildren<Text>().text = string.Format("<b>{0}</b>{1}\n<b>{2}</b>{3}", "Cards: ", PlayerInfoChanged(1).movementCards.Count, "Items: ", PlayerInfoChanged(1).items.Count);
        nextPlayer2.GetComponentInChildren<Text>().text = string.Format("<b>{0}</b>{1}\n<b>{2}</b>{3}", "Cards: ", PlayerInfoChanged(2).movementCards.Count, "Items: ", PlayerInfoChanged(2).items.Count);
        nextPlayer3.GetComponentInChildren<Text>().text = string.Format("<b>{0}</b>{1}\n<b>{2}</b>{3}", "Cards: ", PlayerInfoChanged(3).movementCards.Count, "Items: ", PlayerInfoChanged(3).items.Count);
    }
}
