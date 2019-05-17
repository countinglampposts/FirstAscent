﻿using System;
using TerraGen.Generator;
using UniRx;
using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public struct LatticeData : IShaderLayer
    {
        public int gridSize;
        public float gridWorldSize;
        [HideInInspector] public Vector2[] points;

        public IDisposable ApplyToShader(ShaderLayerParams layerParams, ComputeShader computeShader)
        {
            computeShader.SetInt("lattice_GridSize", gridSize);
            computeShader.SetFloat("lattice_GridWorldSize", gridWorldSize);

            ComputeBuffer pointsBuffer = new ComputeBuffer(points.Length, sizeof(float) * 2);
            pointsBuffer.SetData(points);
            computeShader.SetBuffer(0, "lattice_Points", pointsBuffer);

            return Disposable.Create(pointsBuffer.Release);
        }
    }

    [System.Serializable]
    public struct MutatorData : IShaderLayer
    {
        public int lod;
        [HideInInspector] public Vector2 position;
        public float scale;

        public IDisposable ApplyToShader(ShaderLayerParams layerParams, ComputeShader computeShader)
        {
            computeShader.SetInt("lod", lod);
            computeShader.SetVector("globalPosition", position);
            computeShader.SetFloat("globalScale", scale);

            return Disposable.Empty;
        }
    }
}