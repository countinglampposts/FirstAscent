using System.Collections;
using System.Collections.Generic;
using TerraGen.Test;
using UnityEditor;
using UnityEngine;

namespace TerraGen.Test
{
    [CustomEditor(typeof(TerrainTest))]
    public class TerrainTestEditor : Editor
    {
        private bool generateOnUpdate;

        public override void OnInspectorGUI()
        {
            generateOnUpdate = GUILayout.Toggle(generateOnUpdate, "Generate On Update");

            var terrainTest = target as TerrainTest;
            if (generateOnUpdate || GUILayout.Button("Generate"))
            {
                terrainTest.GenerateTerrain();
            }
            base.OnInspectorGUI();
        }
    }
}