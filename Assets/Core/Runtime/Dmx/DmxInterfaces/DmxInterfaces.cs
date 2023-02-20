using Plml.Dmx.EnttecUsbPro;
using Plml.Dmx.OpenDmx;
using System;

namespace Plml.Dmx
{
    public static class DmxInterfaces
    {
        public static void CopyData(this ISendDmxInterface openDmx, byte[] data) => openDmx.CopyData(0, data, data.Length);

        public static void CopyData(this ISendDmxInterface openDmx, int channelOffset, byte[] data)
            => openDmx.CopyData(channelOffset, data, data.Length);

        public static ISendDmxInterface CreateSendInterface(SendDmxInterfaceType type) => type switch
        {
            SendDmxInterfaceType.None => new LogSendInterface(LogLevel.Info),
            SendDmxInterfaceType.EnntecOpenDmx => new OpenDmxSendInterface(),
            SendDmxInterfaceType.EnntecUsbPro => new EnttecUsbProSendInterface(),
            _ => throw new InvalidOperationException($"Unknown Dmx Interface type: {type}")
        };

        public static IReadDmxInterface CreateReadInterface(ReadDmxInterfaceType type) => type switch
        {
            ReadDmxInterfaceType.None => new LogReadInterface(LogLevel.Info),
            ReadDmxInterfaceType.EnntecUsbPro => new EnttecUsbProReadInterface(),
            _ => throw new InvalidOperationException($"Unknown Dmx Interface type: {type}")
        };
    }
}