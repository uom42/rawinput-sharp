using System;
using System.ComponentModel;
using System.Text;

using UOM.WinAPI.Windows.RawInput.Native;

namespace UOM.WinAPI.Windows.RawInput;


public static class Common
{


	internal static bool EnsureSuccess ( this bool result )
	{
		if (!result) throw new Win32Exception ();
		return result;
	}

	internal static uint EnsureSuccess ( this uint result )
	{
		if (result == unchecked((uint) -1)) throw new Win32Exception ();
		return result;
	}

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


}