using System.Collections;
using System.Collections.Generic;
using TerraGen.Generator;
using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public class PerlinLayerData : ITerrainLayer
    {
        [Range(0f, 1f)]
        public float persistance = .5f;
        [Range(1f, 10f)]
        public float lacunarity = 2f;
        public int octaves = 5;
        public int seed;

        public float ApplyLayer(float x, float y, float height)
        {
            var rnd = new System.Random(seed);

            var frequency = 1f;
            var amplitude = 1f;

            for (float o = 1; o <= octaves; o++)
            {
                var offset = new Vector2(rnd.Next(1000), rnd.Next(1000)) / 1000f;

                var sampleX = x * frequency + offset.x;
                var sampleY = y * frequency + offset.y;

                var noiseValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

                height += noiseValue * amplitude;

                amplitude *= persistance;
                frequency *= lacunarity;
            }

            return height;
        }
    }
}