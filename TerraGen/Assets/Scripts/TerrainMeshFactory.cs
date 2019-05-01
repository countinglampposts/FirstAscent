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

            int width = terrainData.pointData.Length;
            int height = terrainData.pointData[0].Length;
            int lodMultiplier = (int)Mathf.Pow(10, terrainData.lod);

            Vector3[] verticies = new Vector3[width * height];
            int[] triangles = new int[(width - 1) * (height - 1) * 2 * 6];

            Thread thread = new Thread(() =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Vector3 vertex = new Vector3();
                        vertex.x = x * lodMultiplier;
                        vertex.z = y * lodMultiplier;
                        vertex.y = terrainData.pointData[x][y];
                        verticies[x * width + y] = vertex;
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