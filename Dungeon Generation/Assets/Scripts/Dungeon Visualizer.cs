using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using AYellowpaper.SerializedCollections;
using Extensions;

public class DungeonVisualizer : MonoBehaviour
{
    public enum Object
    {
        Wall,
        CornerWall,
        Floor,
        Door1,
        Door2
    }
    
    
    
    public enum ObjectSpawnType
    {
        Simple,
        MarchingSquares
    }
    
    
    
    public enum FloorFillType
    {
        Simple,
        FloodFill
    }


    public enum Side
    {
        Bottom,
        Left,
        Top,
        Right,
    }
    
    [SerializeField]
    DungeonGenerator generator;
    
    [Header("Settings")]
    [SerializeField] private ObjectSpawnType objectSpawnType = ObjectSpawnType.Simple;
    [SerializeField] private FloorFillType floorFillType = FloorFillType.Simple;
    
    [Header("Objects")]
    [SerializedDictionary("Object", "Prefab")]
    public SerializedDictionary<Object, Transform> prefabs = new SerializedDictionary<Object, Transform>();
    private HashSet<Transform> objects = new HashSet<Transform>();
    private List<Vector3> doorPositions = new List<Vector3>();
    
    [Header("Marching Squares")]
    [SerializeField] private Transform[] indexes;
    
    [Header("Flood Fill")]
    [SerializeField] private  Graph<Vector2Int> floodfill = new Graph<Vector2Int>();
    
    
    void TrySpawnObject(Object obj, Vector2 position, Side side, Transform roomParent)
        {
            Vector3 spawnPositionOffset = new Vector3(.5f, 0, .5f);
            Vector3 spawnPosition = Vector3.zero;
            Vector3 spawnRotation = Vector3.zero;

            switch (side)
            {
                case Side.Bottom:
                    spawnRotation = Vector3.zero;
                    break;
                case Side.Left:
                    spawnRotation = new Vector3(0, 90, 0);
                    break;
                case Side.Top:
                    spawnRotation = new Vector3(0, 180, 0);
                    break;
                case Side.Right:
                    spawnRotation = new Vector3(0, -90, 0);
                    break;
            }
            
            spawnPosition = new Vector3(position.x, 0, position.y) + spawnPositionOffset;

            // Checks if can spawn
            bool canSpawn = true;
            
            // Checks if position already in use
            /* // Due to performance issues, it no longer try's, it does.
            foreach (Transform wall in objects)
            {
                if (spawnPosition == wall.position)
                    canSpawn = false;
            }
            */
            
            // Checks if position in door
            /* // Due to performance issues, it no longer try's, it does.
            foreach (RectInt door in dungeonGenerator.doors)
            {
                Vector3 doorPosition = new Vector3(door.x + .5f, 0, door.y + .5f);
                if (spawnPosition == doorPosition)
                    canSpawn = false;
            }
            */
            
            // Checks if position in door position
            foreach (Vector3 doorPosition in doorPositions)
            {
                if (spawnPosition == doorPosition)
                    canSpawn = false;
            }

            // Stops spawn if not able to
            if (!canSpawn) return;

            Transform newWall = Instantiate(prefabs[obj]);
            newWall.position = spawnPosition;
            newWall.localEulerAngles = spawnRotation;
            newWall.parent = roomParent;
            objects.Add(newWall);
        }


    
    void SpawnObject(Object obj, Vector2 position, Transform roomParent)
    {
        
    }
    
    
    
