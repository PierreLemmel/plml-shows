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

        public void Deconstruct(out float min, out float max)
        {
            min = this.min;
            max = this.max;
        }
    }
}