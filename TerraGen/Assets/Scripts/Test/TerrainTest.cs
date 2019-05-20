using TerraGen.Generator;
using UnityEngine;
using TerrainMeshData = TerraGen.Data.TerrainMeshData;
using UniRx;
using TerraGen.Data;
using System;

namespace TerraGen.Test
{
    public class TerrainTest : MonoBehaviour
    {
        [SerializeField] private ComputeShader terrainComputeShader;
        [SerializeField] private ComputeShader finalPassComputeShader;

        [SerializeField] private int mapSize = 256;
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

            TerrainPointData pointData = new TerrainPointData
            {
                data = new float[mapSize * mapSize],
                mapSize = mapSize
            };

            var minMax = new int[] { int.MaxValue, 0 };

            CompositeDisposable shaderDisposables = new CompositeDisposable();

            var shaderParams = new ShaderLayerParams
            {
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

            RunHeightmapShader(ref minMax, ref pointData.data, mapSize)
                .AddTo(shaderDisposables);

            RunFinalPass(minMax, ref pointData.data)
                .AddTo(shaderDisposables);

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

        private IDisposable RunHeightmapShader(ref int[] minMax, ref float[] heightMap, int mapSize)
        {
            var disposables = new CompositeDisposable();

            ComputeBuffer mapBuffer = new ComputeBuffer(heightMap.Length, sizeof(float));
            mapBuffer.SetData(heightMap);
            terrainComputeShader.SetBuffer(0, "heightMap", mapBuffer);
            Disposable.Create(mapBuffer.Release)
                .AddTo(disposables);

            terrainComputeShader.SetInt("mapSize", mapSize);

            ComputeBuffer minMaxBuffer = new ComputeBuffer(minMax.Length, sizeof(int));
            minMaxBuffer.SetData(minMax);
            terrainComputeShader.SetBuffer(0, "minMax", minMaxBuffer);
            Disposable.Create(minMaxBuffer.Release)
                .AddTo(disposables);

            terrainComputeShader.Dispatch(terrainComputeShader.FindKernel("Generate"), heightMap.Length, 1, 1);

            mapBuffer.GetData(heightMap);
            minMaxBuffer.GetData(minMax);

            return disposables;
        }

        private IDisposable RunFinalPass(int[] minMax, ref float[] heightMap)
        {
            var disposables = new CompositeDisposable();

            ComputeBuffer finalMinMaxBuffer = new ComputeBuffer(minMax.Length, sizeof(int));
            finalMinMaxBuffer.SetData(minMax);
            finalPassComputeShader.SetBuffer(0, "finalPass_MinMax", finalMinMaxBuffer);
            Disposable.Create(finalMinMaxBuffer.Release)
                .AddTo(disposables);

            ComputeBuffer finalMapBuffer = new ComputeBuffer(heightMap.Length, sizeof(float));
            finalMapBuffer.SetData(heightMap);
            finalPassComputeShader.SetBuffer(0, "finalPass_HeightMap", finalMapBuffer);
            Disposable.Create(finalMapBuffer.Release)
                .AddTo(disposables);

            finalPassComputeShader.SetFloat("finalPass_MaxHeight", maxHeight);

            finalPassComputeShader.SetFloat("finalPass_GlobalScale", globalScale);

            finalPassComputeShader.Dispatch(0, heightMap.Length, 1, 1);

            finalMapBuffer.GetData(heightMap);

            return disposables;
        }
    }
}