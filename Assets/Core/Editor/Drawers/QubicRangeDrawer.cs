using UnityEditor;
using UnityEngine;

namespace Plml.Editor
{
    [CustomPropertyDrawer(typeof(CubicRangeAttribute))]
    public class QubicRangeDrawer : CustomRangeDrawer
    {
        protected override float FromSliderValue(float value) => value * value * value;
        protected override float ToSliderValue(float value) => Mathf.Pow(value, 1.0f / 3.0f);
    }
}