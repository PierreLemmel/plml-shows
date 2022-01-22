using System;

namespace Plml.Playground.Surfaces
{
    [Serializable]
    public class SurfaceFormula
    {
        public string text = "0";

        [ReadOnly]
        public string error = "";
    }
}