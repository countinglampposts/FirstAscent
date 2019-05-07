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
            //Determine if it is on the gird
            var isOnGrid = pos.x < latticeGrid.gridWorldSize && pos.x > 0 && pos.y < latticeGrid.gridWorldSize && pos.y > 0;
            if (!isOnGrid)
                return pos;

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

            var p1 = Vector2.Lerp(latticeGrid.points[aIndex], latticeGrid.points[bIndex], xLerp);
            var p2 = Vector2.Lerp(latticeGrid.points[cIndex], latticeGrid.points[dIndex], xLerp);
            var returned = Vector2.Lerp(p1, p2, yLerp);

            return returned;
        }
    }
}