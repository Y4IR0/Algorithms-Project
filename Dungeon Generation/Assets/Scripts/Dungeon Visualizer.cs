using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using AYellowpaper.SerializedCollections;

public class DungeonVisualizer : MonoBehaviour
{
    public enum Object
    {
        Wall,
        CornerWall
    }


    public enum Side
    {
        Bottom,
        Left,
        Top,
        Right,
    }
    
    [SerializeField]
    DungeonGenerator dungeonGenerator;
    
    [SerializedDictionary("Object", "Prefab")]
    public SerializedDictionary<Object, Transform> prefabs = new SerializedDictionary<Object, Transform>();
    private HashSet<Transform> objects = new HashSet<Transform>();
    
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
            foreach (Transform wall in objects)
            {
                if (spawnPosition == wall.position)
                    canSpawn = false;
            }
            
            // Checks if position in door
            foreach (RectInt door in dungeonGenerator.doors)
            {
                Vector3 doorPosition = new Vector3(door.x + .5f, 0, door.y + .5f);
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
        
        for (int i = 0; i < dungeonGenerator.rooms.Count; i++)
        {
            RectInt currentRoom = dungeonGenerator.rooms[i];
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
}
