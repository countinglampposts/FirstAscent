﻿using UnityEditor;
using UnityEngine;

namespace TerraGen.Generator
{
    [CustomEditor(typeof(LatticeGrid))]
    public class LatticeGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var latticeGrid = target as LatticeGrid;
            var gridSize = latticeGrid.latticeData.gridSize;

            if (GUILayout.Button("Reset"))
            {
                Undo.RecordObject(latticeGrid, "Reset lattice");
                latticeGrid.ResetPoints();
            }
            if (GUILayout.Button("Randomize"))
            {
                Undo.RecordObject(latticeGrid, "Randomize lattice");
                latticeGrid.Randomize();
            }
        }

        private void OnSceneGUI()
        {
            var latticeGrid = target as LatticeGrid;
            var latticeParams = latticeGrid.latticeData;

            var gridWorldSize = latticeParams.gridLOD;
            var gridSize = latticeParams.gridSize;
            var totalPointCount = gridSize * gridSize;
            Vector2[] points;

            if (latticeParams.points == null || latticeParams.points.Length != totalPointCount)
                latticeGrid.ResetPoints();

            points = latticeParams.points;

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
    }
}