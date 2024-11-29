using System;
using System.IO.Ports;
using System.Linq;

namespace Plml.Dmx.EnttecUsbPro
{
    internal class EnttecUsbProReadInterface : IReadDmxInterface
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

        private DmxPacketReceivedHandler packetHandler;

        public void AddHandler(DmxPacketReceivedHandler handler) => packetHandler += handler;
        public void RemoveHandler(DmxPacketReceivedHandler handler) => packetHandler -= handler;

        public void ClearFrame() => Buffer.BlockCopy(resetData, 0, buffer, 0, BUFFER_SIZE);

        public void Dispose() => port?.Dispose();

        public void Start()
        {
            buffer = new byte[BUFFER_SIZE];

            string[] ports = SerialPort.GetPortNames();
            string portName = ports.Single();

            port = new(portName, 38400, Parity.None, 8, StopBits.One);
            port.Open();

            byte[] foo = new byte[1024];
            int bar = port.Read(foo, 0, 1024);

            port.DataReceived += (_, e) => Logs.Info(e.EventType);
            port.ErrorReceived += (_, e) => Logs.Info(e.EventType);
        }

        public void Stop() => port?.Close();
    }
}