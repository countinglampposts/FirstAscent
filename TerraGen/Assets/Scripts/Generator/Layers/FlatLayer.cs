using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class FlatLayer : ISecondPassFilter
    {
        [SerializeField]
        float altitude;

        public float[,] ApplyLayer(float[,] terrainData)
        {
            for (int x = 0; x < terrainData.GetLength(0); x++)
            {
                for (int y = 0; y < terrainData.GetLength(1); y++)
                {
                    terrainData[x, y] = Mathf.Max(terrainData[x, y], altitude);
                }
            }
            return terrainData;
        }
    }
}