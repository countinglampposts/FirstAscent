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
    public class FalloffLayer : IFirstPassFilter, IShaderLayer
    {
        [SerializeField] FalloffPointData[] points;

        public float ApplyLayer(float x, float y, float height)
        {
            var multiplier = 0f;
            foreach (var point in points)
            {
                var distance = Vector2.Distance(new Vector2(x, y), point.center);
                multiplier += Mathf.SmoothStep(point.weight, 0f, distance / point.radius);
            }
            height *= multiplier;

            return height;
        }

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