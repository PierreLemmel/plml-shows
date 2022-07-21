using System;

namespace Plml
{
    [Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public float range => max - min;

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}