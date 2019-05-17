using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public interface ISecondPassFilter
    {
        float[,] ApplyLayer(float[,] terrainData);
    }
}