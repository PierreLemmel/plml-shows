using System;

namespace Plml
{
    [Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        public int range => max - min;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public void Deconstruct(out int min, out int max)
        {
            min = this.min;
            max = this.max;
        }
    }
}