﻿using System.Collections;
using System.Collections.Generic;
using TerraGen.Data;
using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class LatticeLayer : IMutator
    {
        [SerializeField] LatticeGrid latticeGrid;

        public Vector2 Mutate(Vector2 pos)
        {
            var latticeParams = latticeGrid.latticeParams;
            // Determine where it would be on the grid
            // -right->
            // a--p1--b ^
            // |   |  | |
            // |   x  | up
            // |   |  | |
            // c--p2--d |
            var gridInterval = latticeParams.gridWorldSize / latticeParams.gridSize;

            var upY = Mathf.CeilToInt(pos.y / gridInterval);
            var downY = Mathf.FloorToInt(pos.y / gridInterval);
            var leftX = Mathf.FloorToInt(pos.x / gridInterval);
            var rightX = Mathf.CeilToInt(pos.x / gridInterval);

            var xLerp = (pos.x - leftX * gridInterval) / gridInterval;
            var yLerp = (pos.y - downY * gridInterval) / gridInterval;

            // Determine where it is now on the grid
            var aIndex = latticeGrid.GridPointToIndex(leftX, upY);
            var bIndex = latticeGrid.GridPointToIndex(rightX, upY);
            var cIndex = latticeGrid.GridPointToIndex(leftX, downY);
            var dIndex = latticeGrid.GridPointToIndex(rightX, downY);

            //Determine if it is on the grid
            if (aIndex < 0 || aIndex >= latticeParams.points.Length)
                return pos;
            if (bIndex < 0 || bIndex >= latticeParams.points.Length)
                return pos;
            if (cIndex < 0 || cIndex >= latticeParams.points.Length)
                return pos;
            if (dIndex < 0 || dIndex >= latticeParams.points.Length)
                return pos;

            var p1 = Vector2.Lerp(latticeParams.points[aIndex], latticeParams.points[bIndex], xLerp);
            var p2 = Vector2.Lerp(latticeParams.points[cIndex], latticeParams.points[dIndex], xLerp);
            var returned = Vector2.Lerp(p2, p1, yLerp);

            return returned;
        }

        public LatticeParams GetLatticeParams()
        {
            return latticeGrid.latticeParams;
        }
    }
}