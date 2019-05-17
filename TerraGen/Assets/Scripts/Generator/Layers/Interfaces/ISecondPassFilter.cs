using System.Collections;
using System.Collections.Generic;
using TerraGen.Data;
using UnityEngine;

namespace TerraGen.Generator
{
    public interface ISecondPassFilter
    {
        TerrainPointData ApplyLayer(TerrainPointData terrainData, MutatorData mutatorParams);
    }
}