using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

public abstract class GameClient
{
    protected BoardState board;
    protected int clientId;
    protected int messageId = 0;
    public BlockingCollection<MSG> pending = new BlockingCollection<MSG>();

    public GameClient(int id,BoardState board)
    {
        this.board = board;
        this.clientId = id;
    }

    public virtual void ProcessMsg(MSG msg)
    {
        if(msg.clientId == this.clientId)
        {
            return;
        }
        pending.Add(msg);
    }

    public byte[] SerialiseMsg(MSG msg)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, msg);
            return stream.ToArray();
        }
    }

    public MSG DeserializeMsg(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using(var stream = new MemoryStream(data))
        {
            return (MSG)formatter.Deserialize(stream);
        }
    }

    public void SendPlayerMoved(PlayerState player,GridInfo info)
    {
        this.Send(new PlayerMoved(this.clientId, messageId++, player, info));
    }

    public void SendPlayerUsedAction(PlayerState player)
    {
        this.Send(new PlayerUsedAction(this.clientId, messageId++, player));
    }

    public void SendPlayerUsedCard(PlayerState player,Card card,GridInfo grid)
    {
        this.Send(new PlayerUsedCard(this.clientId, this.messageId++, player, card, grid));
    }

    public void SendPlayerUsedItem(PlayerState player, PlayerState target, Item item) {
        this.Send(new PlayerUsedItem(this.clientId, this.messageId++, player, item, target));
    }
    public abstract void Send(MSG msg);
}

public class Client : GameClient
{

    TcpClient client;

    public Client() : base(0,null)
    {

    }

    public async Task<BoardState> JoinGame(string ip,BoardState board)
    {
        client = new TcpClient();
        await client.ConnectAsync(ip, 8001);
        int id = await RecieveInteger();
        int seed = await RecieveInteger();
        this.clientId = id;
        board.Init(seed);
        RecieveLoop();
        return board;
    }

    private async Task<int> RecieveInteger()
    {
        var stream = client.GetStream();
        var data = new byte[sizeof(int)];
        await stream.ReadAsync(data, 0, sizeof(int));
        return BitConverter.ToInt32(data, 0);
    }

    public override void Send(MSG msg)
    {
        if(client.Connected)
        {
            byte[] data = SerialiseMsg(msg);
            var stream = client.GetStream();
            try
            {
                stream.Write(BitConverter.GetBytes(data.Length),0,sizeof(int));
                stream.Write(data,0,data.Length);
            }
            catch (Exception err) { }
        }
    }

    public async void RecieveLoop()
    {
        NetworkStream stream = this.client.GetStream();
        try
        {
            while (true)
            {
                byte[] header = new byte[sizeof(int)];
                await stream.ReadAsync(header, 0, sizeof(int));
                int length = BitConverter.ToInt32(header, 0);
                if (length > 90000)
                {
                    throw new Exception("Bad you");
                }
                byte[] messageData = new byte[length];
                await stream.ReadAsync(messageData, 0, length);
                var msg = DeserializeMsg(messageData);
                this.ProcessMsg(msg);
            }
        }
        catch (Exception err)
        {
            Debug.Log("Aborted Client Comms");
            Debug.Log(err);
        }
        finally
        {
            if (stream != null)
            {
                stream.Close();
            }
        }
    }
}

public class Server : GameClient
{
    protected List<Socket> clients = new List<Socket>();
    protected object _clientsLock = new object();
    protected List<MSG> replay = new List<MSG>();
    protected TcpListener listener;
    protected int nextClientId = 1;

    public Server(BoardState boardState) : base(0,boardState)
    {

    }

    public void RemoveClient(Socket client)
    {
        lock(_clientsLock)
        {
            clients.Remove(client);
        }
    }

    public void AddClient(Socket client)
    {
        lock(_clientsLock)
        {
            clients.Add(client);
        }
    }

    public async void Start()
    {
        try
        {
            IPAddress ipAd = IPAddress.Parse("127.0.0.1");
            // use local m/c IP address, and 
            // use the same in the client

            /* Initializes the Listener */
            listener = new TcpListener(ipAd, 8001);

            /* Start Listeneting at the specified port */
            listener.Start();

            Console.WriteLine("The server is running at port 8001...");
            Console.WriteLine("The local End point is  :" +
                              listener.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");

            while(true)
            {
                Socket s = await listener.AcceptSocketAsync();
                s.Send(BitConverter.GetBytes(nextClientId++));
                s.Send(BitConverter.GetBytes(board.randomSeed));
                this.RecieveLoop(s);
                this.AddClient(s);
            }

            /* clean up */

        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }

    public async Task<bool> TestAsync()
    {
        await Task.Delay(2000);
        return false;
    }

    public async void RecieveLoop(Socket socket)
    {
        NetworkStream stream = null;
        try
        {
            stream = new NetworkStream(socket, true);
            while (true)
            {
                byte[] header = new byte[sizeof(int)];
                await stream.ReadAsync(header, 0, sizeof(int));
                int length = BitConverter.ToInt32(header, 0);
                if (length > 90000)
                {
                    throw new Exception("Bad you");
                }
                byte[] messageData = new byte[length];
                await stream.ReadAsync(messageData, 0, length);
                var msg = DeserializeMsg(messageData);
                this.ProcessMsg(msg);
            }
        }
        catch(Exception err)
        {
            Debug.Log("Aborted Comms");
            Debug.Log(err);
        }
        finally
        {
            if(stream != null)
            {
                this.RemoveClient(socket);
                stream.Close();
            }
        }
    }

    public void Stop()
    {
        foreach(var socket in clients)
        {
            try
            {
                socket.Close();
            }
            catch (Exception err){}
        }
        listener.Stop();
    }

    public override void ProcessMsg(MSG msg)
    {
        base.ProcessMsg(msg);
        this.Send(msg);
    }

    public override void Send(MSG msg)
    {
        byte[] data = SerialiseMsg(msg);
        foreach(var socket in clients)
        {
            try
            {
                socket.Send(BitConverter.GetBytes(data.Length));
                socket.Send(data);
            }
            catch(Exception err){}
        }
    }



}
