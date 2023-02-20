using System;

namespace Plml.Dmx
{
    public delegate void DmxPacketReceivedHandler();

    public interface IReadDmxInterface : IDisposable
    {
        public void AddHandler(DmxPacketReceivedHandler handler);
        public void RemoveHandler(DmxPacketReceivedHandler handler);

        void Start();
        void Stop();
    }
}