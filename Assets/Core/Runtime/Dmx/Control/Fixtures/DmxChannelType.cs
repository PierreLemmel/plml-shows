using System;
using static Plml.Dmx.DmxChannelType;

namespace Plml.Dmx
{
    public enum DmxChannelType
    {
        Trad,
        Dimmer,
        Stroboscope,
        White,
        Uv,
        Cold,
        Warm,
        Amber,
        Lime,

        Beam,
        Pan,
        PanFine,
        Tilt,
        TiltFine,

        Color,
        SplitColor,

        ColorArray16,
        ColorArray32
    }

    

    public static class DmxChannelTypeExtensions
    {
        public static bool IsByteChannel(this DmxChannelType channel) => channel < Color;
        public static bool IsColorChannel(this DmxChannelType channel) => channel == Color;
        public static bool IsSplitColorChannel(this DmxChannelType channel) => channel == SplitColor;
        public static bool IsColorArray(this DmxChannelType channel) => channel == ColorArray16 || channel == ColorArray32;

        public static int ColorArrayCount(this DmxChannelType channel) => channel switch
        {
            ColorArray16 => 16,
            ColorArray32 => 32,
            _ => throw new InvalidOperationException($"Channel {channel} is not a Color Array")
        };
    }
}