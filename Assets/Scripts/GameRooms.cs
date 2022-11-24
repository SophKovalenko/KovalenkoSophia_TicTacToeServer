using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SerializeField]
public struct GameRoom
{
    public string name;
    public int numPlayers;
    public int connectionIdPlayer1;
    public int connectionIdPlayer2;
    public bool player1Connected;
    public bool player2Connected;
}
