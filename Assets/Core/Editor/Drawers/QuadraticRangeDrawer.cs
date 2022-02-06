using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(QuadraticRangeAttribute))]
    public class QuadraticRangeDrawer : CustomRangeDrawer
    {
        protected override float FromSliderValue(float value) => value * value;
        protected override float ToSliderValue(float value) => Mathf.Sqrt(value);
    }
}