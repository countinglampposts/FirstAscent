using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public interface ITerrainLayer
    {
        float ApplyLayer(float x, float y, float height);
    }
}