using System;
using System.Collections;
using System.Collections.Generic;
using TerraGen.Generator;
using UniRx;
using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public struct PerlinData : IShaderLayer
    {
        [SerializeField]
        float scale;
        [Range(0f, 1f)]
        [SerializeField]
        float persistance;
        [Range(1f, 10f)]
        [SerializeField]
        float lacunarity;
        [SerializeField]
        int octaves;
        [SerializeField]
        int seed;

        public IDisposable ApplyToShader(ShaderLayerParams layerParams, ComputeShader computeShader)
        {
            computeShader.SetInt("perlin_MapSize", layerParams.mapSize);
            computeShader.SetInt("perlin_Octaves", octaves);
            computeShader.SetFloat("perlin_Lacunarity", lacunarity);
            computeShader.SetFloat("perlin_Persistence", persistance);
            computeShader.SetFloat("perlin_Scale", scale);

            var rnd = new System.Random(seed);

            Vector2[] offsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                offsets[i] = new Vector2(rnd.Next(-10000, 10000), rnd.Next(-10000, 10000));
            }
            ComputeBuffer offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 2);
            offsetsBuffer.SetData(offsets);
            computeShader.SetBuffer(0, "perlin_Offsets", offsetsBuffer);

            return Disposable.Create(offsetsBuffer.Release);
        }
    }
}