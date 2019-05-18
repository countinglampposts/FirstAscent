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
        [SerializeField] private ComputeShader finalPassComputeShader;

        [SerializeField] private MutatorData mutatorData;
        [SerializeField] private int meshSize = 256;
        [SerializeField] private float maxHeight = 8000f;
        [SerializeField] private float globalScale = 1f;
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

            var shaderParams = new ShaderLayerParams
            {
                mapSize = mapSize,
                globalScale = globalScale
            };
            perlinData.ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);
            mutatorData.ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);
            latticeLayer.GetLatticeData().ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);
            falloffLayer.ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);

            ComputeBuffer mapBuffer = new ComputeBuffer(pointData.data.Length, sizeof(float));
            mapBuffer.SetData(pointData.data);
            terrainComputeShader.SetBuffer(0, "heightMap", mapBuffer);
            Disposable.Create(mapBuffer.Release)
                .AddTo(shaderDisposables);

            terrainComputeShader.SetInt("mapSize", pointData.mapSize);

            var minMax = new int[] { int.MaxValue, 0 };
            ComputeBuffer minMaxBuffer = new ComputeBuffer(minMax.Length, sizeof(int));
            minMaxBuffer.SetData(minMax);
            terrainComputeShader.SetBuffer(0, "minMax", minMaxBuffer);
            Disposable.Create(minMaxBuffer.Release)
                .AddTo(shaderDisposables);

            terrainComputeShader.Dispatch(terrainComputeShader.FindKernel("Generate"), pointData.data.Length / 1024, 1, 1);

            mapBuffer.GetData(pointData.data);
            minMaxBuffer.GetData(minMax);

            ComputeBuffer finalMinMaxBuffer = new ComputeBuffer(minMax.Length, sizeof(int));
            finalMinMaxBuffer.SetData(minMax);
            finalPassComputeShader.SetBuffer(0, "lastPass_MinMax", finalMinMaxBuffer);
            Disposable.Create(finalMinMaxBuffer.Release)
                .AddTo(shaderDisposables);

            ComputeBuffer finalMapBuffer = new ComputeBuffer(pointData.data.Length, sizeof(float));
            finalMapBuffer.SetData(pointData.data);
            finalPassComputeShader.SetBuffer(0, "lastPass_HeightMap", finalMapBuffer);
            Disposable.Create(finalMapBuffer.Release)
                .AddTo(shaderDisposables);

            finalPassComputeShader.SetFloat("lastPass_MaxHeight", maxHeight);

            finalPassComputeShader.SetFloat("lastPass_GlobalScale", globalScale);

            finalPassComputeShader.Dispatch(0, pointData.data.Length / 1024, 1, 1);

            finalMapBuffer.GetData(pointData.data);

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
            }

            pointData = normalizeLayer.ApplyLayer(pointData, mutatorData);

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    pointData.data[y * mapSize + x] *= globalScale;
                }
            }*/

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