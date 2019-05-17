using System.Collections;
using System.Collections.Generic;
using TerraGen.Data;
using UnityEngine;

namespace TerraGen.Generator
{
    public class LatticeGrid : MonoBehaviour
    {
        public int seed;
        public LatticeData latticeParams;

        public int GridPointToIndex(int x, int y)
        {
            return x + y * latticeParams.gridSize;
        }

        public void ResetPoints()
        {
            latticeParams.points = new Vector2[latticeParams.gridSize * latticeParams.gridSize];
            for (int x = 0; x < latticeParams.gridSize; x++)
            {
                for (int y = 0; y < latticeParams.gridSize; y++)
                {
                    var worldX = Mathf.Lerp(0, latticeParams.gridWorldSize, (float)x / latticeParams.gridSize);
                    var worldY = Mathf.Lerp(0, latticeParams.gridWorldSize, (float)y / latticeParams.gridSize);
                    latticeParams.points[GridPointToIndex(x, y)] = new Vector2(worldX, worldY);
                }
            }
        }

        public void Randomize()
        {
            ResetPoints();
            var randomRange = Mathf.RoundToInt(latticeParams.gridWorldSize / latticeParams.gridSize) / 2;
            var rnd = new System.Random(seed);
            for (int x = 0; x < latticeParams.gridSize; x++)
            {
                for (int y = 0; y < latticeParams.gridSize; y++)
                {
                    latticeParams.points[GridPointToIndex(x, y)] += new Vector2(rnd.Next(-randomRange, randomRange), rnd.Next(-randomRange, randomRange));
                }
            }
        }
    }
}