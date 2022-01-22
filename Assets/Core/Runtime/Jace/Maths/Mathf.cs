using System;

namespace Plml.Jace.Maths
{
    public static class Mathf
    {
        public const float E = (float)Math.E;
        public const float PI = (float)Math.PI;

        public static float Cos(float x) => (float)Math.Cos(x);
        public static float Sin(float x) => (float)Math.Sin(x);
        public static float Tan(float x) => (float)Math.Tan(x);
        public static float Log(float x) => (float)Math.Log(x);
        public static float Logn(float x, float newBase) => (float)Math.Log(x, newBase);
        public static float Log10(float x) => (float)Math.Log10(x);
        public static float Sqrt(float x) => (float)Math.Sqrt(x);
        public static float Acos(float x) => (float)Math.Acos(x);
        public static float Asin(float x) => (float)Math.Asin(x);
        public static float Atan(float x) => (float)Math.Atan(x);
        public static float Truncate(float x) => (float)Math.Truncate(x);
        public static float Ceiling(float x) => (float)Math.Ceiling(x);
        public static float Floor(float x) => (float)Math.Floor(x);
        public static float Round(float x) => (float)Math.Round(x);
        public static float Round(float x, int digits) => (float)Math.Round(x, digits);
        public static float Pow(float x, float y) => (float)Math.Pow(x, y);
        public static float Cot(float a) => 1.0f / Tan(a);
        public static float Acot(float d) => Atan(1 / d);
        public static float Csc(float a) => 1.0f / Sin(a);
        public static float Sec(float d) => 1.0f / Cos(d);
    }
}