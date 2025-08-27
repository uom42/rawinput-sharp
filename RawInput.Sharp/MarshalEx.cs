using System.Runtime.InteropServices;

namespace UOM.WinAPI.Windows.RawInput;

static class MarshalEx
{
#if NET7_0_OR_GREATER
    public static int SizeOf<T>() => Marshal.SizeOf<T>();
#else
    public static int SizeOf<T>() => Marshal.SizeOf(typeof(T));
#endif
}