using TerraGen.Generator;
using UnityEngine;
using TerrainMeshData = TerraGen.Data.TerrainMeshData;
using UniRx;
using TerraGen.Data;

namespace TerraGen.Test
{
    public class TerrainTest : MonoBehaviour
    {
        [SerializeField] private MutatorParams mutatorParams;
        [SerializeField] private int meshSize = 256;
        [SerializeField] private LatticeLayer latticeLayer;
        [SerializeField] private PerlinLayer perlinLayer;
        [SerializeField] private FalloffLayer falloffLayer;
        [SerializeField] private FlatLayer flatLayer;
        [SerializeField] private NormalizeLayer normalizeLayer;

        [SerializeField] private MeshFilter meshFilter;

        public void GenerateTerrain()
        {
            mutatorParams.position = new Vector2(transform.position.x, transform.position.z);
            mutatorParams.latticeParams = latticeLayer.GetLatticeParams();

            var lodMultiplier = Mathf.Pow(2, mutatorParams.lod);
            var mapSize = meshSize + 1;

            TerrainPointData pointData = new TerrainPointData
            {
                data = new float[mapSize * mapSize],
                mapSize = mapSize
            };

            pointData = perlinLayer.ApplyLayer(pointData, mutatorParams);

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    var globalPosition = new Vector2(x, y);
                    globalPosition *= lodMultiplier;
                    globalPosition += mutatorParams.position;
                    globalPosition = latticeLayer.Mutate(globalPosition);
                    globalPosition /= mutatorParams.scale;

                    //pointData.data[y * mapSize + x] = perlinLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * mapSize + x]);
                    pointData.data[y * mapSize + x] = falloffLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * mapSize + x]);
                    pointData.data[y * mapSize + x] = flatLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * mapSize + x]);
                }
            }

            pointData = normalizeLayer.ApplyLayer(pointData, mutatorParams);

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    pointData.data[y * mapSize + x] *= mutatorParams.scale;
                }
            }

            TerrainMeshData terrainData = new TerrainMeshData
            {
                lod = mutatorParams.lod,
                pointData = pointData
            };

            var factory = new TerrainMeshFactory();
            factory.GenerateTerrainMesh(terrainData)
                .Subscribe(mesh => meshFilter.mesh = mesh);
        }
    }
}