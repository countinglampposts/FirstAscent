using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    [System.Serializable]
    public class LatticeLayer : IMutator
    {
        [SerializeField] LatticeGrid latticeGrid;

        public Vector2 Mutate(Vector2 pos)
        {
            // Determine where it would be on the grid
            // -right->
            // a--p1--b ^
            // |   |  | |
            // |   x  | up
            // |   |  | |
            // c--p2--d |
            var gridInterval = latticeGrid.gridWorldSize / latticeGrid.gridSize;

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
            if (aIndex < 0 || aIndex >= latticeGrid.points.Length)
                return pos;
            if (bIndex < 0 || bIndex >= latticeGrid.points.Length)
                return pos;
            if (cIndex < 0 || cIndex >= latticeGrid.points.Length)
                return pos;
            if (dIndex < 0 || dIndex >= latticeGrid.points.Length)
                return pos;

            var p1 = Vector2.Lerp(latticeGrid.points[aIndex], latticeGrid.points[bIndex], xLerp);
            var p2 = Vector2.Lerp(latticeGrid.points[cIndex], latticeGrid.points[dIndex], xLerp);
            var returned = Vector2.Lerp(p2, p1, yLerp);

            return returned;
        }
    }
}