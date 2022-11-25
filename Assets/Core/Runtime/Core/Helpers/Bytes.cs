using System;

namespace Plml
{
    public static class Bytes
    {
        public static byte Lsb(int i) => (byte)(i & 255);
        public static byte Msb(int i) => (byte)(i & 255);
    }
}
