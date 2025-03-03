using System;
using UnityEngine;

public class RectIntTest : MonoBehaviour
{
    [SerializeField] RectInt rectInt;

    void Update()
    {
        AlgorithmsUtils.DebugRectInt(rectInt, Color.blue, .01f, false, .01f);
    }
}
