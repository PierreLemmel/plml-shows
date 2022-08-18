using System;
using UnityEngine;

namespace Plml
{
    public static class Vectors
    {
        public static void Deconstruct(this Vector2 v, out float x, out float y)
        {
            x = v.x;
            y = v.y;
        }

        public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static void Deconstruct(this Vector2Int v, out int x, out int y)
        {
            x = v.x;
            y = v.y;
        }

        public static void Deconstruct(this Vector3Int v, out int x, out int y, out int z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
    }
}