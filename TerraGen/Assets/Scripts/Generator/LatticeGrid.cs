using System.Collections;
using System.Collections.Generic;
using TerraGen.Data;
using UnityEngine;

namespace TerraGen.Generator
{
    public class LatticeGrid : MonoBehaviour
    {
        public int seed;
        public LatticeData latticeData;

        public int GridPointToIndex(int x, int y)
        {
            return x + y * latticeData.gridSize;
        }

        public void ResetPoints()
        {
            var gridInterval = (int)Mathf.Pow(2f, (float)latticeData.gridLOD);

            latticeData.points = new Vector2[latticeData.gridSize * latticeData.gridSize];
            for (int x = 0; x < latticeData.gridSize; x++)
            {
                for (int y = 0; y < latticeData.gridSize; y++)
                {
                    latticeData.points[GridPointToIndex(x, y)] = new Vector2(gridInterval * x, gridInterval * y);
                }
            }
        }

        public void Randomize()
        {
            ResetPoints();
            var gridInterval = (int)Mathf.Pow(2f, (float)latticeData.gridLOD) / 2;
            var rnd = new System.Random(seed);
            for (int x = 0; x < latticeData.gridSize; x++)
            {
                for (int y = 0; y < latticeData.gridSize; y++)
                {
                    latticeData.points[GridPointToIndex(x, y)] += new Vector2(rnd.Next(-gridInterval, gridInterval), rnd.Next(-gridInterval, gridInterval));
                }
            }
        }
    }
}