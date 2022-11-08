using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRooms : MonoBehaviour
{
    public string GameRoomId { get; set; }

    public GameRooms(string gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}
