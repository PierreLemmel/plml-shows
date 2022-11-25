using System;

namespace Plml.Dmx
{
    [Flags]
    public enum DmxFeature
    {
        Write = 1,
        Read = 2,

        ReadWrite = Write | Read,
    }
}