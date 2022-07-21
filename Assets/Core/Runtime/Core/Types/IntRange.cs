using System;

namespace Plml
{
    [Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        public float range => max - min;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}