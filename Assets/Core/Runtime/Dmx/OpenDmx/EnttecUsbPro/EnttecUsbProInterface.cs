using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;

namespace Plml.Dmx.OpenDmx.EnttecUsbPro
{
    internal unsafe class EnttecUsbProInterface : IDmxInterface
    {
        private const int BufferSize = 513;
        private UbsProData data;

        private SerialPort port;

        public void ClearFrame()
        {
            Logs.Info("ClearFrame");
        }

        public void CopyData(int channelOffset, byte[] data, int length)
        {
            Logs.Info("CopyData");
        }

        public void Dispose()
        {
            Logs.Info("Dispose");
        }

        public void SendFrame()
        {
            Logs.Info("SendFrame");
        }

        public void Start()
        {
            Logs.Info("Start");
        }

        public void Stop()
        {
            Logs.Info("Stop");
        }

        private unsafe struct UbsProData
        {
            public IntPtr handle;
            public fixed byte buffer[BufferSize];
        }
    }
}
