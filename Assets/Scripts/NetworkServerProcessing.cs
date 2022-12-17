using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkServerProcessing : MonoBehaviour
{
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + clientConnectionID);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.balloonClicked)
        {
            int refID = int.Parse(csv[1]);
            // gameLogic.BalloonWasClicked(refID);
        }
        else if (signifier == ClientToServerSignifiers.playerHasLeftMatch)
        {
            int refID = int.Parse(csv[1]);
            DisconnectionEvent(refID);
        }

    }
    static public void SendMessageToClient(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClient(msg, clientConnectionID);
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);
        // gameLogic.AddConnectedClient(clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
        // gameLogic.RemoveConnectedClient(clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int balloonClicked = 1;
    public const int playerHasLeftMatch = 2;
}

static public class ServerToClientSignifiers
{
    public const int spawnNewBalloon = 1;
    public const int balloonWasPopped = 2;
}

#endregion


//private void ProcessRecievedMsg(string msg, int id)
//{
//    Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

//    string[] msgFromClient = msg.Split(',');
//    msgSignifier = msgFromClient[0];

//    switch (msgSignifier)
//    {
//        case "CreateAccount":
//            CreateNewAccount(msg, id);
//            break;
//        case "Login":
//            Login(msg, id);
//            break;
//        case "JoinRoom":
//            CreateGameRoom(msg, id);
//            break;
//        case "LeaveRoom":
//            PlayerLeavingRoom(msg, id);
//            break;
//        case "SendMessage":
//            SendMessageBetweenClients(msg, id);
//            break;
//    }

//}
