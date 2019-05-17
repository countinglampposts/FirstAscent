using TerraGen.Data;
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

        public TerrainPointData ApplyLayer(TerrainPointData terrainData, MutatorParams mutatorParams)
        {
            var points = GeneratePerlin(terrainData, mutatorParams);
            terrainData.data = points;
            return terrainData;
        }

        float[] GeneratePerlin(TerrainPointData terrainData, MutatorParams mutatorParams)
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

            float[] map = terrainData.data;

            ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(int));
            mapBuffer.SetData(map);
            heightMapComputeShader.SetBuffer(0, "heightMap", mapBuffer);

            var mutatorParamsArray = new MutatorParams[] { mutatorParams };
            ComputeBuffer mutatorBuffer = new ComputeBuffer(1, sizeof(int) + sizeof(float) * 3);
            mutatorBuffer.SetData(mutatorParamsArray);
            heightMapComputeShader.SetBuffer(0, "mutatorParams", mutatorBuffer);

            //TODO: Find a way to set the mutator params
            heightMapComputeShader.SetInt("mapSize", terrainData.mapSize);
            heightMapComputeShader.SetInt("octaves", octaves);
            heightMapComputeShader.SetFloat("lacunarity", lacunarity);
            heightMapComputeShader.SetFloat("persistence", persistance);
            heightMapComputeShader.SetFloat("scaleFactor", scale);

            heightMapComputeShader.Dispatch(0, map.Length, 1, 1);

            mapBuffer.GetData(map);
            mapBuffer.Release();
            offsetsBuffer.Release();
            mutatorBuffer.Release();

            return map;
        }
    }
}