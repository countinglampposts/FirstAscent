using UnityEngine;

namespace TerraGen.Data
{
    [System.Serializable]
    public struct LatticeParams
    {
        public int gridSize;
        public float gridWorldSize;
        public int seed;
        public Vector2[] points;
    }

    [System.Serializable]
    public struct MutatorParams
    {
        public int lod;
        [HideInInspector] public Vector2 position;
        //public bool useLattice;
        //public LatticeParams latticeParams;
        public float scale;
    }
}