namespace Plml.Dmx
{
    public enum DmxChannelType
    {
        Dimmer,
        Stroboscope,
        White,
        Uv,
        Cold,
        Warm,
        Amber,
        Pan,
        Tilt,

        Color,
    }

    public static class DmxChannelTypeExtensions
    {
        public static bool IsByteChannel(this DmxChannelType channel) => channel != DmxChannelType.Color;
        public static bool IsColorChannel(this DmxChannelType channel) => channel == DmxChannelType.Color;
    }
}