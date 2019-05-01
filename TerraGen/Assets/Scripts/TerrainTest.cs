using TerraGen.Generator;
using UnityEngine;
using TerrainData = TerraGen.Data.TerrainData;
using UniRx;

namespace TerraGen.Test
{
    public class TerrainTest : MonoBehaviour
    {
        [SerializeField] private int lod;
        [SerializeField] private int gridSize = 100;
        [SerializeField] private float frequency = 10f;
        [SerializeField] private float amplitude = 20f;
        [SerializeField] private int octives = 1;
        [SerializeField] private float seaLevel;
        [SerializeField] private float mountainRadius;
        [SerializeField] private Vector2 mountainCenter;

        [SerializeField] private MeshFilter meshFilter;

        public void GenerateTerrain()
        {
            var factory = new TerrainMeshFactory();
            var lodMultiplier = Mathf.Pow(10, lod);

            float[][] pointData = new float[gridSize + 1][];
            for (int x = 0; x < pointData.Length; x++)
            {
                pointData[x] = new float[gridSize + 1];
                for (int y = 0; y < pointData[x].Length; y++)
                {
                    float globalX = x * lodMultiplier;
                    float globalY = y * lodMultiplier;
                    for (float o = 1; o <= octives; o++)
                    {
                        var usedFrequency = frequency * o;
                        var usedAmplitude = amplitude / o;

                        pointData[x][y] += Mathf.PerlinNoise(globalX * usedFrequency, globalY * usedFrequency) * usedAmplitude;
                    }

                    pointData[x][y] *= Mathf.Clamp01(mountainRadius / Vector2.Distance(new Vector2(globalX, globalY), mountainCenter));

                    pointData[x][y] = Mathf.Clamp(pointData[x][y], seaLevel, Mathf.Infinity);
                }
            }

            TerrainData terrainData = new TerrainData
            {
                lod = lod,
                pointData = pointData
            };

            factory.GenerateTerrainMesh(terrainData)
                .Subscribe(mesh => meshFilter.mesh = mesh);
        }
    }
}