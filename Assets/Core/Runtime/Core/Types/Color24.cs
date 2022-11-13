using System;
using UnityEngine;

namespace Plml
{
    [Serializable]
    public struct Color24
    {
        public byte r;
        public byte g;
        public byte b;

        public float hue
        {
            get => Hsv().h;
            set
            {
                (_, float s, float v) = Hsv();
                ApplyHsv(value, s, v);
            }
        }

        public float saturation
        {
            get => Hsv().s;
            set
            {
                (float h, _, float v) = Hsv();
                ApplyHsv(h, value, v);
            }
        }

        public float value
        {
            get => Hsv().v;
            set
            {
                (float h, float s, _) = Hsv();
                ApplyHsv(h, s, value);
            }
        }

        private void ApplyHsv(float h, float s, float v)
        {
            Color24 c = Color.HSVToRGB(h, s, v);
            r = c.r;
            g = c.g;
            b = c.b;
        }

        public (float h, float s, float v) Hsv()
        {
            Color.RGBToHSV(this, out float h, out float s, out float v);
            return (h, s, v);
        }

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

        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = this.r;
            g = this.g;
            b = this.b;
        }

        public static implicit operator Color32(Color24 c24) => new(c24.r, c24.g, c24.b, 0xff);
        public static implicit operator Color24(Color32 c32) => new(c32.r, c32.g, c32.b);

        public static implicit operator Color24(Color c) => (Color24)(Color32)c;
        public static implicit operator Color(Color24 c24) => (Color)(Color32)c24;

        public static Color24 Rgb(byte r, byte g, byte b) => new(r, g, b);
        public static Color24 Rgb(int r, int g, int b) => new(r, g, b);
        public static Color24 Hsv(float h, float s, float v)
        {
            Color24 result = new();
            result.ApplyHsv(h, s, v);
            return result;
        }
    }
}
