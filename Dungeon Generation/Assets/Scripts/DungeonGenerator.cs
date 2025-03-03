using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    RectInt boundary = new RectInt(0, 0, 100, 50);
    new List<RectInt> rooms = new List<RectInt>();
    
    int wallThickness = 2;



    void DrawRooms()
    {
        foreach (RectInt room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow, 100000, false, .01f);
        }
    }

    void SplitRoom(RectInt room, bool splitHorizontally)
    {
        if (splitHorizontally)
        {
            RectInt room1 = new RectInt(room.xMin, room.yMin, room.width, room.height / 2);
            RectInt room2 = new RectInt(room.xMin, room.yMin + room.height / 2, room.width, room.height / 2);

            room1.height -= wallThickness / 2;
            room2.yMin += wallThickness/2;
            
            rooms.Add(room1);
            rooms.Add(room2);
        }
        else
        {
            RectInt room1 = new RectInt(room.xMin, room.yMin, room.width/2, room.height);
            RectInt room2 = new RectInt(room.xMin + room.width/2, room.yMin, room.width/2, room.height);
            
            room1.width -= wallThickness / 2;
            room2.xMin += wallThickness/2;
            
            rooms.Add(room1);
            rooms.Add(room2);
        }

        room = RectInt.zero; // Replace with proper deletion
    }

    void Awake()
    {
        AlgorithmsUtils.DebugRectInt(boundary, Color.green, 100000, false, .1f);

        SplitRoom(boundary, Random.Range(0, 2) == 0);
        
        for (int i = 0; i < 8; i++)
        {
            SplitRoom(rooms[Random.Range(0, rooms.Count)], Random.Range(0, 2) == 0);
        }
        
        DrawRooms();
    }
}
