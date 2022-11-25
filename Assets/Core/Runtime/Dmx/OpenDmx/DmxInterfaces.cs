using Plml.Dmx.OpenDmx.EnttecUsbPro;
using System;

namespace Plml.Dmx.OpenDmx
{
    public static class DmxInterfaces
    {
        public static void CopyData(this IDmxInterface openDmx, byte[] data) => openDmx.CopyData(0, data, data.Length);

        public static void CopyData(this IDmxInterface openDmx, int channelOffset, byte[] data)
            => openDmx.CopyData(channelOffset, data, data.Length);

        public static IDmxInterface Create(DmxInterfaceType type) => type switch
        {
            DmxInterfaceType.None => new LogInterface(LogLevel.Info),
            DmxInterfaceType.EnntecOpenDmx => new OpenDmxInterface(),
            DmxInterfaceType.EnntecUsbPro => new EnttecUsbProInterface(),
            _ => throw new InvalidOperationException($"Unknown Dmx Interface type: {type}")
        };

        public static bool HasFeature(this IDmxInterface dmxInterface, DmxFeature feature) => dmxInterface.Features.HasFlag(feature);
        public static bool HasReadFeature(this IDmxInterface dmxInterface) => dmxInterface.HasFeature(DmxFeature.Read);
        public static bool HasWriteFeature(this IDmxInterface dmxInterface) => dmxInterface.HasFeature(DmxFeature.Write);
    }
}