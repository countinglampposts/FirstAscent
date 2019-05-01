using TerraGen.Generator;
using UnityEngine;
using TerrainData = TerraGen.Data.TerrainData;
using UniRx;
using TerraGen.Data;

namespace TerraGen.Test
{
    public class TerrainTest : MonoBehaviour
    {
        [SerializeField] private int lod;
        [SerializeField] private int gridSize = 100;
        [SerializeField] private float seaLevel;
        [SerializeField] private PerlinLayerData perlinData;
        [SerializeField] private PointLayerData pointLayerData;
        [SerializeField] private FlatLayerData flatLayerData;

        [SerializeField] private MeshFilter meshFilter;

        public void GenerateTerrain()
        {
            var lodMultiplier = Mathf.Pow(10, lod);

            float[][] pointData = new float[gridSize + 1][];

            for (int x = 0; x < pointData.Length; x++)
            {
                pointData[x] = new float[gridSize + 1];
                for (int y = 0; y < pointData[x].Length; y++)
                {
                    float globalX = x * lodMultiplier;
                    float globalY = y * lodMultiplier;

                    pointData[x][y] = perlinData.ApplyLayer(globalX, globalY, pointData[x][y]);
                    pointData[x][y] = pointLayerData.ApplyLayer(globalX, globalY, pointData[x][y]);
                    pointData[x][y] = flatLayerData.ApplyLayer(globalX, globalY, pointData[x][y]);
                }
            }

            TerrainData terrainData = new TerrainData
            {
                lod = lod,
                pointData = pointData
            };

            var factory = new TerrainMeshFactory();
            factory.GenerateTerrainMesh(terrainData)
                .Subscribe(mesh => meshFilter.mesh = mesh);
        }
    }
}