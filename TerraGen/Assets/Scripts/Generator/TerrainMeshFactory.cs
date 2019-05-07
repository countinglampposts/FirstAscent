using System;
using System.Threading;
using UniRx;
using UnityEngine;
using TerrainData = TerraGen.Data.TerrainData;

namespace TerraGen.Generator
{
    public class TerrainMeshFactory
    {
        public IObservable<Mesh> GenerateTerrainMesh(TerrainData terrainData)
        {
            var returned = new Subject<Mesh>();

            int width = terrainData.pointData.GetLength(0);
            int height = terrainData.pointData.GetLength(1);
            int lodMultiplier = (int)Mathf.Pow(2, terrainData.lod);

            Vector3[] verticies = new Vector3[width * height];
            Vector2[] uvs = new Vector2[verticies.Length];
            int[] triangles = new int[(width - 1) * (height - 1) * 2 * 6];

            Thread thread = new Thread(() =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var uv = new Vector2();
                        var vertex = new Vector3();

                        uv.x = vertex.x = x * lodMultiplier;
                        uv.y = vertex.z = y * lodMultiplier;

                        vertex.y = terrainData.pointData[x, y];

                        var index = x * width + y;
                        verticies[index] = vertex;
                        uvs[index] = uv;
                    }
                }

                for (int x = 0; x < width - 1; x++)
                {
                    for (int y = 0; y < height - 1; y++)
                    {
                        int baseVertexIndex = y * width + x;
                        int baseTriangleIndex = baseVertexIndex * 6;

                        // |\
                        // |_\
                        triangles[baseTriangleIndex + 2] = baseVertexIndex;
                        triangles[baseTriangleIndex + 1] = baseVertexIndex + width;
                        triangles[baseTriangleIndex + 0] = baseVertexIndex + width + 1;

                        // ___
                        // \ |
                        //  \|
                        triangles[baseTriangleIndex + 3] = baseVertexIndex;
                        triangles[baseTriangleIndex + 4] = baseVertexIndex + 1;
                        triangles[baseTriangleIndex + 5] = baseVertexIndex + width + 1;
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