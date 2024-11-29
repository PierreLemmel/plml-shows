namespace Plml.Dmx
{
    internal class LogSendInterface : ISendDmxInterface
    {
        private readonly LogLevel logLvl;

        public LogSendInterface(LogLevel lvl) => logLvl = lvl;

        public void ClearFrame() => Log("Frame cleared");

        public void CopyData(int channelOffset, byte[] data, int length) { }
        public void Dispose() { }

        public void SendFrame() { }

        public void Start() => Log("Send interface started");
        public void Stop() => Log("Send interface stopped");

        private void Log(string msg) => Logs.Log(logLvl, "DmxInterface: " + msg);
    }
}