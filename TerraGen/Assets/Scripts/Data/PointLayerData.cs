using System.Collections;
using System.Collections.Generic;
using TerraGen.Generator;
using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public class PointLayerData : ITerrainLayer
    {
        public PointData[] points;

        public float ApplyLayer(float x, float y, float height)
        {
            var multiplier = 1f;
            foreach (var point in points)
            {
                var distance = Vector2.Distance(new Vector2(x, y), point.center);
                multiplier += Mathf.Lerp(point.weight, 0, distance / point.radius);
            }
            height *= multiplier;

            return height;
        }
    }

    [System.Serializable]
    public class PointData
    {
        public float weight = 1f;
        public Vector2 center;
        public float radius;
    }
}