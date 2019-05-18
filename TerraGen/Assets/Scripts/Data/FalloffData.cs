using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class FalloffPointData
    {
        public float weight = 1f;
        public Vector2 center;
        public float radius;
    }

    [System.Serializable]
    public class FalloffData : IShaderLayer
    {
        [SerializeField] FalloffPointData[] points;

        public IDisposable ApplyToShader(ShaderLayerParams layerParams, ComputeShader computeShader)
        {
            List<float> weights = new List<float>();
            List<Vector2> centers = new List<Vector2>();
            List<float> radiuses = new List<float>();

            foreach (var point in points)
            {
                weights.Add(point.weight);
                centers.Add(point.center);
                radiuses.Add(point.radius);
            }

            computeShader.SetInt("falloff_Count", points.Length);

            ComputeBuffer weightsBuffer = new ComputeBuffer(points.Length, sizeof(float));
            weightsBuffer.SetData(weights);
            computeShader.SetBuffer(0, "falloff_Weights", weightsBuffer);

            ComputeBuffer centersBuffer = new ComputeBuffer(points.Length, sizeof(float) * 2);
            centersBuffer.SetData(centers);
            computeShader.SetBuffer(0, "falloff_Centers", centersBuffer);

            ComputeBuffer radiusBuffer = new ComputeBuffer(points.Length, sizeof(float));
            radiusBuffer.SetData(radiuses);
            computeShader.SetBuffer(0, "falloff_Radiuses", radiusBuffer);

            return Disposable.Create(() =>
            {
                weightsBuffer.Release();
                centersBuffer.Release();
                radiusBuffer.Release();
            });
        }
    }
}