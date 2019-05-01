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
        public override void OnInspectorGUI()
        {
            var terrainTest = target as TerrainTest;
            if (GUILayout.Button("Generate"))
            {
                terrainTest.GenerateTerrain();
            }
            base.OnInspectorGUI();
        }
    }
}