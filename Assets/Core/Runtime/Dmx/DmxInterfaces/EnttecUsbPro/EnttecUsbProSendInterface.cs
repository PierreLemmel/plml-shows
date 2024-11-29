using System;
using System.IO.Ports;
using System.Linq;

namespace Plml.Dmx.EnttecUsbPro
{
    internal class EnttecUsbProSendInterface : ISendDmxInterface
    {
        private const int DATA_SIZE = 512;
        private const int BUFFER_SIZE = DATA_SIZE + BUFFER_OFFSET;

        private const int BUFFER_OFFSET = 4;
        private const byte START_DELIMITER = 0x7e;
        private const byte END_DELIMITER = 0xe7;

        private const byte SEND_MESSAGE_REQUEST = 0x06; 

        private byte[] buffer;
        private byte[] resetData = new byte[BUFFER_SIZE];

        private SerialPort port;

        public void ClearFrame() => Buffer.BlockCopy(resetData, 0, buffer, 0, BUFFER_SIZE);

        public void CopyData(int channelOffset, byte[] input, int length)
        {
            if (length > input.Length) throw new InvalidOperationException($"Input length must be smaller than length");
            if (channelOffset + length > BUFFER_SIZE) throw new InvalidOperationException($"'channelOffset + length' mush be smaller than BufferSize ({BUFFER_SIZE})");

            Buffer.BlockCopy(input, 0, buffer, channelOffset + BUFFER_OFFSET, length);
        }

        public void Dispose() => port?.Dispose();

        private void InitializeToSendFrame()
        {
            buffer[0] = START_DELIMITER;
            buffer[1] = SEND_MESSAGE_REQUEST;
            buffer[2] = Bytes.Lsb(DATA_SIZE);
            buffer[3] = Bytes.Msb(DATA_SIZE);
            buffer[4] = (byte)0;

            buffer[^1] = END_DELIMITER;
        }

        public void SendFrame() => port.Write(buffer, 0, buffer.Length);

        public void Start()
        {
            buffer = new byte[BUFFER_SIZE];

            InitializeToSendFrame();

            string[] ports = SerialPort.GetPortNames();
            string portName = ports.Single();

            port = new(portName, 38400, Parity.None, 8, StopBits.One);
            port.Open();
        }

        public void Stop() => port?.Close();
    }
}