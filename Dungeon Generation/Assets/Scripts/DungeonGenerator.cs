using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public class DungeonGenerator : MonoBehaviour
{
    RectInt boundary = new RectInt(0, 0, 100, 100);
    new List<RectInt> rooms = new List<RectInt>();
    [SerializeField] [Range(0, 128)] int splitAmount = 36;
    //[SerializeField] int wallThickness = 2;



    void DrawRooms()
    {
        foreach (RectInt room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow, 100000, false, .01f);
        }
    }

    void SplitRoom(RectInt room, bool splitHorizontally)
    {
        Debug.Log(rooms.IndexOf(room));
        Debug.Log(room);
        if (splitHorizontally)
        {
            RectInt room1 = new RectInt(room.x, room.y, room.width, room.height / 2);
            RectInt room2 = new RectInt(room.x, room.y, room.width, room.height / 2);

            //room1.height -= wallThickness / 2;
            //room2.yMin += wallThickness/2;
            room2.y += room2.height;
            
            rooms.Add(room1);
            rooms.Add(room2);
        }
        else
        {
            RectInt room1 = new RectInt(room.x, room.y, room.width / 2, room.height);
            RectInt room2 = new RectInt(room.x, room.y, room.width / 2, room.height);
            
            //room1.width -= wallThickness / 2;
            //room2.xMin += wallThickness/2;
            room2.x += room2.width;
            
            rooms.Add(room1);
            rooms.Add(room2);
        }

        //room = RectInt.zero; // Replace with proper deletion
    }

    void Awake()
    {
        GenerateRooms();
    }

    [Button("Generate Rooms")]
    void GenerateRooms()
    {
        AlgorithmsUtils.DebugRectInt(boundary, Color.green, 100000, false, .1f);

        SplitRoom(boundary, Random.Range(0, 2) == 0);

        for (int i = 0; i < splitAmount; i++)
        {
            Debug.Log(Random.Range(0, rooms.Count));
            SplitRoom(rooms[Random.Range(0, rooms.Count)], Random.Range(0, 2) == 0);
        }

        DrawRooms();
    }

    
    [Button("SplitRoom0")]
    void SplitRoom0()
    {
        SplitRoom(rooms[0], Random.Range(0, 2) == 0);
    }
    
    
    
    /*
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
     */
}
