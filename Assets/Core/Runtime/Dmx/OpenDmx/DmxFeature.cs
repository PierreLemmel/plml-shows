using System;

namespace Plml.Dmx
{
    [Flags]
    public enum DmxFeature
    {
        None = 0,

        Write = 1,
        Read = 2,

        ReadWrite = Write | Read,
    }
}