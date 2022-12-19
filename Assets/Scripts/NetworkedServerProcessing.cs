using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedServerProcessing : MonoBehaviour
{
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + clientConnectionID);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.verifyLogin)
        {
            FindObjectOfType<LoginManager>().userName = csv[1];
            FindObjectOfType<LoginManager>().passWord = csv[2];
            FindObjectOfType<LoginManager>().Login(clientConnectionID);
        }
        if (signifier == ClientToServerSignifiers.createAccount)
        {
            FindObjectOfType<LoginManager>().userName = csv[1];
            FindObjectOfType<LoginManager>().passWord = csv[2];
            FindObjectOfType<LoginManager>().CreateNewAccount(clientConnectionID);
        }
        if (signifier == ClientToServerSignifiers.joinRoom)
        {  
            FindObjectOfType<GameRoomManager>().gameRoomName = csv[1];
            FindObjectOfType<GameRoomManager>().CreateGameRoom(clientConnectionID);
            gameLogic.AssignSides(clientConnectionID);
        }
        if (signifier == ClientToServerSignifiers.leaveRoom)
        {
            FindObjectOfType<GameRoomManager>().PlayerLeavingRoom(clientConnectionID);
        }
        if (signifier == ClientToServerSignifiers.sendMsg)
        {
            if (clientConnectionID == FindObjectOfType<LoginManager>().player1ConnectionID)
            { SendMessageToClient("Hello From Opponent", FindObjectOfType<LoginManager>().player2ConnectionID); }

            if (clientConnectionID == FindObjectOfType<LoginManager>().player2ConnectionID)
            { SendMessageToClient("Hello From Opponent", FindObjectOfType<LoginManager>().player1ConnectionID); }
        }
        if (signifier == ClientToServerSignifiers.turnTaken)
        {
            string buttonPressedByPlayer = csv[1];

            foreach (int connectedClient in gameLogic.connectedClientIds)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.buttonPressed + ",", connectedClient);
            }

            gameLogic.turnsTaken++;
            gameLogic.ChangeTurn();
            Debug.Log(gameLogic.turnsTaken);
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
        gameLogic.AddConnectedClient(clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
        gameLogic.RemoveConnectedClient(clientConnectionID);
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
    public const int verifyLogin = 1;
    public const int createAccount = 2;
    public const int joinRoom = 3;
    public const int leaveRoom = 4;
    public const int sendMsg = 5;
    public const int playerHasLeftMatch = 6;
    //During Game
    public const int turnTaken = 7;
}

static public class ServerToClientSignifiers
{
    //Login
    public const int loginSuccessful = 1;
    public const int wrongPassword = 2;
    public const int wrongUsername = 3;

    //Gamr Room
    public const int startGame = 4;
    public const int helloFromOtherPlayer = 5;

    //During Game
    public const int sideAssignment = 6;
    public const int buttonPressed = 7;
    public const int changeTurn = 8;
    public const int gameDraw = 9;
    public const int gameOver = 10;
}

#endregion

