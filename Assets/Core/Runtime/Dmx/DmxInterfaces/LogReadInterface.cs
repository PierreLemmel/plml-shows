namespace Plml.Dmx
{
    internal class LogReadInterface : IReadDmxInterface
    {
        private readonly LogLevel logLvl;

        public LogReadInterface(LogLevel lvl) => logLvl = lvl;


        public void AddHandler(DmxPacketReceivedHandler handler) => Log("Dmx Packet handler added");
        public void RemoveHandler(DmxPacketReceivedHandler handler) => Log("Dmx Packet handler removed");

        public void Dispose() { }

        public void Start() => Log("Read interface started");
        public void Stop() => Log("Read interface stopped");

        private void Log(string msg) => Logs.Log(logLvl, "DmxInterface: " + msg);
    }
}