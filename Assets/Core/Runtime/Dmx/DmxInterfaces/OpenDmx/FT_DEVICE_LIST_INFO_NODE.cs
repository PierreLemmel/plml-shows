using System.Runtime.InteropServices;

namespace Plml.Dmx.OpenDmx
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FT_DEVICE_LIST_INFO_NODE
    {
        public ulong Flags;
        public ulong Type;
        public ulong ID;
        public ulong LocId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string SerialNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Description;

        public uint Handle;
    }
}