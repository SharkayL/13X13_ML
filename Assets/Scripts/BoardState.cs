﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardState {
    public GridInfo[,] lightGrids = new GridInfo[13, 13];
    public GridInfo[,] darkGrids = new GridInfo[13, 13];
    public bool flipped; // true is dark, false is light.
    public List<PlayerState> players = new List<PlayerState>();
    List<Item> items;
    public int currentTurn = 0;
    public PlayerState currentPlayer;
    int maxTurns = 40;
    public GameObject cellPrefab;
    public GameObject parent;
    public MatureManager manager;
    int gridSize = 70;
    public static int defaultActionCount = 2;

    public Action<PlayerState,PlayerState> playerTurnStart;
    public Action<PlayerState, Item> playerGetsItem;
    public Action<PlayerState, Item> playerLosesItem;
    public Action<PlayerState, Card> playerGetsCard;
    public Action<PlayerState, Card> playerLosesCard;
    public Func<PlayerState, GridInfo, GridInfo, bool> playerMoves;
    public Action<int> teamWins;

    public void NotifyGameover(int team) {
        if (teamWins != null) {
            teamWins(team);
        }
    }

    public void NotifyPlayerTurnStarted(PlayerState newPlayer,PlayerState oldPlayer)
    {
        if(playerTurnStart != null)
        {
            playerTurnStart(newPlayer, oldPlayer);
        }
    }

    public void NotifyPlayerMoved(PlayerState player,GridInfo from,GridInfo to)
    {
        if(playerMoves != null)
        {
            playerMoves(player,from,to);
        }
    }

    public void NotifyItemGotten(PlayerState player, Item item) {
        if(playerGetsItem != null)
        {
            playerGetsItem(player, item);
        }
    }

    public void NotifyItemLost(PlayerState player, Item item) {
        if (playerLosesItem != null) {
            playerLosesItem(player, item);
        }
    }

    public void NotifyCardGotten(PlayerState player, Card card) {
        if (playerGetsCard != null) {
            playerGetsCard(player, card);
        }
    }

    public void NotifyCardLost(PlayerState player, Card card) {
        if (playerLosesCard != null)
        {
            playerLosesCard(player, card);
        }
    }

    public GridInfo GetGrid(int col,int row)
    {
        if (flipped) {
            return darkGrids[col, 12- row];
        }
        else {
            return lightGrids[col,12- row];
        }
    }

    public GridInfo GetGrid(Placement place)
    {
        return GetGrid(place.col, place.row);
    }

    public void Init() {
        for(int i = 0; i < 4; ++i)
        {
            players.Add(new PlayerState(this));
        }
        
        currentPlayer = players[0];
        currentPlayer.actionCount = defaultActionCount;
        for (int i = 0; i < 13; ++i) {
            for (int j = 0; j < 13; ++j) {
                var light = new GridInfo(i, j);
                light.board = this;
                var dark = new GridInfo(i, j);
                lightGrids[i, j] = light;
                darkGrids[i, j] = dark;
                var obj = GameObject.Instantiate(cellPrefab);
                obj.transform.SetParent(parent.transform);
                obj.GetComponent<UIGrid>().info = light;
                light.cell = obj;
                obj.transform.localPosition = new Vector3(gridSize * i, gridSize * j, 0);
            }
        }
        LightLayout.InitLayout(this);
        for (int i = 0; i < 4; ++i)
        {
            players[i].playerSprite = manager.playerSprites[i];
            players[i].team = (int)i % 2;
            players[i].InitPlayerCards();            
        }
        this.NotifyPlayerTurnStarted(this.currentPlayer, null);

        manager.actionsCount.text = "Actions left: " + string.Format("<b>{0}</b>", defaultActionCount);
    }

    public void NextTurn()
    {
        var oldPlayer = this.currentPlayer;
        currentTurn += 1;
        if (currentTurn >= maxTurns) {
            //Return to mainMenu;
            return;
        }
        currentPlayer = players[currentTurn % players.Count];
        currentPlayer.actionCount = defaultActionCount;
        this.NotifyPlayerTurnStarted(currentPlayer, oldPlayer);
    }

    public int CurrentPlayerNumber(int currentTurn) {
        return players.IndexOf(currentPlayer) + 1;
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
}
