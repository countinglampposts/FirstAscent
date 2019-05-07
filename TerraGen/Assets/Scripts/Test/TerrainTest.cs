﻿using TerraGen.Generator;
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

            float[,] pointData = new float[gridSize + 1, gridSize + 1];

            for (int x = 0; x < pointData.GetLength(0); x++)
            {
                for (int y = 0; y < pointData.GetLength(1); y++)
                {
                    float globalX = (x * lodMultiplier + transform.position.x) / scale;
                    float globalY = (y * lodMultiplier + transform.position.z) / scale;
                    var globalPosition = latticeLayer.Mutate(new Vector2(globalX, globalY));

                    pointData[x, y] = perlinLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData[x, y]);
                    pointData[x, y] = falloffLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData[x, y]);
                    pointData[x, y] = flatLayer.ApplyLayer(globalPosition.x, globalPosition.y, pointData[x, y]);
                }
            }

            pointData = normalizeLayer.ApplyLayer(pointData);

            for (int x = 0; x < pointData.GetLength(0); x++)
            {
                for (int y = 0; y < pointData.GetLength(1); y++)
                {
                    pointData[x, y] *= scale;
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