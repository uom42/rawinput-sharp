﻿using System.Runtime.InteropServices;

namespace UOM.WinAPI.Windows.RawInput.Native;

[StructLayout(LayoutKind.Sequential)]
public struct HidPCapsRange
{
    public ushort UsageMin;
    public ushort UsageMax;
    public ushort StringMin;
    public ushort StringMax;
    public ushort DesignatorMin;
    public ushort DesignatorMax;
    public ushort DataIndexMin;
    public ushort DataIndexMax;
}