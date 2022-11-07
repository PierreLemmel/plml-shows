using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Plml.Dmx.Scripting.Compilation
{
    internal static class LightScriptFunctions
    {
        public static LightScriptFunction Sin { get; } = new(LightScriptType.Float, "sin", true, (Func<float, float>)Mathf.Sin, new LightScriptArgument(LightScriptType.Float));
        public static LightScriptFunction Cos { get; } = new(LightScriptType.Float, "cos", true, (Func<float, float>)Mathf.Cos, new LightScriptArgument(LightScriptType.Float));

        public static LightScriptFunction Abs_Float { get; } = new(LightScriptType.Float, "abs", true, (Func<float, float>)Mathf.Abs, new LightScriptArgument(LightScriptType.Float));
        public static LightScriptFunction Abs_Int { get; } = new(LightScriptType.Integer, "abs", true, (Func<int, int>)Mathf.Abs, new LightScriptArgument(LightScriptType.Integer));

        public static LightScriptFunction Round { get; } = new(LightScriptType.Integer, "round", true, (Func<float, int>)Mathf.RoundToInt, new LightScriptArgument(LightScriptType.Float));
        public static LightScriptFunction Rng { get; } = new(LightScriptType.Float, "rng", false, (Func<float>)(() => URandom.value));

        public static IEnumerable<LightScriptFunction> DefaultFunctions => new LightScriptFunction[]
        {
            Sin,
            Cos,
            Abs_Float,
            Abs_Int,
            Round,
            Rng
        };
    }
}
