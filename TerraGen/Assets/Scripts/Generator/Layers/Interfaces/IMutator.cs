using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraGen.Generator
{
    public interface IMutator
    {
        Vector2 Mutate(Vector2 pos);
    }
}