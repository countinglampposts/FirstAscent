using UnityEditor;
using UnityEngine;

namespace TerraGen.Generator
{
    [CustomEditor(typeof(LatticeGrid))]
    public class LatticeGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var latticeGrid = target as LatticeGrid;
            base.OnInspectorGUI();
            if (GUILayout.Button("Reset"))
                ResetPoints();
        }

        private void OnSceneGUI()
        {
            var latticeGrid = target as LatticeGrid;
            var gridWorldSize = latticeGrid.gridWorldSize;
            var gridSize = latticeGrid.gridSize;
            var totalPointCount = gridSize * gridSize;
            Vector2[] points;

            if (latticeGrid.points == null || latticeGrid.points.Length != totalPointCount)
                ResetPoints();

            points = latticeGrid.points;

            EditorGUI.BeginChangeCheck();
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    var point = points[latticeGrid.GridPointToIndex(x, y)];
                    var drawnPoint = new Vector3(point.x, 10000, point.y);
                    var newPoint = Handles.PositionHandle(drawnPoint, Quaternion.identity);
                    if (newPoint != drawnPoint)
                        Undo.RecordObject(target, "Changed lattice data");
                    points[latticeGrid.GridPointToIndex(x, y)] = new Vector2(newPoint.x, newPoint.z);

                    if (y < gridSize - 1)
                    {
                        var upPoint = points[latticeGrid.GridPointToIndex(x, y + 1)];
                        Handles.DrawLine(drawnPoint, new Vector3(upPoint.x, 10000, upPoint.y));
                    }

                    if (x < gridSize - 1)
                    {
                        var rightPoint = points[latticeGrid.GridPointToIndex(x + 1, y)];
                        Handles.DrawLine(drawnPoint, new Vector3(rightPoint.x, 10000, rightPoint.y));
                    }
                }
            }
            EditorGUI.EndChangeCheck();
        }

        private void ResetPoints()
        {
            var latticeGrid = target as LatticeGrid;
            var gridWorldSize = latticeGrid.gridWorldSize;
            var gridSize = latticeGrid.gridSize;
            Vector2[] points;

            points = new Vector2[gridSize * gridSize];
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    var worldX = Mathf.Lerp(0, gridWorldSize, (float)x / gridSize);
                    var worldY = Mathf.Lerp(0, gridWorldSize, (float)y / gridSize);
                    points[latticeGrid.GridPointToIndex(x, y)] = new Vector2(worldX, worldY);
                }
            }

            Undo.RecordObject(target, "Reset lattice data");
            latticeGrid.points = points;
        }
    }
}