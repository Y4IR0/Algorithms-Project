using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using NaughtyAttributes;

public class DungeonGenerator : MonoBehaviour
{
    Random rng;
    
    public new Graph<RectInt> dungeon = new Graph<RectInt>();
    
    public new List<RectInt> rooms = new List<RectInt>();
    public new List<RectInt> doors = new List<RectInt>(); 
    
    new List<RectInt> splittableRooms = new List<RectInt>();
 
    [Header("Specifications")]
    [SerializeField] public RectInt boundary = new RectInt(0, 0, 200, 200);
    [SerializeField] RectInt minRoomRect = new RectInt(0, 0, 10, 10);
    [SerializeField] int wallThickness = 2;
    [SerializeField] int doorWidth = 4;

    [Header("Seed")]
    [SerializeField] int seed;

    [Header("Visual")]
    [SerializeField] bool isAnimated = true;
    [SerializeField] bool showWalls = true;



    void DrawRooms()
    {
        AlgorithmsUtils.DebugRectInt(boundary, Color.green, Time.deltaTime, false, 0);

        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            AlgorithmsUtils.DebugRectInt(room, Color.red, Time.deltaTime, false, 0);

            if (showWalls)
            {
                RectInt wallLeft = new RectInt(room.x, room.y, wallThickness/2, room.height);
                RectInt WallTop = new RectInt(room.x, room.y + room.height - wallThickness/2, room.width, wallThickness/2);
                RectInt WallRight = new RectInt(room.x + room.width - wallThickness/2, room.y, wallThickness/2, room.height);
                RectInt WallBottom = new RectInt(room.x, room.y, room.width, wallThickness/2);
                
                AlgorithmsUtils.DebugRectInt(wallLeft, Color.yellow, Time.deltaTime, false, 0);
                AlgorithmsUtils.DebugRectInt(WallTop, Color.yellow, Time.deltaTime, false, 0);
                AlgorithmsUtils.DebugRectInt(WallRight, Color.yellow, Time.deltaTime, false, 0);
                AlgorithmsUtils.DebugRectInt(WallBottom, Color.yellow, Time.deltaTime, false, 0);
            }
        }
    }

    void DrawDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            RectInt room = doors[i];
            AlgorithmsUtils.DebugRectInt(room, Color.cyan, Time.deltaTime, false, 0);
        }
    }

    void DrawGraph()
    {
        foreach (RectInt room in dungeon.GetNodes())
        {
            Vector3 roomPos = new Vector3(room.x + room.width / 2, Time.deltaTime, room.y + room.height / 2);
            DebugExtension.DebugCircle(roomPos, Color.green, 3, Time.deltaTime, false);

            foreach (RectInt neighbor in dungeon.GetNeighbors(room))
            {
                Vector3 neighborPos = new Vector3(neighbor.x + neighbor.width / 2, 0, neighbor.y + neighbor.height / 2);
                Debug.DrawLine(roomPos, neighborPos, Color.green, Time.deltaTime, false);
            }
        }
    }

    void Start()
    {
        GenerateDungeon();
    }

    void Update()
    {
        DrawGraph();
        DrawDoors();
        DrawRooms();
    }

    [Button("Generate Dungeon")]
    IEnumerator GenerateDungeon()
    {
        dungeon.Clear();
        rooms.Clear();
        doors.Clear();
        splittableRooms.Clear();
        
        
        rng = new Random(Convert.ToUInt32(seed));
        
        yield return StartCoroutine(GenerateRooms());
        yield return StartCoroutine(GenerateDoors());
        yield return StartCoroutine(RemoveRooms());
        yield return StartCoroutine(GenerateDoors());
        yield return StartCoroutine(GenerateDungeonGraph());
    }
    
    
    
    IEnumerator GenerateRooms()
    {
        rooms.Clear();
        SplitRoom(boundary, rng.NextBool());

        int safetyAmount = 10000; // Unity kept crashing due to infinite while loops at the start, this no longer is necessary.
        int previousSplittableRoomsCount = splittableRooms.Count;
        int previousSplittableRoomsCountStreak = 0;
        while (splittableRooms.Count > 0 && safetyAmount > 0)
        {
            int currentSplittableRoomsCount = splittableRooms.Count;
            if (previousSplittableRoomsCount == currentSplittableRoomsCount)
                previousSplittableRoomsCountStreak++;
            else
                previousSplittableRoomsCountStreak = 0;

            if (previousSplittableRoomsCountStreak >= 50) break;
            
            if (isAnimated) yield return new WaitForEndOfFrame();
            safetyAmount--;
            
            SplitRoom(splittableRooms[0], rng.NextBool());
            
            previousSplittableRoomsCount = currentSplittableRoomsCount;
        }


        void SplitRoom(RectInt room, bool splitHorizontally)
    {
        RectInt room1;
        RectInt room2;

        float room1Divider = rng.NextInt(4, 6) * .1f;
        
        
        if (splitHorizontally)
        {
            int room1Height = Mathf.RoundToInt(room.height * room1Divider);
            int room2Height =  Mathf.RoundToInt(room.height - room1Height);
            
            room1 = new RectInt(room.x, room.y + room2Height, room.width, room1Height);
            room2 = new RectInt(room.x, room.y, room.width, room2Height);
        }
        else
        {
            int room1Width = Mathf.RoundToInt(room.width * room1Divider);
            int room2Width = Mathf.RoundToInt(room.width - room1Width);
            
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
    }

    
    
    IEnumerator GenerateDoors()
    {
        doors.Clear();
        
        for (int i = 0; i < rooms.Count; i++)
        {
            if (isAnimated) yield return new WaitForEndOfFrame();
            RectInt targetRoom = rooms[i];
            
            for (int ii = 0; ii < rooms.Count; ii++)
            {
                RectInt neighborRoom = rooms[ii];
                
                if (neighborRoom == targetRoom) continue;
                
                // Creating variables
                int x;
                int y;
                int width;
                int height;
                RectInt door;
                
                
                // Checking left side
                RectInt leftWall = new RectInt(targetRoom.x - wallThickness/2, targetRoom.y, wallThickness, targetRoom.height);
                RectInt leftIntersection = AlgorithmsUtils.Intersect(neighborRoom, leftWall);

                if (leftIntersection != RectInt.zero && leftIntersection.height > doorWidth * 4)
                {
                    x = leftIntersection.x;
                    y = leftIntersection.y + rng.NextInt(doorWidth * 2, leftIntersection.height - doorWidth * 2);
                    width = wallThickness;
                    height = doorWidth;
                            
                    door = new RectInt(x, y, width, height);
                    doors.Add(door); 
                    //dungeon.AddEdge(targetRoom, neighborRoom); // Adds connection to dungeon graph
                }
                
                
                // Checking bottom side
                RectInt bottomWall = new RectInt(targetRoom.x, targetRoom.y - wallThickness/2, targetRoom.width, wallThickness);
                RectInt bottomIntersection = AlgorithmsUtils.Intersect(neighborRoom, bottomWall);

                if (bottomIntersection != RectInt.zero && bottomIntersection.width > doorWidth * 4)
                {
                    x = bottomIntersection.x + rng.NextInt(doorWidth * 2, bottomIntersection.width - doorWidth * 2);
                    y = bottomIntersection.y;
                    width = doorWidth;
                    height = wallThickness;
                            
                    door = new RectInt(x, y, width, height);
                    doors.Add(door);
                    //dungeon.AddEdge(targetRoom, neighborRoom); // Adds connection to dungeon graph
                }
            }
        }
    }
    
    
    
    IEnumerator GenerateDungeonGraph()
    {
        dungeon.Clear();
        
        for (int i = 0; i < doors.Count; i++)
        {
            if (isAnimated) yield return new WaitForEndOfFrame();
            
            RectInt door = doors[i];
            
            RectInt room1 = RectInt.zero;
            RectInt room2 = RectInt.zero;

            for (int ii = 0; ii < rooms.Count; ii++)
            {
                RectInt currentRoom = rooms[ii];
                bool intersects = AlgorithmsUtils.Intersects(door, currentRoom);

                if (intersects)
                {
                    if (room1 == RectInt.zero)
                        room1 = currentRoom;
                    else
                        room2 = currentRoom;
                }
            }
            
            if (room1 != RectInt.zero && room2 != RectInt.zero)
                dungeon.AddEdge(room1, room2); // Adds connection to dungeon graph
        }
        
        // Logs message about if all rooms are connected
        if (dungeon.GetNodeCount() == rooms.Count)
            Debug.Log("All rooms are connected! " + dungeon.GetNodeCount() + "/" +  rooms.Count);
        else
            Debug.Log("Not all rooms are connected! " + dungeon.GetNodeCount() + "/" +  rooms.Count);
    }

    
    
    IEnumerator RemoveRooms()
    {
        List<RectInt> roomsDuplicate = new List<RectInt>();

        foreach (RectInt room in rooms) // Fills the roomsDuplicate list
            roomsDuplicate.Add(room);
        
        rooms.Clear(); // Clears rooms
        
        while (roomsDuplicate.Count > 1) // Sort rooms from high to low
        {
            int largestRoomIndex = -1;
            int largestRoomSize = 0;
        
            for (int i = 0; i < roomsDuplicate.Count; i++)
            {
                RectInt currentRoom = roomsDuplicate[i];
                int currentRoomSize = currentRoom.height * currentRoom.width;
                
                if (currentRoomSize > largestRoomSize)
                {
                    largestRoomIndex = i;
                    largestRoomSize = currentRoomSize;
                }
            }
        
            rooms.Add(roomsDuplicate[largestRoomIndex]);
            roomsDuplicate.Remove(roomsDuplicate[largestRoomIndex]);
        }
        
        int roomsStartCount = rooms.Count;
        int roomsEndCount = Convert.ToInt32(roomsStartCount * .9f);
        
        while (rooms.Count > roomsEndCount) // Removes rooms until not able
        {
            RectInt toRemoveRoom = rooms[rooms.Count - 1];
            rooms.RemoveAt(rooms.Count - 1);

            if (DungeonIsConnected() == false) // Checks if not connected
            {
                rooms.Add(toRemoveRoom);
                break;
            }
            
            if (isAnimated) yield return new WaitForEndOfFrame();
        }
        
        // Log amount of removed rooms
        Debug.Log("Succesfully removed " + (roomsStartCount - rooms.Count) + " rooms!");
    }

    
    
    bool DungeonIsConnected()
    {
        dungeon.Clear();
        
        for (int i = 0; i < doors.Count; i++)
        {
            RectInt door = doors[i];
            
            RectInt room1 = RectInt.zero;
            RectInt room2 = RectInt.zero;

            for (int ii = 0; ii < rooms.Count; ii++)
            {
                RectInt currentRoom = rooms[ii];
                bool intersects = AlgorithmsUtils.Intersects(door, currentRoom);

                if (intersects)
                {
                    if (room1 == RectInt.zero)
                        room1 = currentRoom;
                    else
                        room2 = currentRoom;
                }
            }
            
            if (room1 != RectInt.zero && room2 != RectInt.zero)
                dungeon.AddEdge(room1, room2); // Adds connection to dungeon graph
        }
        
        return dungeon.GetNodeCount() == rooms.Count;
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














/*
 // Generate Doors
 
                RectInt room2 = new RectInt(neighborRoom.x - 2, neighborRoom.y - 2, neighborRoom.width + 2, neighborRoom.height + 2);
                
                bool intersects = AlgorithmsUtils.Intersects(biggerNeighborRoom, room2);

                if (intersects && targetRoom != neighborRoom) // Make new door
                {
                    Vector2 targetRoomPosition = new Vector2(targetRoom.x, targetRoom.y);
                    Vector2 neighborRoomPosition = new Vector2(neighborRoom.x, neighborRoom.y);
                    Vector2 direction = (neighborRoomPosition - targetRoomPosition).normalized;
                    
                    int doorWidth = 3;

                    int x;
                    int y;
                    int width;
                    int height;
                    RectInt door;
                    
                    Debug.Log(direction);
                    
                    switch (direction)
                    {
                        case var value when value == Vector2.left:
                            x = targetRoom.x - wallThickness / 2;
                            y = Random.Range(doorWidth, targetRoom.height - doorWidth);
                            width = wallThickness;
                            height = doorWidth;
                            
                            door = new RectInt(x, y, width, height);
                            doors.Add(door);
                            break;
                        
                        case var value when value ==  Vector2.right:
                            x = targetRoom.x - wallThickness/2 + targetRoom.width;
                            y = Random.Range(doorWidth, targetRoom.height - doorWidth);
                            width = wallThickness;
                            height = doorWidth;
                            
                            door = new RectInt(x, y, width, height);
                            doors.Add(door);
                            break;
                        
                        case var value when value ==  Vector2.up:
                            x = Random.Range(doorWidth, targetRoom.width - doorWidth);
                            y = targetRoom.y - wallThickness/2 + targetRoom.height;
                            width = doorWidth;
                            height = wallThickness;
                            
                            door = new RectInt(x, y, width, height);
                            doors.Add(door);
                            break;
                        
                        case var value when value ==  Vector2.down:
                            x = Random.Range(doorWidth, targetRoom.width - doorWidth);
                            y = targetRoom.y - wallThickness/2;
                            width = doorWidth;
                            height = wallThickness;
                            
                            door = new RectInt(x, y, width, height);
                            doors.Add(door);
                            break;
                    }
                }
                */