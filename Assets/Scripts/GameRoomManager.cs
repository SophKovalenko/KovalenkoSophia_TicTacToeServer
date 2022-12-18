using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameRoomManager : MonoBehaviour
{
    public string gameRoomName;

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

    public void CreateGameRoom(int id)
    {
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
                }
            }
            numPlayerConnectedInThisRoom++;
        }
        CheckForTwoClients();
    }

    void CheckForTwoClients()
    {

        if (twoClientsConnected == true && numPlayerConnectedInThisRoom == 2)
        {
            foreach (int playerID in LoginManager.connectedPlayerIDs)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.startGame + ",", playerID);
            }
        }
    }

    //void PlayerLeavingRoom(string msg, int id)
    //{
    //    numPlayerConnectedInThisRoom--;

    //    if (id == player1ConnectionID)
    //    { SendMessageToClient("LeavingRoom", player2ConnectionID); }

    //    if (id == player2ConnectionID)
    //    { SendMessageToClient("LeavingRoom", player1ConnectionID); }
    //}
}
