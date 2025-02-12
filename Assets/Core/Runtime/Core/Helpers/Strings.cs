﻿using System;
using System.Text;

namespace Plml
{
    public static class Strings
    {
        public static string Repeat(this string str, int count)
        {
            StringBuilder sb = new(str.Length * count);
            for (int i = 0; i < count; i++)
                sb.Append(str);

            return sb.ToString();
        }

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNotEmpty(this string str) => !string.IsNullOrEmpty(str);
    }
}