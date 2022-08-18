using UnityEngine;

namespace Plml
{
    public struct Color24
    {
        public byte r;
        public byte g;
        public byte b;

        public Color24(int r, int g, int b)
        {
            this.r = (byte)r;
            this.g = (byte)g;
            this.b = (byte)b;
        }

        public Color24(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public static implicit operator Color32(Color24 c24) => new(c24.r, c24.g, c24.b, 0xff);
        public static implicit operator Color24(Color32 c32) => new(c32.r, c32.g, c32.b);

        public static implicit operator Color24(Color c) => (Color24)(Color32)c;
        public static implicit operator Color(Color24 c24) => (Color)(Color32)c24;
    }
}
