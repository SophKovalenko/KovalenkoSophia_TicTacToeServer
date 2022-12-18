using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public string userName;
    public string passWord;
    string savedPassword;
    int savedConnectionId;
    int connectionIdOfAccount;
    bool loggedIntoSystem;
    bool accountAuthenticated = false;
    string msgSignifier;

    public bool player1Connected = false;
    public int player1ConnectionID;
    public bool player2Connected = false;
    public int player2ConnectionID;
    public bool twoClientsConnected = false;

    //Used for storing the login info of players into text docs
    public static string loginFileNames = "";

    //Create a new list for usernames/ passwords
    static List<string> fileNames = new List<string>();

    //Create a new list for connectedPlayers
    public static List<int> connectedPlayerIDs = new List<int>();

    void Start()
    {
        string line = "";

        using (StreamReader sr = new StreamReader("savedLogins.txt"))
        {
            while ((line = sr.ReadLine()) != null)
            {
                fileNames.Add(line);

            }
        }
    }

    public void Login(int id)
    {
        if (fileNames.Contains(userName) == true)
        {
            string line = "";

            using (StreamReader sr = new StreamReader(userName + ".txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    string[] savedInfo = line.Split(',');

                    savedPassword = savedInfo[1];
                    Debug.Log(savedPassword);
                    savedConnectionId = int.Parse(savedInfo[2]);

                    if (passWord == savedPassword)
                    {
                        if (player1Connected == false && player2Connected == false)
                        {
                            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.loginSuccessful + "," + userName, id);
                            player1Connected = true;
                            player1ConnectionID = id;
                            connectedPlayerIDs.Add(player1ConnectionID);
                            Debug.Log("One player is connected");
                        }
                        else if (player1Connected == true && player2Connected == false)
                        {
                            loggedIntoSystem = true;
                            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.loginSuccessful + "," + userName, id);
                            player2Connected = true;
                            player2ConnectionID = id;
                            connectedPlayerIDs.Add(player2ConnectionID);
                            twoClientsConnected = true;
                            Debug.Log("Two players are connected");
                        }
                        break;
                    }

                    if (passWord != savedPassword)
                    {
                       loggedIntoSystem = false;
                       NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.wrongPassword + "," + userName, id);
                    }
                }

            }
            if (fileNames.Contains(userName) == false)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.wrongUsername + "," + userName, id);
                Debug.Log("That username does not exist");
            }

        }

    }

    public void CreateNewAccount(int id)
    {
        if (fileNames.Contains(userName) == false)
        {
            fileNames.Add(userName);

            using (StreamWriter sw = new StreamWriter(userName + ".txt"))
            {
                sw.WriteLine(userName + "," + passWord + "," + connectionIdOfAccount + ",");
            }

            using (StreamWriter sw = new StreamWriter("savedLogins.txt"))
            {
                foreach (var fileName in fileNames)
                {
                    sw.WriteLine(fileName);
                }
            }
        }
        else if (fileNames.Contains(userName) == true)
        {
            Debug.Log("Username already taken, please choose another");

        }
    }
}
