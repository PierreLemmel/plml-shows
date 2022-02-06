using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plml
{
    public static class Colors
    {
        public static string ToHex(this Color c) => ToHex((Color32)c);
        public static string ToHex(this Color32 c) => $"#{c.a:X}{c.r:X}{c.g:X}{c.b:X}";

        public static Color Max(IEnumerable<Color> colors) => new(
            colors.Max(c => c.r),
            colors.Max(c => c.g),
            colors.Max(c => c.b)
        );

        public static Color32 Max(IEnumerable<Color32> colors) => new(
            colors.Max(c => c.r),
            colors.Max(c => c.g),
            colors.Max(c => c.b),
            colors.Max(c => c.a)
        );

        public static Color Max(params Color[] colors) => Max(colors.AsEnumerable());
        public static Color32 Max(params Color32[] colors) => Max(colors.AsEnumerable());
    }
}