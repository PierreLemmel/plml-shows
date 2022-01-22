namespace Plml.Dmx.OpenDmx
{
    public static class OpenDmxExtensions
    {
        public static void CopyData(this IOpenDmxInterface openDmx, byte[] data) => openDmx.CopyData(0, data, data.Length);

        public static void CopyData(this IOpenDmxInterface openDmx, int channelOffset, byte[] data)
            => openDmx.CopyData(channelOffset, data, data.Length);
    }
}