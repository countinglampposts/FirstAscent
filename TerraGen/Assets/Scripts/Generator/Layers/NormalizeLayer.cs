using System.Collections;
using System.Collections.Generic;
using TerraGen.Data;
using TerraGen.Generator;
using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class NormalizeLayer : ISecondPassFilter
    {
        [SerializeField] float maxY;

        public TerrainPointData ApplyLayer(TerrainPointData terrainData, MutatorParams mutatorParams)
        {
            var min = float.MaxValue;
            var max = float.MinValue;
            foreach (var point in terrainData.data)
            {
                min = Mathf.Min(min, point);
                max = Mathf.Max(max, point);
            }

            for (int x = 0; x < terrainData.mapSize; x++)
            {
                for (int y = 0; y < terrainData.mapSize; y++)
                {
                    terrainData.data[y * terrainData.mapSize + x] = Mathf.InverseLerp(min, max, terrainData.data[y * terrainData.mapSize + x]) * maxY;
                }
            }

            return terrainData;
        }
    }
}