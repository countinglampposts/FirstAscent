using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public class LatticeGrid : MonoBehaviour
    {
        public int gridSize;
        public float gridWorldSize;
        public int seed;
        [HideInInspector]
        public Vector2[] points;

        public int GridPointToIndex(int x, int y)
        {
            return x + y * gridSize;
        }

        public void ResetPoints()
        {
            points = new Vector2[gridSize * gridSize];
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    var worldX = Mathf.Lerp(0, gridWorldSize, (float)x / gridSize);
                    var worldY = Mathf.Lerp(0, gridWorldSize, (float)y / gridSize);
                    points[GridPointToIndex(x, y)] = new Vector2(worldX, worldY);
                }
            }
        }

        public void Randomize()
        {
            ResetPoints();
            var randomRange = Mathf.RoundToInt(gridWorldSize / gridSize) / 2;
            var rnd = new System.Random(seed);
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    points[GridPointToIndex(x, y)] += new Vector2(rnd.Next(-randomRange, randomRange), rnd.Next(-randomRange, randomRange));
                }
            }
        }
    }
}