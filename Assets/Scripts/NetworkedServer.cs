using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class NetworkedServer : MonoBehaviour
{
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;

    string gameRoomName;

    string recievedMsgToSendToOtherClient;
    bool twoClientsConnected = false;
    int idOfSender;
    int idOfReciever;

    //Used for storing the saved game room info 
    public static string gameRoomNames = "";

    //Create a new list for gameRooms
    static List<string> gameRoomFileNames = new List<string>();

    int numPlayerConnectedInThisRoom;

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkedServerProcessing.GetNetworkedServer() == null)
        {
            NetworkedServerProcessing.SetNetworkedServer(this);

            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, socketPort, null);
        }
        else
        {
            Debug.Log("Singleton-ish architecture violation detected, investigate where NetworkedServer.cs Start() is being called.  Are you creating a second instance of the NetworkedServer game object or has the NetworkedServer.cs been attached to more than one game object?");
            Destroy(this.gameObject);
        }

        string line = "";

        using (StreamReader sr = new StreamReader("existingGameRooms.txt"))
        {
            while ((line = sr.ReadLine()) != null)
            {
                gameRoomFileNames.Add(line);
            }
        }

        numPlayerConnectedInThisRoom = 0;
    }


    void Update()
    {
        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error = 0;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                NetworkedServerProcessing.ConnectionEvent(recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                NetworkedServerProcessing.ReceivedMessageFromClient(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                NetworkedServerProcessing.DisconnectionEvent(recConnectionID);
                break;
        }

    }
    public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    void CreateGameRoom(string msg, int id)
    {
        //Now recieving info to create game room
        string[] gameRoomInfo = msg.Split(',');
        gameRoomName = gameRoomInfo[1];

        if (gameRoomFileNames.Contains(gameRoomName) == false)
        {
          
            gameRoomFileNames.Add(gameRoomName);

            using (StreamWriter sw = new StreamWriter(gameRoomName + ".txt"))
            {
                sw.WriteLine(gameRoomName);

            }

            using (StreamWriter sw = new StreamWriter("existingGameRooms.txt"))
            {
                foreach (var gameRoomFileName in gameRoomFileNames)
                {
                    sw.WriteLine(gameRoomFileName);
                }
            }
            numPlayerConnectedInThisRoom++;

        }
        else if (gameRoomFileNames.Contains(gameRoomName) == true)
        {
            
            string line = "";

            using (StreamReader sr = new StreamReader(gameRoomName + ".txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    string[] savedInfo = line.Split(',');

                    //Pull info we need from the client
                    gameRoomName = savedInfo[1];

                }
            }

        }
        numPlayerConnectedInThisRoom++;
        //CheckForTwoClients();
    }


    void SendMessageBetweenClients(string msg, int id)
    {
        if (id == FindObjectOfType<LoginManager>().player1ConnectionID)
        { SendMessageToClient("Hello", FindObjectOfType<LoginManager>().player2ConnectionID); }

        if (id == FindObjectOfType<LoginManager>().player2ConnectionID)
        { SendMessageToClient("Hello", FindObjectOfType<LoginManager>().player1ConnectionID); }

    }

    //void CheckForTwoClients()
    //{

    //    if (twoClientsConnected == true && numPlayerConnectedInThisRoom == 2)
    //    {
    //        foreach (int playerID in connectedPlayerIDs)
    //        {
    //            SendMessageToClient("StartGame", playerID);
    //        }
    //    }
    //}

    //void PlayerLeavingRoom(string msg, int id)
    //{
    //    numPlayerConnectedInThisRoom--;

    //    if (id == player1ConnectionID)
    //    { SendMessageToClient("LeavingRoom", player2ConnectionID); }

    //    if (id == player2ConnectionID)
    //    { SendMessageToClient("LeavingRoom", player1ConnectionID); }
    //}

}
