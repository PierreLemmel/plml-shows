using UnityEngine;

namespace Plml
{
    public class RangeBoundsAttribute : PropertyAttribute
    {
        public float lowerBound;
        public float upperBound;

        public RangeBoundsAttribute(float lowerBound, float upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }
    }
}