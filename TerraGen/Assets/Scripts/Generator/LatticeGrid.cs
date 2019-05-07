using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public class LatticeGrid : MonoBehaviour
    {
        public int gridSize;
        public float gridWorldSize;
        public Vector2[] points;

        public int GridPointToIndex(int x, int y)
        {
            return x + y * gridSize;
        }
    }
}