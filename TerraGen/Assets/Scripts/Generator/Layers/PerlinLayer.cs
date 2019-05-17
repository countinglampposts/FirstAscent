using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class PerlinLayer : ISecondPassFilter, IFirstPassFilter
    {
        [SerializeField]
        ComputeShader heightMapComputeShader;
        [SerializeField]
        float scale = 1f;
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

            var frequency = 1f;
            var amplitude = 1f;

            for (float o = 1; o <= octaves; o++)
            {
                var offset = new Vector2(rnd.Next(1000), rnd.Next(1000)) / 1000f;

                var sampleX = (x * frequency + offset.x) / scale;
                var sampleY = (y * frequency + offset.y) / scale;

                var noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

                height += noiseValue * amplitude;

                amplitude = Mathf.Pow(persistance, o);
                frequency = Mathf.Pow(lacunarity, o);
            }

            return height * scale;
        }

        public float[,] ApplyLayer(float[,] terrainData)
        {
            throw new System.NotImplementedException();
        }

        float[] GenerateHeightMapGPU(int mapSize)
        {
            var prng = new System.Random(seed);

            Vector2[] offsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                offsets[i] = new Vector2(prng.Next(-10000, 10000), prng.Next(-10000, 10000));
            }
            ComputeBuffer offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 2);
            offsetsBuffer.SetData(offsets);
            heightMapComputeShader.SetBuffer(0, "offsets", offsetsBuffer);

            int floatToIntMultiplier = 1000;
            float[] map = new float[mapSize * mapSize];

            ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(int));
            mapBuffer.SetData(map);
            heightMapComputeShader.SetBuffer(0, "heightMap", mapBuffer);

            int[] minMaxHeight = { floatToIntMultiplier * octaves, 0 };
            ComputeBuffer minMaxBuffer = new ComputeBuffer(minMaxHeight.Length, sizeof(int));
            minMaxBuffer.SetData(minMaxHeight);
            heightMapComputeShader.SetBuffer(0, "minMax", minMaxBuffer);

            heightMapComputeShader.SetInt("mapSize", mapSize);
            heightMapComputeShader.SetInt("octaves", octaves);
            heightMapComputeShader.SetFloat("lacunarity", lacunarity);
            heightMapComputeShader.SetFloat("persistence", persistance);
            heightMapComputeShader.SetFloat("scaleFactor", scale);
            heightMapComputeShader.SetInt("floatToIntMultiplier", floatToIntMultiplier);

            heightMapComputeShader.Dispatch(0, map.Length, 1, 1);

            mapBuffer.GetData(map);
            minMaxBuffer.GetData(minMaxHeight);
            mapBuffer.Release();
            minMaxBuffer.Release();
            offsetsBuffer.Release();

            float minValue = (float)minMaxHeight[0] / (float)floatToIntMultiplier;
            float maxValue = (float)minMaxHeight[1] / (float)floatToIntMultiplier;

            for (int i = 0; i < map.Length; i++)
            {
                map[i] = Mathf.InverseLerp(minValue, maxValue, map[i]);
            }

            return map;
        }
    }
}