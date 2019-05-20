using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public class ShaderLayerParams
    {
        public float globalScale;
    }

    public interface IShaderLayer
    {
        IDisposable ApplyToShader(ShaderLayerParams layerParams, ComputeShader computeShader);
    }
}