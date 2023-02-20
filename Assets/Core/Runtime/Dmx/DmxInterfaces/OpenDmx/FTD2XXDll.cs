using System;
using System.Runtime.InteropServices;

namespace Plml.Dmx.OpenDmx
{
    internal static unsafe class FTD2XXDll
    {
        private const string LibName = "FTD2XX.dll";

        public const byte BITS_8 = 8;
        public const byte STOP_BITS_2 = 2;
        public const byte PARITY_NONE = 0;

        public const ushort FLOW_NONE = 0;

        public const byte PURGE_TX = 2;
        public const byte PURGE_RX = 1;

        [DllImport(LibName)]
        public static extern FT_STATUS FT_CreateDeviceInfoList(out ulong lpdwNumDevs);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_GetDeviceInfoList([In, Out] FT_DEVICE_LIST_INFO_NODE[] pDest, out ulong lpdwNumDevs);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_Open(uint uiPort, out IntPtr ftHandle);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_OpenEx(string pvArg1, uint dwFlags, out IntPtr ftHandle);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_Close(IntPtr ftHandle);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_Read(IntPtr ftHandle, byte* lpBuffer, uint dwBytesToRead, out uint lpdwBytesReturned);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_Write(IntPtr ftHandle, byte* lpBuffer, uint dwBytesToRead, out uint lpdwBytesWritten);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_SetDataCharacteristics(IntPtr ftHandle, byte uWordLength, byte uStopBits, byte uParity);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_SetFlowControl(IntPtr ftHandle, ushort usFlowControl, byte uXon, byte uXoff);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_GetModemStatus(IntPtr ftHandle, out uint lpdwModemStatus);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_Purge(IntPtr ftHandle, uint dwMask);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_ClrRts(IntPtr ftHandle);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_SetBreakOn(IntPtr ftHandle);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_SetBreakOff(IntPtr ftHandle);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_GetStatus(IntPtr ftHandle, out uint lpdwAmountInRxQueue, ref uint lpdwAmountInTxQueue, ref uint lpdwEventStatus);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_ResetDevice(IntPtr ftHandle);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_SetDivisor(IntPtr ftHandle, ushort usDivisor);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_GetLatencyTimer(IntPtr ftHandle, out byte pucTimer);
        [DllImport(LibName)]
        public static extern FT_STATUS FT_EE_Read(IntPtr ftHandle, ref FT_PROGRAM_DATA pData);
    }
}