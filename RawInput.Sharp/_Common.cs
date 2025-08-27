using System;
using System.ComponentModel;
using System.Text;

using UOM.WinAPI.Windows.RawInput.Native;

namespace UOM.WinAPI.Windows.RawInput;


public static class Common
{


	internal static void EnsureSuccess ( this ConfigReturnValue result )
	{
		if (result != ConfigReturnValue.Success) throw new InvalidOperationException (result.ToString ());
	}




	internal static IntPtr EnsureValid ( this IntPtr handle )
	{
		if (handle == IntPtr.Zero) throw new Win32Exception ();
		return handle;
	}

	internal static Kernel32.SafeHINSTANCE EnsureValid ( this Kernel32.SafeHINSTANCE handle )
	{
		if (handle.IsInvalid) throw new Win32Exception ();
		return handle;
	}

	internal static Kernel32.SafeHFILE EnsureValid ( this Kernel32.SafeHFILE handle )
	{
		if (handle.IsInvalid) throw new Win32Exception ();
		return handle;
	}


	/*
	public static bool IsWow64Process ( this IntPtr hProcess )
	{
		if (!Kernel32.IsWow64Process (hProcess, out var result)) throw new Win32Exception ();
		return result;
	}
	 */



	/*
internal static string FormatErrorMessage ( this int errorCode )
{
var message = new StringBuilder (2048);
var charsWritten = Kernel32.FormatMessage (
Kernel32.FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM,
IntPtr.Zero,
(uint) errorCode,
0,
message,
(uint) message.Capacity,
IntPtr.Zero);

if (charsWritten == 0) throw new Win32Exception ();
return message.ToString ();

}

	 */
}