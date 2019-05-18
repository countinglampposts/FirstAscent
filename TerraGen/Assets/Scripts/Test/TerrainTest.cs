using TerraGen.Generator;
using UnityEngine;
using TerrainMeshData = TerraGen.Data.TerrainMeshData;
using UniRx;
using TerraGen.Data;

namespace TerraGen.Test
{
    public class TerrainTest : MonoBehaviour
    {
        [SerializeField] private ComputeShader terrainComputeShader;

        [SerializeField] private MutatorData mutatorData;
        [SerializeField] private int meshSize = 256;
        [SerializeField] private LatticeLayer latticeLayer;
        [SerializeField] private PerlinData perlinData;
        [SerializeField] private FalloffLayer falloffLayer;
        [SerializeField] private FlatLayer flatLayer;
        [SerializeField] private NormalizeLayer normalizeLayer;

        [SerializeField] private MeshFilter meshFilter;

        public void GenerateTerrain()
        {
            mutatorData.position = new Vector2(transform.position.x, transform.position.z);

            var lodMultiplier = Mathf.Pow(2, mutatorData.lod);
            var mapSize = meshSize + 1;

            TerrainPointData pointData = new TerrainPointData
            {
                data = new float[mapSize * mapSize],
                mapSize = mapSize
            };

            CompositeDisposable shaderDisposables = new CompositeDisposable();

            var shaderParams = new ShaderLayerParams { mapSize = mapSize };
            perlinData.ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);
            mutatorData.ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);
            latticeLayer.GetLatticeData().ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);
            falloffLayer.ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);

            ComputeBuffer mapBuffer = new ComputeBuffer(pointData.data.Length, sizeof(int));
            mapBuffer.SetData(pointData.data);
            terrainComputeShader.SetBuffer(0, "heightMap", mapBuffer);

            terrainComputeShader.SetInt("mapSize", pointData.mapSize);

            terrainComputeShader.Dispatch(0, pointData.data.Length, 1, 1);

            mapBuffer.GetData(pointData.data);

            shaderDisposables.Dispose();

            /*for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    var globalPosition = new Vector2(x, y);
                    globalPosition *= lodMultiplier;
                    globalPosition += mutatorData.position;
                    globalPosition = latticeLayer.Mutate(globalPosition);
                    globalPosition /= mutatorData.scale;

                    //pointData.data[y * mapSize + x] = perlinLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * mapSize + x]);
                    pointData.data[y * mapSize + x] = falloffLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * mapSize + x]);
                    pointData.data[y * mapSize + x] = flatLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData.data[y * mapSize + x]);
                }
            }*/

            pointData = normalizeLayer.ApplyLayer(pointData, mutatorData);

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    pointData.data[y * mapSize + x] *= mutatorData.scale;
                }
            }

            TerrainMeshData terrainData = new TerrainMeshData
            {
                lod = mutatorData.lod,
                pointData = pointData
            };

            var factory = new TerrainMeshFactory();
            factory.GenerateTerrainMesh(terrainData)
                .Subscribe(mesh => meshFilter.mesh = mesh);
        }
    }
}