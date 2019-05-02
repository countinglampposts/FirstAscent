using System.Collections;
using System.Collections.Generic;
using TerraGen.Generator;
using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public class PerlinLayerData : ITerrainLayer
    {
        public float frequency = .0005f;
        public float amplitude = 1000f;
        public int octaves = 5;
        public int seed;

        public float ApplyLayer(float x, float y, float height)
        {
            var rnd = new System.Random(seed);
            for (float o = 1; o <= octaves; o++)
            {
                var usedFrequency = frequency * o;
                var usedAmplitude = amplitude / o;
                var offset = new Vector2(rnd.Next(1000), rnd.Next(1000)) / 1000f;

                height += Mathf.PerlinNoise(x * usedFrequency + offset.x, y * usedFrequency + offset.y) * usedAmplitude;
            }

            return height;
        }
    }
}