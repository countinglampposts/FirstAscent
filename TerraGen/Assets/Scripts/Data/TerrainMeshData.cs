namespace TerraGen.Data
{
    public struct TerrainPointData
    {
        public float[] data;
        public int mapSize;
    }

    public struct TerrainMeshData
    {
        public TerrainPointData pointData;
        public int lod;
    }
}