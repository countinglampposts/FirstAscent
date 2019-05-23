using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using TerrainMeshData = TerraGen.Data.TerrainMeshData;

namespace TerraGen.Generator
{
    public class TerrainMeshFactory
    {
        public Mesh GenerateTerrainMesh(TerrainMeshData terrainData, ComputeShader computeShader)
        {
            var disposables = new CompositeDisposable();

            var mapSize = terrainData.pointData.mapSize;
            var mapLength = terrainData.pointData.data.Length;

            computeShader.SetInt("mapSize", mapSize);
            computeShader.SetInt("lodMultiplier", (int)Mathf.Pow(2, terrainData.lod));

            ComputeBuffer heightMapBuffer = new ComputeBuffer(mapLength, sizeof(float));
            heightMapBuffer.SetData(terrainData.pointData.data);
            computeShader.SetBuffer(0, "heightMap", heightMapBuffer);
            Disposable.Create(heightMapBuffer.Release)
                .AddTo(disposables);

            Vector3[] verticies = new Vector3[mapLength];
            ComputeBuffer verticiesBuffer = new ComputeBuffer(verticies.Length, sizeof(float) * 3);
            verticiesBuffer.SetData(verticies);
            computeShader.SetBuffer(0, "verticies", verticiesBuffer);
            Disposable.Create(verticiesBuffer.Release)
                .AddTo(disposables);

            Vector2[] uvs = new Vector2[mapLength];
            ComputeBuffer uvsBuffer = new ComputeBuffer(uvs.Length, sizeof(float) * 2);
            uvsBuffer.SetData(uvs);
            computeShader.SetBuffer(0, "uvs", uvsBuffer);
            Disposable.Create(uvsBuffer.Release)
                .AddTo(disposables);

            int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 2 * 6];
            triangles = triangles.Select(_ => -1).ToArray();
            ComputeBuffer trianglesBuffer = new ComputeBuffer(triangles.Length, sizeof(int));
            trianglesBuffer.SetData(triangles);
            computeShader.SetBuffer(0, "triangles", trianglesBuffer);
            Disposable.Create(trianglesBuffer.Release)
                .AddTo(disposables);

            computeShader.Dispatch(0, mapLength, 1, 1);

            verticiesBuffer.GetData(verticies);
            uvsBuffer.GetData(uvs);
            trianglesBuffer.GetData(triangles);

            disposables.Dispose();

            Mesh mesh = new Mesh();

            mesh.vertices = verticies;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            foreach (var t in triangles)
            {
                Debug.Log(t);
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }

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
                        var index = x * size + y;

                        uv.x = vertex.x = x * lodMultiplier;
                        uv.y = vertex.z = y * lodMultiplier;

                        vertex.y = terrainData.pointData.data[index];

                        verticies[index] = vertex;
                        uvs[index] = uv;

                        if (x < size - 1 && y < size - 1)
                        {
                            int baseTriangleIndex = index * 6;

                            // |\
                            // |_\
                            triangles[baseTriangleIndex + 2] = index;
                            triangles[baseTriangleIndex + 1] = index + size;
                            triangles[baseTriangleIndex + 0] = index + size + 1;

                            // ___
                            // \ |
                            //  \|
                            triangles[baseTriangleIndex + 3] = index;
                            triangles[baseTriangleIndex + 4] = index + 1;
                            triangles[baseTriangleIndex + 5] = index + size + 1;
                        }
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