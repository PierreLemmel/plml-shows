using System;
using UnityEngine;

namespace Plml.Playground.Surfaces
{
    [Serializable]
    public class GridSurfaceParameters
    {
        [Range(0.0f, 100.0f)]
        public float width = 15.0f;

        [Range(0.0f, 100.0f)]
        public float height = 10.0f;

        [Range(2, 100)]
        public int rows = 10;

        [Range(2, 100)]
        public int columns = 15;
    }
}