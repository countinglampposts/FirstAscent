using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public interface IFirstPassFilter
    {
        float ApplyLayer(float x, float y, float height);
    }
}