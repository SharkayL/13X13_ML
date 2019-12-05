using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Threading.Tasks;

public class PlayerAcademy : Academy
{
    public BoardState board;
    public bool slowPlay = true;
    public bool busy = false;
    MatureManager manager;
    List<PlayerState> availablePlayers;
    Dictionary<int, PlayerAgent> agents = new Dictionary<int, PlayerAgent>();

    public override void InitializeAcademy()
    {
        manager = GetComponent<MatureManager>();
        manager.Start();
        board = manager.board;
        availablePlayers = new List<PlayerState>(board.players);
        //availablePlayers.RemoveAt(0);
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
            var playerId = player.id;
            if(agents.ContainsKey(playerId))
            {
                agents[playerId].player = player;
            }
        }
    }

    public async void StepLater(PlayerAgent agent)
    {
        await Task.Delay(200);
        agent.RequestDecision();
    }

    public override void AcademyStep()
    {
        if(board.currentTurn == board.maxTurns)
        {
            AcademyDone();
        }
        if(busy)
        {
            return;
        }

        var playerId = board.currentPlayer.id;
        PlayerAgent agent = null;
        agents.TryGetValue(playerId,out agent);
        if(agent != null) {
            busy = true;
            if (slowPlay)
            {
                StepLater(agent);
            }
            else
            {
                agent.RequestDecision();
            }

        }
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
