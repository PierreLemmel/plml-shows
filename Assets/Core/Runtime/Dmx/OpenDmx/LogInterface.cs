namespace Plml.Dmx
{
    internal class LogInterface : IDmxInterface
    {
        private readonly LogLevel logLvl;

        public LogInterface(LogLevel lvl) => logLvl = lvl;

        public DmxFeature Features => DmxFeature.ReadWrite;

        public void ClearFrame() => Log("Frame cleared");

        public void CopyData(int channelOffset, byte[] data, int length) { }
        public void Dispose() { }

        public void SendFrame() { }
        public void Start() => Log("Started");
        public void Stop() => Log("Stopped");

        private void Log(string msg) => Logs.Log(logLvl, "DmxInterface: " + msg);
    }
}