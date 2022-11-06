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

    string userName;
    string passWord;
    string savedPassword;
    int savedConnectionId;
    int connectionIdOfAccount;
    bool loggedIntoSystem;

    public static string loginFileNames = "";

    //Create a new list for usernames/ passwords
    static List<string> fileNames = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null);

        string line = "";

        using (StreamReader sr = new StreamReader("savedLogins.txt"))
        {
            while ((line = sr.ReadLine()) != null)
            {
                fileNames.Add(line);

            }
        }


    }

    // Update is called once per frame
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
                Debug.Log("Connection, " + recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                ProcessRecievedMsg(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnection, " + recConnectionID);
                break;
        }
    }

    public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] accountInfo = msg.Split(',');
        userName = accountInfo[0];
        passWord = accountInfo[1];
        connectionIdOfAccount = id;

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
            string line = "";

            using (StreamReader sr = new StreamReader(userName + ".txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    string[] savedInfo = line.Split(',');

                    savedPassword = savedInfo[1];
                    savedConnectionId = int.Parse(savedInfo[2]);

                    if (passWord == savedPassword)
                    {
                        loggedIntoSystem = true;
                        msg = loggedIntoSystem + "," + userName + "," + savedConnectionId + ",";
                        SendMessageToClient(msg, savedConnectionId);
                    }

                    if (passWord != savedPassword)
                    {
                        loggedIntoSystem = false;
                        msg = loggedIntoSystem + "," + userName + "," + savedConnectionId + ",";
                        SendMessageToClient(msg, savedConnectionId);
                    }
                }
            }

        }
    }
}
