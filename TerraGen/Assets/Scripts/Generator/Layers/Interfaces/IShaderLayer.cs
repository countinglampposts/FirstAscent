using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public class ShaderLayerParams
    {
        public int mapSize;
    }

    public interface IShaderLayer
    {
        IDisposable ApplyToShader(ShaderLayerParams layerParams, ComputeShader computeShader);
    }
}