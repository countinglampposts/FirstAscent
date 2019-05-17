using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public struct LatticeParams
    {
        public int gridSize;
        public float gridWorldSize;
        public int seed;
        [HideInInspector] public Vector2[] points;
    }

    [System.Serializable]
    public struct MutatorParams
    {
        public int lod;
        [HideInInspector] public Vector2 position;
        public bool useLattice;
        [HideInInspector] public LatticeParams latticeParams;
        public float scale;
    }
}