    [Button]
    public void SpawnDungeonAssets()
    {
        // Clear previous assets
        foreach (Transform wall in objects)
            Destroy(wall.gameObject);
        
        objects.Clear();
        
        
        
        // Add parent
        Transform parent = new GameObject("Rooms").transform; 
        objects.Add(parent);
        
        
        
        
        if (objectSpawnType == ObjectSpawnType.Simple)
        {
            // Door
        foreach (RectInt door in generator.doors)
        {
            //Vector3 doorPosition = new Vector3(door.x + .5f, 0, door.y + .5f);
            
            for (int x = 0; x < door.width; x++)
            {
                for (int y = 0; y < door.height; y++)
                {
                    Vector2 currentPosition = new Vector2(x + door.x, y + door.y);

                    if (door.width > door.height) // Horizontal
                    {
                        if (x == 0 && y == 0)
                            TrySpawnObject(Object.Door1, currentPosition, Side.Top, parent);
                        else if (x == door.width - 1 && y == 0)
                            TrySpawnObject(Object.Door2, currentPosition, Side.Top, parent);
                        else if (x == 0 && y == door.height - 1)
                            TrySpawnObject(Object.Door2, currentPosition, Side.Bottom, parent);
                        else if (x == door.width - 1 && y == door.height - 1)
                            TrySpawnObject(Object.Door1, currentPosition, Side.Bottom, parent);
                        else
                            TrySpawnObject(Object.Floor, currentPosition, Side.Bottom, parent);
                        
                        doorPositions.Add(new Vector3(currentPosition.x, 0, currentPosition.y) + new Vector3(.5f, 0, .5f));
                    }
                    else // Vertical
                    {
                        if (x == 0 && y == 0)
                            TrySpawnObject(Object.Door2, currentPosition, Side.Right, parent);
                        else if (x == door.width - 1 && y == 0)
                            TrySpawnObject(Object.Door1, currentPosition, Side.Left, parent);
                        else if (x == 0 && y == door.height - 1)
                            TrySpawnObject(Object.Door1, currentPosition, Side.Right, parent);
                        else if (x == door.width - 1 && y == door.height - 1)
                            TrySpawnObject(Object.Door2, currentPosition, Side.Left, parent);
                        else
                            TrySpawnObject(Object.Floor, currentPosition, Side.Bottom, parent);
                        
                        doorPositions.Add(new Vector3(currentPosition.x, 0, currentPosition.y) + new Vector3(.5f, 0, .5f));
                    }
                }
            }
        }

        for (int i = 0; i < generator.rooms.Count; i++)
        {
            RectInt currentRoom = generator.rooms[i];
            Transform roomParent = new GameObject("Room x: " + currentRoom.x + " y: " + currentRoom.y).transform;
            roomParent.parent = parent;
            objects.Add(roomParent);

            // Walls
            // Bottom
            for (int pos = 1; pos < currentRoom.width - 1; pos++)
                TrySpawnObject(Object.Wall, new Vector2(currentRoom.x + pos, currentRoom.y), Side.Bottom, roomParent);

            // Left
            for (int pos = 1; pos < currentRoom.height - 1; pos++)
                TrySpawnObject(Object.Wall, new Vector2(currentRoom.x, currentRoom.y + pos), Side.Left, roomParent);

            // Top
            for (int pos = 1; pos < currentRoom.width - 1; pos++)
                TrySpawnObject(Object.Wall, new Vector2(currentRoom.x + pos, currentRoom.y + currentRoom.height - 1), Side.Top, roomParent);

            // Right
            for (int pos = 1; pos < currentRoom.height - 1; pos++)
                TrySpawnObject(Object.Wall, new Vector2(currentRoom.x + currentRoom.width -1, currentRoom.y + pos), Side.Right, roomParent);

            // Corners
            // A B
            // C D

            // A
            TrySpawnObject(Object.CornerWall, new Vector2(currentRoom.x, currentRoom.y + currentRoom.height - 1), Side.Left, roomParent);
            // B
            TrySpawnObject(Object.CornerWall, new Vector2(currentRoom.x + currentRoom.width - 1, currentRoom.y + currentRoom.height - 1), Side.Top, roomParent);
            // C
            TrySpawnObject(Object.CornerWall, new Vector2(currentRoom.x, currentRoom.y), Side.Bottom, roomParent);
            // D
            TrySpawnObject(Object.CornerWall, new Vector2(currentRoom.x + currentRoom.width - 1, currentRoom.y), Side.Right, roomParent);
        }
        }
        else if (objectSpawnType == ObjectSpawnType.MarchingSquares)
        {
            
            int[,] tileMap = new int[generator.boundary.height, generator.boundary.width];
            int rows = tileMap.GetLength(0);
            int cols = tileMap.GetLength(1);

            List<RectInt> rooms = generator.rooms;

            //Fill the map with empty spaces
            foreach (RectInt room in rooms) {
                AlgorithmsUtils.FillRectangleOutline(tileMap, room, 1);
            }
            foreach (RectInt door in generator.doors) {
                AlgorithmsUtils.FillRectangle(tileMap, door, 0);
            }

            //Draw the rooms
            for (int row = 0; row < rows - 1; row++)
            {
                for (int col = 0; col < cols - 1; col++)
                {
                    int a = tileMap[row + 1, col];
                    int b = tileMap[row + 1, col + 1];
                    int c = tileMap[row, col + 1];
                    int d = tileMap[row, col];
                
                    int ID = a * 1 + b * 2 + c * 4 + d * 8;

                    if (ID == 0)
                    {
                        Vector2Int fromNode = new Vector2Int(col, row);
                    
                        // Top
                        if (row + 1 < generator.boundary.height)
                            floodfill.AddEdge(fromNode, new Vector2Int(col, row + 1));
                    
                        // Right
                        if (col + 1 < generator.boundary.height)
                            floodfill.AddEdge(fromNode, new Vector2Int(col + 1, row));
                    
                        // Bottem
                        if (row - 1 < generator.boundary.height)
                            floodfill.AddEdge(fromNode, new Vector2Int(col, row - 1));
                    
                        // Left
                        if (col - 1 < generator.boundary.height)
                            floodfill.AddEdge(fromNode, new Vector2Int(col - 1, row));
                        continue;
                    }
                    
                    Transform prefab = Instantiate(indexes[ID]);
                    prefab.position = new Vector3(col + .5f, 0, row + .5f);
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
        // Floor
        if (floorFillType == FloorFillType.Simple)
        {
            for (int i = 0; i < generator.rooms.Count; i++)
            {
                RectInt currentRoom = generator.rooms[i];
                Transform roomParent = new GameObject("Room x: " + currentRoom.x + " y: " + currentRoom.y).transform;
                roomParent.parent = parent;
                objects.Add(roomParent);
                
                for (int x = 1; x < currentRoom.width - 1; x++)
                {
                    for (int y = 1; y < currentRoom.height - 1; y++)
                    {
                        Vector2 currentPosition = new Vector2(x + currentRoom.x, y + currentRoom.y);
                        TrySpawnObject(Object.Floor, currentPosition, Side.Bottom, roomParent);
                    }
                }
            }   
        }
        else if (floorFillType == FloorFillType.FloodFill)
        {
            Vector2Int startNode = generator.rooms[0].position + generator.rooms[0].size/2;
            
            // Instantiate start floor tile
            Transform startInstance = Instantiate(prefabs[Object.Floor]);
            startInstance.position = new Vector3(startNode.x + 0.5f, 0, startNode.y + 0.5f);
            
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
        
            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                Vector2Int node = queue.Dequeue();
                visited.Add(node);

                foreach (Vector2Int edgeNode in floodfill.GetNeighbors(node))
                {
                    if (!visited.Contains(edgeNode))
                    {
                        queue.Enqueue(edgeNode);
                        visited.Add(edgeNode);
                        
                        // Instantiate floor
                        Transform instance = Instantiate(prefabs[Object.Floor]);
                        Vector3 position = new Vector3(edgeNode.x + .5f, 0, edgeNode.y + .5f);
                        // Vector3 position = new Vector3(node.x + .5f, 0, node.y + .5f);
                        instance.position = position;
                    }
                }
            }

            /*
            foreach (RectInt door in generator.doors)
            {
                // Instantiate floor
                Transform instance = Instantiate(prefabs[Object.Floor]);
                        
                Vector3 position = new Vector3(door.x + door.width/2, 0, door.y + door.height/2);
                position += new Vector3(-.5f, 0, -.5f);
                        
                instance.position = position;
                instance.localScale = new Vector3(door.width, 1, door.height);
            }
            
            
            
            
            
            foreach (RectInt room in generator.dungeon.GetNodes())
            {
                Vector3 roomPos = new Vector3(room.x + room.width / 2, Time.deltaTime, room.y + room.height / 2);
                DebugExtension.DebugCircle(roomPos, Color.green, 3, Time.deltaTime, false);

                foreach (RectInt neighbor in generator.dungeon.GetNeighbors(room))
                {
                    Vector3 neighborPos = new Vector3(neighbor.x + neighbor.width / 2, 0, neighbor.y + neighbor.height / 2);
                    Debug.DrawLine(roomPos, neighborPos, Color.green, Time.deltaTime, false);
                }
            }
            */
        }
    }
}
