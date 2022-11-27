using System;

namespace Plml.Dmx
{
    public delegate void DmxFrameHandler(byte[] data);

    public interface IDmxInterface : IDisposable
    {
        void Start();
        void Stop();

        void SendFrame();
        void ClearFrame();
        void CopyData(int channelOffset, byte[] data, int length);

        void AddFrameReceivedHandler(DmxFrameHandler handler);
        void RemoveFrameReceivedHandler(DmxFrameHandler handler);

        public DmxFeature Features { get; }
    }
}