using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public class DungeonGenerator : MonoBehaviour
{
    new List<RectInt> rooms = new List<RectInt>();
    new List<RectInt> splittableRooms = new List<RectInt>();
 
    [SerializeField] RectInt boundary = new RectInt(0, 0, 200, 200);
    [SerializeField] RectInt minRoomRect = new RectInt(0, 0, 10, 10);
    //[SerializeField] int wallThickness = 2;



    void DrawRooms()
    {
        AlgorithmsUtils.DebugRectInt(boundary, Color.green, 100000, false, 0);

        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            AlgorithmsUtils.DebugRectInt(room, Color.yellow, 100000, false, .03f * i);
        }
    }

    void SplitRoom(RectInt room, bool splitHorizontally)
    {
        RectInt room1;
        RectInt room2;

        float room1Divider = Random.Range(3, 7) * .1f;
        float room2Divider = 1 - room1Divider;
        
        
        if (splitHorizontally)
        {
            int room1Height;
            int room2Height;
            
            if (room.height % 2 == 0)   // Even
            {
                room1Height = Mathf.RoundToInt(room.height * room1Divider);
                room2Height = Mathf.RoundToInt(room.height * room2Divider);
            }
            else                        // Odd
            {
                room1Height = Mathf.RoundToInt(room.height * room1Divider);
                room2Height = Mathf.RoundToInt(room.height * room2Divider + .49999f);
            }
            
            room1 = new RectInt(room.x, room.y + room2Height, room.width, room1Height);
            room2 = new RectInt(room.x, room.y, room.width, room2Height);
        }
        else
        {
            int room1Width;
            int room2Width;
            
            if (room.width % 2 == 0)    // Even
            {
                room1Width = Mathf.RoundToInt(room.width * room1Divider);
                room2Width = Mathf.RoundToInt(room.width * room2Divider);
            }
            else                        // Odd
            {
                room1Width = Mathf.RoundToInt(room.width * room1Divider);
                room2Width = Mathf.RoundToInt(room.width * room2Divider + .49999f);
            }
            
            room1 = new RectInt(room.x, room.y, room1Width, room.height);
            room2 = new RectInt(room.x + room1Width, room.y, room2Width, room.height);
        }
        
        
        
        
        // Check if not splittable
        if (room1.width <= minRoomRect.width || room2.width <= minRoomRect.width || room1.height <= minRoomRect.height || room2.height <= minRoomRect.height)
        {
        }
        else
        {
            splittableRooms.Add(room1);
            splittableRooms.Add(room2);
            
            rooms.Add(room1);
            rooms.Add(room2);
            
            splittableRooms.Remove(room);
            rooms.Remove(room);
        }
    }

    void Awake()
    {
        GenerateRooms();
    }

    [Button("Generate Rooms")]
    void GenerateRooms()
    {
        SplitRoom(boundary, Random.Range(0, 1) == 1);

        int amount = 0;
        while (splittableRooms.Count > 0 && amount < 10000)
        {
            amount++;
            SplitRoom(splittableRooms[0], Random.Range(0, 2) == 1);
        }
        

        DrawRooms();
    }
}

/*
new List<RectInt> rooms = new List<RectInt>();
    new List<RectInt> horizontalSplittableRooms = new List<RectInt>();
    new List<RectInt> verticalSplittableRooms = new List<RectInt>();
 
    [SerializeField] RectInt boundary = new RectInt(0, 0, 200, 200);
    [SerializeField] RectInt minRoomRect = new RectInt(0, 0, 10, 10);
    //[SerializeField] int wallThickness = 2;



    void DrawRooms()
    {
        AlgorithmsUtils.DebugRectInt(boundary, Color.green, 100000, false, 0);

        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            AlgorithmsUtils.DebugRectInt(room, Color.yellow, 100000, false, .03f * i);
        }
    }

    void SplitRoom(RectInt room, bool splitHorizontally)
    {
        RectInt room1;
        RectInt room2;

        float room1Divider = Random.Range(3, 7) * .1f;
        float room2Divider = 1 - room1Divider;
        
        
        if (splitHorizontally)
        {
            int room1Height;
            int room2Height;
            
            if (room.height % 2 == 0)   // Even
            {
                room1Height = Mathf.RoundToInt(room.height * room1Divider);
                room2Height = Mathf.RoundToInt(room.height * room2Divider);
            }
            else                        // Odd
            {
                room1Height = Mathf.RoundToInt(room.height * room1Divider);
                room2Height = Mathf.RoundToInt(room.height * room2Divider + .49999f);
            }
            
            room1 = new RectInt(room.x, room.y + room2Height, room.width, room1Height);
            room2 = new RectInt(room.x, room.y, room.width, room2Height);
        }
        else
        {
            int room1Width;
            int room2Width;
            
            if (room.width % 2 == 0)    // Even
            {
                room1Width = Mathf.RoundToInt(room.width * room1Divider);
                room2Width = Mathf.RoundToInt(room.width * room2Divider);
            }
            else                        // Odd
            {
                room1Width = Mathf.RoundToInt(room.width * room1Divider);
                room2Width = Mathf.RoundToInt(room.width * room2Divider + .49999f);
            }
            
            room1 = new RectInt(room.x, room.y, room1Width, room.height);
            room2 = new RectInt(room.x + room1Width, room.y, room2Width, room.height);
        }
        
        
        
        
        // Check if horizontally splittable
        if (room1.width > minRoomRect.width && room2.width > minRoomRect.width)
        {
            print("Horizontally Spilttable!");
            horizontalSplittableRooms.Add(room1);
            horizontalSplittableRooms.Add(room2);
        }
            
        // Check if vertically splittable
        if (room1.height > minRoomRect.height && room2.height > minRoomRect.height)
        {
            verticalSplittableRooms.Add(room1);
            verticalSplittableRooms.Add(room2);
        }
        
        rooms.Add(room1);
        rooms.Add(room2);
        
        if (splitHorizontally)
            horizontalSplittableRooms.Remove(room);
        else
            verticalSplittableRooms.Remove(room);

        rooms.Remove(room);
    }

    void Awake()
    {
        GenerateRooms();
    }

    [Button("Generate Rooms")]
    void GenerateRooms()
    {
        SplitRoom(boundary, Random.Range(0, 1) == 1);

        int amount = 0;
        while (horizontalSplittableRooms.Count > 0 && amount < 10000)
        {
            amount++;
            SplitRoom(horizontalSplittableRooms[0], true);
        }
        
        
        int amount2 = 0;
        while (verticalSplittableRooms.Count > 0 && amount2 < 10000)
        {
            amount2++;
            SplitRoom(verticalSplittableRooms[0], false);
            Debug.Log("AWEfhugyasefjhhjraswgfhjsjhrfghj");
        }
        

        DrawRooms();
    }
 */
