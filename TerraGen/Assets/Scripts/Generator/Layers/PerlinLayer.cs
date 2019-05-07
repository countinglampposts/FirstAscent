using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class PerlinLayer : IFirstPassFilter
    {
        [SerializeField]
        float frequency = 1f;
        [Range(0f, 1f)]
        [SerializeField]
        float persistance = .5f;
        [Range(1f, 10f)]
        [SerializeField]
        float lacunarity = 2f;
        [SerializeField]
        int octaves = 5;
        [SerializeField]
        int seed;

        public float ApplyLayer(float x, float y, float height)
        {
            var rnd = new System.Random(seed);

            var frequency = this.frequency;
            var amplitude = 1f;

            for (float o = 1; o <= octaves; o++)
            {
                var offset = new Vector2(rnd.Next(1000), rnd.Next(1000)) / 1000f;

                var sampleX = x * frequency + offset.x;
                var sampleY = y * frequency + offset.y;

                var noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

                height += noiseValue * amplitude;

                amplitude *= persistance;
                frequency *= lacunarity;
            }

            return height;
        }
    }
}