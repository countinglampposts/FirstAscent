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

        [SerializeField] private int meshSize = 256;
        [SerializeField] private float maxHeight = 8000f;
        [SerializeField] private float globalScale = 1f;
        [SerializeField] private LatticeGrid latticeGrid;
        [SerializeField] private MutatorData mutatorData;
        [SerializeField] private PerlinData perlinData;
        [SerializeField] private FalloffData falloffData;

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
            latticeGrid.latticeParams.ApplyToShader(shaderParams, terrainComputeShader)
                .AddTo(shaderDisposables);
            falloffData.ApplyToShader(shaderParams, terrainComputeShader)
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

            terrainComputeShader.Dispatch(terrainComputeShader.FindKernel("Generate"), pointData.data.Length, 1, 1);

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

            finalPassComputeShader.Dispatch(0, pointData.data.Length, 1, 1);

            finalMapBuffer.GetData(pointData.data);

            shaderDisposables.Dispose();

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