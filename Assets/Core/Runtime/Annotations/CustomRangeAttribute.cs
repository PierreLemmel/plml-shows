using UnityEngine;

namespace Plml
{
    public abstract class CustomRangeAttribute : PropertyAttribute
    {
        public readonly float min;

        public readonly float max;

        public CustomRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}