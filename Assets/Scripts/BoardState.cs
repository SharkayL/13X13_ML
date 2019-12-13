using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class BoardState {
    public GridInfo[,] lightGrids = new GridInfo[13, 13];
    public GridInfo[,] darkGrids = new GridInfo[13, 13];
    public Dictionary<int, Card> cardDatabase;
    public Dictionary<int, Item> itemDatabase = new Dictionary<int, Item>();
    public List<PlayerState> players = new List<PlayerState>();
    protected int nextCardId = 0;
    protected int nextItemId = 0;
    public bool isLight = true;
    public int currentTurn = 0;
    public PlayerState currentPlayer;
    public int maxTurns = 40;
    public GameObject cellPrefab;
    public GameObject parent;
    public MatureManager manager;
    public bool over = false;
    
    public static int defaultActionCount = 2;

    public Action<PlayerState,PlayerState,bool> playerTurnStart;
    public Action<PlayerState, Item,bool> playerGetsItem;
    public Action<PlayerState, Item,bool> playerLosesItem;
    public Action<PlayerState, Card, GridInfo, bool> playerUsedCard;
    public Action<PlayerState, PlayerState, Item, bool> playerUsedItem;
    public Action<PlayerState, Card,bool> playerGetsCard;
    public Action<PlayerState, Card,bool> playerLosesCard;
    public Action<PlayerState,bool> playerUsedAction;
    public Func<PlayerState, GridInfo, GridInfo, bool,bool> playerMoves;
    public Action<PlayerState, EventToken,bool> PlayerTriggersEve;
    public Action<int> teamWins;
    public System.Random random;
    public int randomSeed;

    public List<AdditionalBoards> usedBoards = new List<AdditionalBoards>();
    public List<AdditionalBoards> unusedBoards;
    int roundsToFlip;
    AdditionalBoards currentLayout;
    public int timesToOracle;

    public GridInfo[,] GetCurrentGrids() {
        if (isLight)
        {
            return lightGrids;
        }
        else {
            return darkGrids;
        }
    }

    public int NextCardId()
    {
        return nextCardId++;
    }

    public int NextItemId()
    {
        return nextItemId++;
    }

    #region Notifications
    public void NotifyGameover(int team,bool msg=true) {
        if (teamWins != null) {
            over = true;
            teamWins(team);
        }
    }

    public void NotifyPlayerTurnStarted(PlayerState newPlayer,PlayerState oldPlayer,bool msg)
    {
        if(playerTurnStart != null)
        {
            playerTurnStart(newPlayer, oldPlayer,msg);
        }
    }

    public void NotifyPlayerUsedCard(PlayerState player,Card card,GridInfo grid,bool msg)
    {
        if (this.playerUsedCard != null)
        {
            this.playerUsedCard(player, card, grid, msg);
        }
    }

    public void NotifyPlayerUsedAction(PlayerState player,bool msg)
    {
        if(playerUsedAction != null)
        {
            playerUsedAction(player,msg);
        }
    }

    public void NotifyPlayerMoved(PlayerState player,GridInfo from,GridInfo to,bool msg)
    {
        if(playerMoves != null)
        {
            playerMoves(player,from,to,msg);
            manager.RecoveringGrids(true);
            manager.HighlightingBasicMoves();
        }
    }

    public void NotifyItemGotten(PlayerState player, Item item,bool msg) {
        if(playerGetsItem != null)
        {
            playerGetsItem(player, item,msg);
        }
    }
    public void NotifyPlayerUsedItem(PlayerState playerBy, PlayerState targetPlayer, Item item, bool msg) {
        if (this.playerUsedItem != null) {
            playerUsedItem(playerBy, targetPlayer, item, msg);
        }
    }

    public void NotifyItemLost(PlayerState player, Item item,bool msg) {
        if (playerLosesItem != null) {
            playerLosesItem(player, item,msg);
        }
    }

    public void NotifyCardGotten(PlayerState player, Card card,bool msg) {
        if (playerGetsCard != null) {
            playerGetsCard(player, card,msg);
        }
    }

    public void NotifyCardLost(PlayerState player, Card card,bool msg) {
        if (playerLosesCard != null)
        {
            playerLosesCard(player, card,msg);
        }
    }

    public void NotifyEventHappened(PlayerState player, EventToken eve,bool msg) {
        if(PlayerTriggersEve != null)
        {
            PlayerTriggersEve(player, eve,msg);
        }
    }
    #endregion
    public GridInfo GetGrid(int col,int row)
    {
        if (!isLight) {
            return darkGrids[col, 12- row];
        }
        else {
            return lightGrids[col, 12- row];
        }
    }

    public GridInfo GetGrid(int index)
    {
        return GetGrid(index % 13, index / 13);
    }

    public GridInfo GetExit()
    {
        foreach(var grid in GetCurrentGrids())
        {
            if(grid.exit)
            {
                return grid;
            }
        }
        return null;
    }

    public GridInfo GetGrid(Placement place)
    {
        return GetGrid(place.col, place.row);
    }


    public void Init(int seed) {
        this.randomSeed = seed;
        isLight = true;
        over = false;
        players = new List<PlayerState>();
        currentTurn = 0;
        this.random = new System.Random(seed);
        roundsToFlip = random.Next(3, 6);
        timesToOracle = random.Next(2, 8);
        this.cardDatabase = new Dictionary<int, Card>();
        this.itemDatabase = new Dictionary<int, Item>();
        for (int i = 0; i < 4; ++i)
        {
            players.Add(new PlayerState(this,i));
        }
        
        currentPlayer = players[0];
        currentPlayer.actionCount = defaultActionCount;
        for (int i = 0; i < 13; ++i)
        {
            for (int j = 0; j < 13; ++j)
            {
                var light = new GridInfo(i, j);
                var dark = new GridInfo(i, j);
                lightGrids[i, j] = light;
                darkGrids[i, j] = dark;
                light.board = this;
                dark.board = this;
                manager.CellDisplay(light);
            }
        }
        LightLayout.InitLayout(this, true);
        currentLayout = null;
        unusedBoards = AdditionalBoards.Boards();
        foreach (var cell in lightGrids)
        {
            cell.InitGameObject(manager,true, isLight);
        }

        for (int i = 0; i < 4; ++i)
        {
            players[i].playerOG.GetComponent<UIPlayer>().player = players[i];
            players[i].playerSprite = manager.playerSprites[i];
            players[i].humanSprite = manager.humanSprites[i];
            players[i].ghostSprite = manager.ghostSprites[i];
            players[i].team = (int)i % 2;
            players[i].InitPlayerCards();            
        }
        this.NotifyPlayerTurnStarted(this.currentPlayer, null,false);
        manager.actionsCount.text = defaultActionCount.ToString();
    }

    public Task<int> nextTurnBlock;

    public async void NextTurn()
    {
        if(nextTurnBlock != null)
        {
            await nextTurnBlock;
        }
        nextTurnBlock = null;
        var oldPlayer = this.currentPlayer;
        oldPlayer.noItem = false;
        currentTurn += 1;
        if (currentTurn >= maxTurns) {
            //Return to mainMenu;
            return;
        }

        currentPlayer = players[currentTurn % players.Count];
        if (currentPlayer.id == 0)
        {
            roundsToFlip -= 1;
            if (currentLayout != null)
            {
                UpdatingObstacles(currentLayout);
            }
            if (isLight)
            {
                RecoverGridInfo(lightGrids);
            }
            else {
                RecoverGridInfo(darkGrids);
            }
        }
            if (aboutToFlip)
            {
                BoardFlip();
            }
        currentPlayer.actionCount = defaultActionCount;
        if (currentPlayer.lessAction) {
            currentPlayer.UseAction();
            currentPlayer.lessAction = false;
        }
        this.NotifyPlayerTurnStarted(currentPlayer, oldPlayer,true);
        
        //Vincent Add
        manager.actionsCount.text = defaultActionCount.ToString();
    }

    void UpdatingObstacles(AdditionalBoards layout) {
        if (layout.pendingObstacles == null) {
            return;
        }
        for (int i = 0; i < 2; ++i)
        {
            if (layout.pendingObstacles.Count <= 0) {
                return;
            }
            int index = random.Next(0, layout.pendingObstacles.Count);
            GridInfo info = GetGrid(layout.pendingObstacles[index]);
            //info.obstacle = true;
            //info.InitGameObject(manager, false, isLight);
            AddObstacle(info);
            layout.pendingObstacles.RemoveAt(index);    
        }
    }

    public void BoardFlip() {
        var board = PickBoard();
        currentLayout = board;
        if (board == null)
        {
            return;
        }
        isLight = !isLight;
        manager.DestroyCurrentBoard();
        foreach(var grid in this.GetCurrentGrids())
        {
            grid.Clear();
        }
        roundsToFlip = random.Next(3, 6);
        board.InitLayout(this, roundsToFlip);
        manager.BoardLayout(false);
        foreach (var player in players)
        {
            int row = player.currentCell.row;
            int column = player.currentCell.column;
            player.currentCell = GetGrid(column, 12 - row);
            player.playerOG.GetComponent<Animator>().enabled = true;
        }
        
    }

    public int CurrentPlayerNumber(int currentTurn) {
        return players.IndexOf(currentPlayer) + 1;
    }

    public PlayerState GetPlayerById(int id)
    {
        return players[id];
    }

    public Card GetCardById(int id)
    {
        return cardDatabase[id];
    }

    public Item GetItemById(int id)
    {
        return itemDatabase[id];
    }

    public PlayerState GetPlayer(int number)
    {
        return players[number - 1];
    }

    public bool gameOver
    {
        get {
            return currentPlayer.ghost && GetTeammate(currentPlayer).ghost;
        }
    }
    
    public PlayerState GetTeammate(PlayerState currentPlayer) {
        foreach(var player in players){
            if (player != currentPlayer && player.team == currentPlayer.team) {
                return player;
            }
        }
        Debug.Log("No teammate");
        return null;
    }

    public bool aboutToFlip {
        get {
            //return (int)currentTurn % 16 == 0 && currentTurn != 0;
            return (int)roundsToFlip <= 0 && currentTurn != 0;
        }
    }

    public AdditionalBoards PickBoard() {
        AdditionalBoards newBoard = SelectRandomBoard(unusedBoards);
        if(newBoard == null)
        {
            newBoard = SelectRandomBoard(usedBoards);
            if (newBoard == null) {
                //no flip
                return null;
            }
        }
        unusedBoards.Remove(newBoard);
        usedBoards.Add(newBoard);
        return newBoard;
    }

    protected AdditionalBoards SelectRandomBoard(List<AdditionalBoards> boards)
    {
        AdditionalBoards newBoard = null;
        List<AdditionalBoards> possibleBoards = new List<AdditionalBoards>(boards);
        do
        {
            if (possibleBoards.Count == 0)
            {
                break;
            }
            newBoard = possibleBoards[random.Next(0, possibleBoards.Count)];
            possibleBoards.Remove(newBoard);
        }
        while (WinnerOnExit(newBoard));
        return newBoard;
    }

    bool WinnerOnExit(AdditionalBoards board) {
        foreach (var player in players) {
            if (board.exit.row == player.currentCell.row && board.exit.col == player.currentCell.column ) {
                return true;
            }
        }
        return false;
    }

    public void AddObstacle(GridInfo grid)
    {
        grid.obstacle = true;
        grid.InitGameObject(manager, false, isLight);
    }
    public void RemoveObstacle(GridInfo grid)
    {
        grid.obstacle = false;
        var obs = grid.cell.transform.GetComponent<UIGrid>();
        GameObject.Destroy(obs.gameObject);
    }
    public void RecoverGridInfo(GridInfo[,] currentGrids)
    {
        foreach (var grid in currentGrids)
        {
            if (grid.roundsToRecoverCard == 1)
            {
                grid.containsCard = true;
                grid.InitGameObject(manager, false, isLight);
                grid.roundsToRecoverCard = 0;
            }
            if (grid.roundsToRecoverItem == 1)
            {
                grid.containsItem = true;
                grid.InitGameObject(manager, false, isLight);
                grid.roundsToRecoverItem = 0;
            }
            if (grid.roundsToRecoverCard > 1)
            {
                --grid.roundsToRecoverCard;
            }
            if (grid.roundsToRecoverItem > 1)
            {
                --grid.roundsToRecoverItem;
            }
        }

    }
}
