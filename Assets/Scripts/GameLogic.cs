using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PlayerCharacterString
{
    NONE,
    X,
    O
}

public class GameLogic : MonoBehaviour
{
    public PlayerCharacterString playerChoice;
    public int turnsTaken;

    //Make a list of all the buttons on the client board
    public string[] buttonList =
        new string[10] { "Button", "Button1", "Button2", "Button3", "Button4", "Button5", "Button6", "Button7", "Button8", "Button9" };

    GameRoomManager gameRoomManagerRef;
    LoginManager loginManager;

    public LinkedList<int> connectedClientIds;

    // Start is called before the first frame update
    void Start()
    {
        connectedClientIds = new LinkedList<int>();
        turnsTaken = 0;
        loginManager = FindObjectOfType<LoginManager>();
        NetworkedServerProcessing.SetGameLogic(this);
    }

    public void AssignSides(int id)
    {
        if (id == loginManager.player1ConnectionID)
        {
            playerChoice = PlayerCharacterString.X;
        }
        if (id == loginManager.player2ConnectionID)
        {
            playerChoice = PlayerCharacterString.O;
        }

        NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.sideAssignment + "," + playerChoice.ToString() + ",", id);
    }

    public void CheckWin(int id)
    {
        //Check all the rows
        if (buttonList[0] == playerChoice.ToString() && buttonList[1] == playerChoice.ToString() && buttonList[2] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }
        if (buttonList[3] == playerChoice.ToString() && buttonList[4] == playerChoice.ToString() && buttonList[5] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }
        if (buttonList[6] == playerChoice.ToString() && buttonList[7] == playerChoice.ToString() && buttonList[8] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }

        //Check all the colomns
        if (buttonList[0] == playerChoice.ToString() && buttonList[3] == playerChoice.ToString() && buttonList[6] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }
        if (buttonList[1] == playerChoice.ToString() && buttonList[4] == playerChoice.ToString() && buttonList[7] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }
        if (buttonList[2] == playerChoice.ToString() && buttonList[5] == playerChoice.ToString() && buttonList[8] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }

        //Check all the diagonals
        if (buttonList[0] == playerChoice.ToString() && buttonList[4] == playerChoice.ToString() && buttonList[8] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }
        if (buttonList[2] == playerChoice.ToString() && buttonList[4] == playerChoice.ToString() && buttonList[6] == playerChoice.ToString())
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameOver + "," + playerChoice.ToString(), id);
        }

        if (turnsTaken >= 9)
        {
            foreach (int playerID in LoginManager.connectedPlayerIDs)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameDraw + ",", playerID);
            }
        }

    }

    public void ChangeTurn()
    {
        //If player is X, change to O or vise versa
        playerChoice = (playerChoice == PlayerCharacterString.X) ? PlayerCharacterString.O : PlayerCharacterString.X;

        foreach (int clientConnectedID in connectedClientIds)
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.changeTurn + "," + playerChoice.ToString(), clientConnectedID);
        }
    }

    public void AddConnectedClient(int clientConnectionID)
    {
        connectedClientIds.AddLast(clientConnectionID);
    }

    public void RemoveConnectedClient(int clientConnectionID)
    {
        if (connectedClientIds.Contains(clientConnectionID))
        {
            connectedClientIds.Remove(clientConnectionID);
        }
    }
}


