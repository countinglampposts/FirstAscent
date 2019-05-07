using System.Collections;
using System.Collections.Generic;
using TerraGen.Generator;
using UnityEngine;

namespace TerraGen.Generator
{
    public class NormalizeLayer : ISecondPassFilter
    {
        [SerializeField] float maxY;

        public float[,] ApplyLayer(float[,] terrainData)
        {
            var min = float.MaxValue;
            var max = float.MinValue;
            foreach (var point in terrainData)
            {
                min = Mathf.Min(min, point);
                max = Mathf.Max(max, point);
            }

            for (int x = 0; x < terrainData.GetLength(0); x++)
            {
                for (int y = 0; y < terrainData.GetLength(1); y++)
                {
                    terrainData[x, y] = Mathf.InverseLerp(min, max, terrainData[x, y]) * maxY;
                }
            }

            return terrainData;
        }
    }
}