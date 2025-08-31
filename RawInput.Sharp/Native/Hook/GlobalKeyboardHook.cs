namespace UOM.WinAPI.Windows.RawInput.Native.Hook;


class GlobalKeyboardHook2
{

	/// <summary>The collections of keys to watch for</summary>
	public List<System.Windows.Forms.Keys> HookedKeys = [];

	/// <summary>Handle to the hook, need this to unhook and call the next hook</summary>
	private User32.SafeHHOOK _hHook = User32.SafeHHOOK.Null;


	#region Events


	/// <summary>Occurs when one of the hooked keys is pressed</summary>
	public event KeyEventHandler KeyDown = delegate { };

	/// <summary>Occurs when one of the hooked keys is released</summary>
	public event KeyEventHandler KeyUp = delegate { };


	#endregion


	#region Constructors and Destructors


	/// <summary>Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.</summary>
	public GlobalKeyboardHook2 ()
		=> Hook ();

	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the
	/// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
	/// </summary>
	~GlobalKeyboardHook2 ()
		=> UnHook ();


	#endregion



	#region Public Methods


	/// <summary>Installs the global hook</summary>
	public void Hook ()
	{
		var hInstance = Kernel32.LoadLibrary (Vanara.PInvoke.Lib.User32);
		_hHook = User32.SetWindowsHookEx (User32.HookType.WH_KEYBOARD_LL, LowLevelKeyboardProc, hInstance, 0);
	}

	/// <summary>Uninstalls the global hook</summary>
	public void UnHook ()
	{
		if (!_hHook.IsInvalid) User32.UnhookWindowsHookEx (_hHook);
		_hHook = User32.SafeHHOOK.Null;
	}

	/// <summary>
	/// The callback for the keyboard hook
	/// </summary>
	/// <param name="nCode">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
	/// <param name="wParam">The event type</param>
	/// <param name="lParam">The keyhook event information</param>
	/// <returns></returns>
	private unsafe IntPtr LowLevelKeyboardProc ( int nCode, IntPtr wParam, IntPtr lParam )
	{
		if (nCode >= 0)
		{
			if (OnHookCallback (nCode, (User32.WindowMessage) wParam, (User32.KBDLLHOOKSTRUCT*) lParam))
				return (nint) 1;
		}
		return User32.CallNextHookEx (_hHook, nCode, wParam, lParam);
	}

	private unsafe bool OnHookCallback ( int nCode, User32.WindowMessage msg, User32.KBDLLHOOKSTRUCT* kbd )
	{
		Keys key = (Keys) kbd->vkCode;
		if (HookedKeys.Contains (key))
		{
			KeyEventArgs kea = new (key);
			if (( msg == User32.WindowMessage.WM_KEYDOWN || msg == User32.WindowMessage.WM_SYSKEYDOWN ) && ( KeyDown != null ))
			{
				KeyDown?.Invoke (this, kea);
			}
			else if (( msg == User32.WindowMessage.WM_KEYUP || msg == User32.WindowMessage.WM_SYSKEYUP ) && ( KeyUp != null ))
			{
				KeyUp?.Invoke (this, kea);
			}
			return kea.Handled;
		}
		return false;
	}

	#endregion


}