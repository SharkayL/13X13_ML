using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAcademy : Academy
{
    public BoardState board;
    MatureManager manager;
    List<PlayerState> availablePlayers;
    Dictionary<int, PlayerAgent> agents = new Dictionary<int, PlayerAgent>();

    public override void InitializeAcademy()
    {
        manager = GetComponent<MatureManager>();
        manager.Start();
        board = manager.board;
        availablePlayers = new List<PlayerState>(board.players);
    }

    public PlayerState GetSlot(PlayerAgent agent)
    {
        var entry = availablePlayers[0];
        availablePlayers.RemoveAt(0);
        agents.Add(entry.id, agent);
        return entry;
    }

    public override void AcademyReset()
    {
        manager.Reset();
        var players = board.players;
        foreach(var player in players)
        {
            agents[player.id].player = player;
        }
    }

    public override void AcademyStep()
    {
        if(board.currentTurn == board.maxTurns)
        {
            AcademyDone();
            return;
        }
        var playerId = board.currentPlayer.id;
        var agent = agents[playerId];
        agent.RequestDecision();
    }

    public void AcademyDone()
    {
        foreach(var entry in agents)
        {
            entry.Value.Done();
        }
        AcademyReset();
    }
}
