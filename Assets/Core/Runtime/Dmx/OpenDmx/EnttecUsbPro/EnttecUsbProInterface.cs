using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;

namespace Plml.Dmx.OpenDmx.EnttecUsbPro
{
    internal class EnttecUsbProInterface : IDmxInterface
    {
        private const int BUFFER_SIZE = 512 + 6;

        private const int BUFFER_OFFSET = 4;
        private const byte START_DELIMITER = 0x7e;
        private const byte END_DELIMITER = 0xe7;

        private const byte SEND_MESSAGE_REQUEST = 0x06; 

        private byte[] buffer;
        private byte[] resetData = new byte[BUFFER_SIZE];

        private SerialPort port;

        public DmxFeature Features => DmxFeature.ReadWrite;

        public void ClearFrame() => Buffer.BlockCopy(resetData, 0, buffer, BUFFER_OFFSET, BUFFER_SIZE);

        public void CopyData(int channelOffset, byte[] input, int length)
        {
            if (length > input.Length) throw new InvalidOperationException($"Input length must be smaller than length");
            if (channelOffset + length > BUFFER_SIZE) throw new InvalidOperationException($"'channelOffset + length' mush be smaller than BufferSize (513)");

            Buffer.BlockCopy(input, 0, buffer, channelOffset + BUFFER_OFFSET, length);
        }

        public void Dispose() => port?.Dispose();

        private void InitializeToSendFrame()
        {
            buffer[0] = START_DELIMITER;
            buffer[1] = SEND_MESSAGE_REQUEST;
            buffer[2] = Bytes.Lsb(BUFFER_SIZE);
            buffer[3] = Bytes.Msb(BUFFER_SIZE);

            buffer[^1] = END_DELIMITER;
        }

        public void SendFrame() => port.Write(buffer, 0, buffer.Length);

        private DmxFrameHandler frameHandler;
        public void AddFrameReceivedHandler(DmxFrameHandler handler) => frameHandler += handler;
        public void RemoveFrameReceivedHandler(DmxFrameHandler handler) => frameHandler -= handler;

        public void Start()
        {
            InitializeToSendFrame();

            port = new("COM4", 38400, Parity.None, 8, StopBits.One);
            port.Open();
        }

        public void Stop() => port?.Close();
    }
}