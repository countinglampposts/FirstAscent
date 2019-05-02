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
        [SerializeField] private int gridSize = 256;
        [SerializeField] private float scale = 1f;
        [SerializeField] private PerlinLayerData perlinLayerData;
        [SerializeField] private PointLayerData pointLayerData;
        [SerializeField] private FlatLayerData flatLayerData;

        [SerializeField] private MeshFilter meshFilter;

        public void GenerateTerrain()
        {
            var lodMultiplier = Mathf.Pow(2, lod);

            float[][] pointData = new float[gridSize + 1][];

            for (int x = 0; x < pointData.Length; x++)
            {
                pointData[x] = new float[gridSize + 1];
                for (int y = 0; y < pointData[x].Length; y++)
                {
                    float globalX = (x * lodMultiplier + transform.position.x) / scale;
                    float globalY = (y * lodMultiplier + transform.position.z) / scale;

                    pointData[x][y] = perlinLayerData.ApplyLayer(globalX, globalY, pointData[x][y]);
                    pointData[x][y] = pointLayerData.ApplyLayer(globalX, globalY, pointData[x][y]);
                    pointData[x][y] = flatLayerData.ApplyLayer(globalX, globalY, pointData[x][y]);
                    pointData[x][y] *= scale;
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