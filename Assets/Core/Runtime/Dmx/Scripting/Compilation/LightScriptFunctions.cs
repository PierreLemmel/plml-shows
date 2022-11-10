using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Plml.Dmx.Scripting.Compilation
{
    internal static class LightScriptFunctions
    {
        public static LightScriptFunction Sin { get; } = new(LightScriptType.Float, "sin", true, (Func<float, float>)Mathf.Sin, LightScriptType.Float);
        public static LightScriptFunction Cos { get; } = new(LightScriptType.Float, "cos", true, (Func<float, float>)Mathf.Cos, LightScriptType.Float);

        public static LightScriptFunction Abs_Float { get; } = new(LightScriptType.Float, "abs", true, (Func<float, float>)Mathf.Abs, LightScriptType.Float);
        public static LightScriptFunction Abs_Int { get; } = new(LightScriptType.Integer, "abs", true, (Func<int, int>)Mathf.Abs, LightScriptType.Integer);

        public static LightScriptFunction Round { get; } = new(LightScriptType.Integer, "round", true, (Func<float, int>)Mathf.RoundToInt, LightScriptType.Float);
        public static LightScriptFunction Rng { get; } = new(LightScriptType.Float, "rng", false, (Func<float>)RngImpl);

        public static LightScriptFunction Clamp_Float { get; } = new(
            LightScriptType.Float, "clamp", true, (Func<float, float, float, float>)Mathf.Clamp,
            LightScriptType.Float, LightScriptType.Float, LightScriptType.Float
        );

        public static LightScriptFunction Clamp_Int { get; } = new(
            LightScriptType.Integer, "clamp", true, (Func<int, int, int, int>)Mathf.Clamp,
            LightScriptType.Integer, LightScriptType.Integer, LightScriptType.Integer
        );

        public static LightScriptFunction Clamp01 { get; } = new(LightScriptType.Float, "clamp01", true, (Func<float, float>)Mathf.Clamp01, LightScriptType.Float);

        public static LightScriptFunction ClampDmx_Float { get; } = new(LightScriptType.Float, "clampDmx", true, (Func<float, float>)ClampDmx_Impl, LightScriptType.Float);

        public static LightScriptFunction ClampDmx_Int { get; } = new(LightScriptType.Integer, "clampDmx", true, (Func<int, int>)ClampDmx_Impl, LightScriptType.Integer);

        public static LightScriptFunction Min_Int { get; } = new(LightScriptType.Integer, "min", true,
            (Func<int[], int>)Mathf.Min, new LightScriptArgument(LightScriptType.Integer, true));

        public static LightScriptFunction Min_Float { get; } = new(LightScriptType.Float, "min", true,
            (Func<float[], float>)Mathf.Min, new LightScriptArgument(LightScriptType.Float, true));

        public static LightScriptFunction Max_Int { get; } = new(LightScriptType.Integer, "max", true,
            (Func<int[], int>)Mathf.Max, new LightScriptArgument(LightScriptType.Integer, true));

        public static LightScriptFunction Max_Float { get; } = new(LightScriptType.Float, "max", true,
            (Func<float[], float>)Mathf.Max, new LightScriptArgument(LightScriptType.Float, true));


        private static float RngImpl() => URandom.value;

        private static float ClampDmx_Impl(float value) => Mathf.Clamp(value, 0f, 255f);
        private static int ClampDmx_Impl(int value) => Mathf.Clamp(value, 0, 255);

        public static IEnumerable<LightScriptFunction> DefaultFunctions => new LightScriptFunction[]
        {
            Sin,
            Cos,
            Abs_Float,
            Abs_Int,
            Round,
            Rng,
            Clamp_Float,
            Clamp_Int,
            Clamp01,
            ClampDmx_Int,
            ClampDmx_Float,
            Min_Int,
            Min_Float,
            Max_Int,
            Max_Float,
        };
    }
}
