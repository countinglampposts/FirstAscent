﻿using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class FalloffLayer : IFirstPassFilter
    {
        [SerializeField] float baseWeight = 0f;
        [SerializeField] FalloffPointData[] points;

        public float ApplyLayer(float x, float y, float height)
        {
            var multiplier = baseWeight;
            foreach (var point in points)
            {
                var distance = Vector2.Distance(new Vector2(x, y), point.center);
                multiplier += Mathf.Lerp(point.weight, 0f, distance / point.radius);
            }
            height *= multiplier;

            return height;
        }
    }

    [System.Serializable]
    public class FalloffPointData
    {
        public float weight = 1f;
        public Vector2 center;
        public float radius;
    }
}