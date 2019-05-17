using System;
using System.Threading;
using UniRx;
using UnityEngine;
using TerrainMeshData = TerraGen.Data.TerrainMeshData;

namespace TerraGen.Generator
{
    public class TerrainMeshFactory
    {
        public IObservable<Mesh> GenerateTerrainMesh(TerrainMeshData terrainData)
        {
            var returned = new Subject<Mesh>();

            int size = terrainData.pointData.mapSize;
            int lodMultiplier = (int)Mathf.Pow(2, terrainData.lod);

            Vector3[] verticies = new Vector3[size * size];
            Vector2[] uvs = new Vector2[verticies.Length];
            int[] triangles = new int[(size - 1) * (size - 1) * 2 * 6];

            Thread thread = new Thread(() =>
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        var uv = new Vector2();
                        var vertex = new Vector3();

                        uv.x = vertex.x = x * lodMultiplier;
                        uv.y = vertex.z = y * lodMultiplier;

                        vertex.y = terrainData.pointData.data[y * size + x];

                        var index = x * size + y;
                        verticies[index] = vertex;
                        uvs[index] = uv;
                    }
                }

                for (int x = 0; x < size - 1; x++)
                {
                    for (int y = 0; y < size - 1; y++)
                    {
                        int baseVertexIndex = y * size + x;
                        int baseTriangleIndex = baseVertexIndex * 6;

                        // |\
                        // |_\
                        triangles[baseTriangleIndex + 2] = baseVertexIndex;
                        triangles[baseTriangleIndex + 1] = baseVertexIndex + size;
                        triangles[baseTriangleIndex + 0] = baseVertexIndex + size + 1;

                        // ___
                        // \ |
                        //  \|
                        triangles[baseTriangleIndex + 3] = baseVertexIndex;
                        triangles[baseTriangleIndex + 4] = baseVertexIndex + 1;
                        triangles[baseTriangleIndex + 5] = baseVertexIndex + size + 1;
                    }
                }
            });

            thread.Start();

            Observable.EveryUpdate()
                .First(_ => !thread.IsAlive)
                .Subscribe(_ =>
                {
                    Mesh mesh = new Mesh();

                    mesh.vertices = verticies;
                    mesh.triangles = triangles;
                    mesh.uv = uvs;

                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();

                    returned.OnNext(mesh);
                    returned.OnCompleted();
                });


            return returned;
        }
    }
}