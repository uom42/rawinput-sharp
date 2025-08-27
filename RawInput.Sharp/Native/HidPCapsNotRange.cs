using System.Runtime.InteropServices;

namespace UOM.WinAPI.Windows.RawInput.Native;

[StructLayout(LayoutKind.Sequential)]
public struct HidPCapsNotRange
{
    public ushort Usage;
    readonly ushort Reserved1;
    public ushort StringIndex;
    readonly ushort Reserved2;
    public ushort DesignatorIndex;
    readonly ushort Reserved3;
    public ushort DataIndex;
    readonly ushort Reserved4;
}