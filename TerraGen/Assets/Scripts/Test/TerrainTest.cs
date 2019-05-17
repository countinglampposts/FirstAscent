using TerraGen.Generator;
using UnityEngine;
using TerrainMeshData = TerraGen.Data.TerrainMeshData;
using UniRx;
using TerraGen.Data;

namespace TerraGen.Test
{
    public class TerrainTest : MonoBehaviour
    {
        [SerializeField] private int lod;
        [SerializeField] private int gridSize = 256;
        [SerializeField] private float scale = 1f;
        [SerializeField] private LatticeLayer latticeLayer;
        [SerializeField] private PerlinLayer perlinLayer;
        [SerializeField] private FalloffLayer falloffLayer;
        [SerializeField] private FlatLayer flatLayer;
        [SerializeField] private NormalizeLayer normalizeLayer;

        [SerializeField] private MeshFilter meshFilter;

        public void GenerateTerrain()
        {
            var lodMultiplier = Mathf.Pow(2, lod);
            var mapSize = gridSize + 1;

            TerrainPointData pointData = new TerrainPointData
            {
                data = new float[mapSize * mapSize],
                mapSize = mapSize
            };

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    var globalPosition = new Vector2(x, y);
                    globalPosition *= lodMultiplier;
                    globalPosition += new Vector2(transform.position.x, transform.position.z);
                    globalPosition = latticeLayer.Mutate(globalPosition);
                    globalPosition /= scale;

                    pointData.data[y * gridSize + x] = perlinLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * gridSize + x]);
                    pointData.data[y * gridSize + x] = falloffLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * gridSize + x]);
                    pointData.data[y * gridSize + x] = flatLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * gridSize + x]);
                }
            }

            pointData = normalizeLayer.ApplyLayer(pointData);

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    pointData.data[y * gridSize + x] *= scale;
                }
            }

            TerrainMeshData terrainData = new TerrainMeshData
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