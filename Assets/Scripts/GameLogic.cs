using System;
using System.Collections;
using System.Collections.Generic;
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

    GameRoomManager gameRoomManagerRef;
    LoginManager loginManager;


    // Start is called before the first frame update
    void Start()
    {
        turnsTaken = 0;
        loginManager = FindObjectOfType<LoginManager>();        
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

    // Update is called once per frame
    void Update()
    {
        if (turnsTaken >= 9)
        {
            foreach (int playerID in LoginManager.connectedPlayerIDs)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameDraw + ",", playerID);
            }
        }
    }

    void ChangeTurn()
    {
        //If player is X, change to O or vise versa
        playerChoice = (playerChoice == PlayerCharacterString.X) ? PlayerCharacterString.O : PlayerCharacterString.X;
    }
}

//Assign 1st player to X, second player to O
//Display move to both clients
//Change turns correctly
//Game won/ lost conditions