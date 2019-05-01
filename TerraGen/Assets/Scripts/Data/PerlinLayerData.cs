using System.Collections;
using System.Collections.Generic;
using TerraGen.Generator;
using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public class PerlinLayerData : ITerrainLayer
    {
        public float frequency = .001f;
        public float amplitude = 1000f;
        public int octaves = 5;

        public float ApplyLayer(float x, float y, float height)
        {
            for (float o = 1; o <= octaves; o++)
            {
                var usedFrequency = frequency * o;
                var usedAmplitude = amplitude / o;

                height += Mathf.PerlinNoise(x * usedFrequency, y * usedFrequency) * usedAmplitude;
            }

            return height;
        }
    }
}