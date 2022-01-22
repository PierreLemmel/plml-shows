using System;

namespace Plml.Dmx.OpenDmx.FTD2XX
{
    public sealed class FT_Exception : Exception
    {
        public FT_STATUS Status { get; }

        internal FT_Exception(FT_STATUS status) : base($"Unexpected status: {status}")
        {
            Status = status;
        }
    }
}