using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace UOM.WinAPI.Windows.RawInput.Native.Window;



public abstract class MessageOnlyWindow : User32.SafeHWND
{

	private Vanara.PInvoke.BasicMessageWindow _bmw;

	public MessageOnlyWindow ( User32.WindowProc? windowProc = null, string? windowClass = null ) : base ((nint) 0, true)
	{

		_bmw = new BasicMessageWindow (this.windowProc);

		if (_bmw.IsInvalid)
			throw new Win32Exception ();

		this.SetHandle ((nint) _bmw.Handle);
	}


	private bool windowProc ( HWND hwnd, uint msg, IntPtr wParam, IntPtr lParam, out IntPtr lReturn )
	{
		lReturn = WindowProc (hwnd, msg, wParam, lParam);
		return false;
	}

	protected virtual IntPtr WindowProc ( [In] HWND hWnd, [In] uint int_msg, [In] IntPtr wParam, [In] IntPtr lParam )
	{
		//User32.WindowMessage msg = (User32.WindowMessage) int_msg;
		// handle the messages here
		return IntPtr.Zero;// User32.DefWindowProc (hWnd, (uint) int_msg, wParam, lParam);
	}

	protected override bool ReleaseHandle ()
	{
		_bmw.Dispose ();
		//return base.ReleaseHandle ();
		return true;
	}

}
