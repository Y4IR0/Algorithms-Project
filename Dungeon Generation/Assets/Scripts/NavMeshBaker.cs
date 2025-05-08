using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface navMesh;

    [Button]
    public void GenerateNavMesh()
    {
        navMesh.BuildNavMesh();
    }
}
