using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public class DungeonGenerator : MonoBehaviour
{
    RectInt boundary = new RectInt(0, 0, 40, 40);
    new List<RectInt> rooms = new List<RectInt>();
    private int splitAmount = 4;
    
    [SerializeField] RectInt maxRoomRect = new RectInt(0, 0, 15, 15);
    //[SerializeField] int wallThickness = 2;



    void DrawRooms()
    {
        AlgorithmsUtils.DebugRectInt(boundary, Color.green, 100000, false, .1f);
        
        foreach (RectInt room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow, 100000, false, .01f);
        }
    }

    void SplitRoom(RectInt room, bool splitHorizontally)
    {
        if (splitHorizontally)
        {
            RectInt room1 = new RectInt(room.x, room.y, room.width/2, room.height);
            RectInt room2 = new RectInt(room.x + room.width/2, room.y, room.width/2, room.height);
            
            rooms.Add(room1);
            rooms.Add(room2);
        }
        else
        {
            RectInt room1 = new RectInt(room.x, room.y + room.height/2, room.width, room.height/2);
            RectInt room2 = new RectInt(room.x, room.y, room.width, room.height/2);
            
            rooms.Add(room1);
            rooms.Add(room2);
        }

        rooms.Remove(room);
    }

    void Awake()
    {
        GenerateRooms();
    }

    [Button("Generate Rooms")]
    void GenerateRooms()
    {
        SplitRoom(boundary, Random.Range(0, 2) == 0);

        for (int i = 0; i < splitAmount; i++)
        {
            SplitRoom(rooms[Random.Range(0, rooms.Count)], Random.Range(0, 2) == 0);
        }

        DrawRooms();
    }
}
