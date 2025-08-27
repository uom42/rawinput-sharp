using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace Linearstar.Windows.RawInput.Native.Window;


public abstract class MessageOnlyWindow : User32.SafeHWND
{

	public MessageOnlyWindow ( User32.WindowProc? windowProc = null, string? windowClass = null ) : base ((nint) 0, true)
	{
		windowClass ??= "HelperWindowClass_" + Guid.NewGuid ().ToString ();

		var wind_class = new User32.WNDCLASS
		{
			lpszClassName = windowClass,
			lpfnWndProc = windowProc ?? this.WindowProc
		};

		var classAtom = User32.RegisterClass (wind_class);
		if (classAtom.IsInvalid) throw new Win32Exception ();

		var hwnd = User32.CreateWindowEx (
			User32.WindowStylesEx.WS_EX_NOACTIVATE,
			windowClass,
			"",
			User32.WindowStyles.WS_POPUP,
			0, 0, 0, 0,
			Linearstar.Windows.RawInput.Native.User32_.HWND_MESSAGE,
			IntPtr.Zero,
			IntPtr.Zero,
			IntPtr.Zero
		);

		if (hwnd.IsInvalid)
			throw new Win32Exception ();

		this.SetHandle (hwnd.DangerousGetHandle ());
	}


	protected virtual IntPtr WindowProc ( [In] HWND hWnd, [In] uint int_msg, [In] IntPtr wParam, [In] IntPtr lParam )
	{
		//User32.WindowMessage msg = (User32.WindowMessage) int_msg;
		// handle the messages here
		return User32.DefWindowProc (hWnd, (uint) int_msg, wParam, lParam);
	}
}
