using System;
using System.Runtime.InteropServices;

namespace Plml.Dmx.OpenDmx.FTD2XX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FT_PROGRAM_DATA
    {
        public uint Signature1;
        public uint Signature2;
        public uint Version;
        public ushort VendorId;
        public ushort ProductId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Manufacturer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ManufacturerId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Description;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string SerialNumber;

        public uint MaxPower;
        public uint PnP;
        public uint SelfPowered;
        public uint RemoteWakeup;
        public byte Rev4;
        public byte IsoIn;
        public byte IsoOut;
        public byte PullDownEnable;
        public byte SerNumEnable;
        public byte USBVersionEnable;
        
        public ushort USBVersion;

        public byte Rev5;
        public byte IsoInA;
        public byte IsoInB;
        public byte IsoOutA;
        public byte IsoOutB;
        public byte PullDownEnable5;
        public byte SerNumEnable5;
        public byte USBVersionEnable5;
        public uint USBVersion5;
        public byte AIsHighCurrent;
        public byte BIsHighCurrent;
        public byte IFAIsFifo;
        public byte IFAIsFifoTar;
        public byte IFAIsFastSer;
        public byte AIsVCP;
        public byte IFBIsFifo;
        public byte IFBIsFifoTar;
        public byte IFBIsFastSer;
        public byte BIsVCP;
        public byte UseExtOsc;
        public byte HighDriveIOs;
        public byte EndpointSize;
        public byte PullDownEnableR;
        public byte SerNumEnableR;
        public byte InvertTXD;
        public byte InvertRXD;
        public byte InvertRTS;
        public byte InvertCTS;
        public byte InvertDTR;
        public byte InvertDSR;
        public byte InvertDCD;
        public byte InvertRI;
        public byte Cbus0;
        public byte Cbus1;
        public byte Cbus2;
        public byte Cbus3;
        public byte Cbus4;
        public byte RIsD2XX;
        public byte PullDownEnable7;
        public byte SerNumEnable7;
        public byte ALSlowSlew;
        public byte ALSchmittInput;
        public byte ALDriveCurrent;
        public byte AHSlowSlew;
        public byte AHSchmittInput;
        public byte AHDriveCurrent;
        public byte BLSlowSlew;
        public byte BLSchmittInput;
        public byte BLDriveCurrent;
        public byte BHSlowSlew;
        public byte BHSchmittInput;
        public byte BHDriveCurrent;
        public byte IFAIsFifo7;
        public byte IFAIsFifoTar7;
        public byte IFAIsFastSer7;
        public byte AIsVCP7;
        public byte IFBIsFifo7;
        public byte IFBIsFifoTar7;
        public byte IFBIsFastSer7;
        public byte BIsVCP7;
        public byte PowerSaveEnable;
        public byte PullDownEnable8;
        public byte SerNumEnable8;
        public byte ASlowSlew;
        public byte ASchmittInput;
        public byte ADriveCurrent;
        public byte BSlowSlew;
        public byte BSchmittInput;
        public byte BDriveCurrent;
        public byte CSlowSlew;
        public byte CSchmittInput;
        public byte CDriveCurrent;
        public byte DSlowSlew;
        public byte DSchmittInput;
        public byte DDriveCurrent;
        public byte ARIIsTXDEN;
        public byte BRIIsTXDEN;
        public byte CRIIsTXDEN;
        public byte DRIIsTXDEN;
        public byte AIsVCP8;
        public byte BIsVCP8;
        public byte CIsVCP8;
        public byte DIsVCP8;
        public byte PullDownEnableH;
        public byte SerNumEnableH;
        public byte ACSlowSlewH;
        public byte ACSchmittInputH;
        public byte ACDriveCurrentH;
        public byte ADSlowSlewH;
        public byte ADSchmittInputH;
        public byte ADDriveCurrentH;
        public byte Cbus0H;
        public byte Cbus1H;
        public byte Cbus2H;
        public byte Cbus3H;
        public byte Cbus4H;
        public byte Cbus5H;
        public byte Cbus6H;
        public byte Cbus7H;
        public byte Cbus8H;
        public byte Cbus9H;
        public byte IsFifoH;
        public byte IsFifoTarH;
        public byte IsFastSerH;
        public byte IsFT1248H;
        public byte FT1248CpolH;
        public byte FT1248LsbH;
        public byte FT1248FlowControlH;
        public byte IsVCPH;
        public byte PowerSaveEnableH;
    }
}