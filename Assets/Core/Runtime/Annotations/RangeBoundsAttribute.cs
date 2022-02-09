using UnityEngine;

namespace Plml
{
    public class RangeBoundsAttribute : PropertyAttribute
    {
        public readonly float lowerBound;
        public readonly float upperBound;

        public readonly RangeType rangeType;

        public RangeBoundsAttribute(int lowerBound, int upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;

            rangeType = RangeType.Int;
        }

        public RangeBoundsAttribute(float lowerBound, float upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;

            rangeType = RangeType.Float;
        }

        public enum RangeType
        {
            Int,
            Float
        }
    }
}