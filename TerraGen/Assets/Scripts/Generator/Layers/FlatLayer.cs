using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class FlatLayer : IFirstPassFilter
    {
        [SerializeField]
        float altitude;

        public float ApplyLayer(float x, float y, float height)
        {
            return Mathf.Clamp(height, altitude, Mathf.Infinity);
        }
    }
}