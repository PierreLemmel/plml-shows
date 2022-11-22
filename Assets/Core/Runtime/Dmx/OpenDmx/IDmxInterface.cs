using System;

namespace Plml.Dmx
{
    public interface IDmxInterface : IDisposable
    {
        void Start();
        void Stop();

        void SendFrame();
        void ClearFrame();
        void CopyData(int channelOffset, byte[] data, int length);
    }
}