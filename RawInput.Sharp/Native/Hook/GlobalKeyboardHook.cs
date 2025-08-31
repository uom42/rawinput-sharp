namespace UOM.WinAPI.Windows.RawInput.Native.Hook;


public class GlobalKeyboardHook
{

	#region Events


	/// <summary>Occurs when one of the hooked keys is pressed</summary>
	public event KeyEventHandler KeyDown = delegate { };

	/// <summary>Occurs when one of the hooked keys is released</summary>
	public event KeyEventHandler KeyUp = delegate { };


	#endregion

	/// <summary>Handle to the hook, need this to unhook and call the next hook</summary>
	private User32.SafeHHOOK _hHook = User32.SafeHHOOK.Null;


	/// <summary>The collections of keys to watch for</summary>
	public List<System.Windows.Forms.Keys> HookedKeys = [];



	#region Constructors and Destructors


	/// <summary>Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.</summary>
	public GlobalKeyboardHook ( bool hookNow = true )
	{
		if (hookNow) Hook ();
	}


	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the
	/// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
	/// </summary>
	~GlobalKeyboardHook ()
		=> UnHook ();


	#endregion



	/// <summary>Installs the global hook</summary>
	public void Hook ( User32.HookProc? lowLevelKeyboardProc = null )
	{
		UnHook ();

		using (System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess ())
		using (System.Diagnostics.ProcessModule currentModule = currentProcess.MainModule!)
		{
			var moduleHandle = Kernel32.GetModuleHandle (currentModule.ModuleName);
			//_hHook = User32.SetWindowsHookEx (User32.HookType.WH_KEYBOARD_LL, LowLevelKeyboardProc, hInstance, 0);
			_hHook = User32.SetWindowsHookEx (User32.HookType.WH_KEYBOARD_LL, lowLevelKeyboardProc ??= LowLevelKeyboardProc, moduleHandle, 0);
		}
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
	private unsafe IntPtr LowLevelKeyboardProc ( int nCode, IntPtr wParam, IntPtr lParam )
	{

		//Если значение nCode меньше нуля, процедура перехватчика должна передать сообщение функции CallNextHookEx без дальнейшей обработки и возвращать значение, возвращаемое CallNextHookEx.
		if (nCode >= 0)
		{
			if (OnHookCallback (nCode, (User32.WindowMessage) wParam, (User32.KBDLLHOOKSTRUCT*) lParam))
				return (nint) 1;

			//if (msg == User32.WindowMessage.WM_KEYDOWN)
			{
				// Block the key press
				//return (IntPtr) 1;
			}
		}

		return User32.CallNextHookEx (_hHook, nCode, wParam, lParam);
	}



	protected unsafe virtual bool OnHookCallback ( int nCode, User32.WindowMessage msg, User32.KBDLLHOOKSTRUCT* kbd )
	{
		Console.WriteLine ();
		Console.WriteLine (@$"HookCallback: {nCode} {msg}
vkCode: {kbd->vkCode}
scanCode: {kbd->scanCode}
flags: {kbd->flags}
time: {kbd->time}
dwExtraInfo: {kbd->dwExtraInfo}
"
);
		//Console.WriteLine ();


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



}








/*






using System;
using System.Windows.Forms;

public partial class MainForm : Form
{
	private KeyboardHook hook = new KeyboardHook ();
	private bool isHooked = false;

	public MainForm ()
	{
		InitializeComponent ();
	}

	private void btnHook_Click ( object sender, EventArgs e )
	{
		if (!isHooked)
		{
			hook.SetHook ();
			isHooked = true;
			Console.WriteLine ("Keyboard hook set.");
		}
	}

	private void btnUnhook_Click ( object sender, EventArgs e )
	{
		if (isHooked)
		{
			hook.UnHook ();
			isHooked = false;
			Console.WriteLine ("Keyboard hook unset.");
		}
	}
}



Key points:

The SetHook method sets the low-level keyboard hook.
The Unhook method uninstalls the hook.
The HookCallback method is called whenever a key is pressed. By returning (IntPtr)1, you block the key press from being processed further.
Choosing the Right Method
Specific Control: Use control.Enabled = false; for simple cases.
Application-Wide (Simple): Use IMessageFilter for a straightforward approach to block input.
Application-Wide (Advanced): Use low-level keyboard hooks for precise, system-wide control, but be aware of the added complexity.
Remember to handle these techniques carefully, especially the global input blocking methods, as they can affect the user experience if not implemented correctly.

I hope this helps you disable keyboard input in your C# application effectively!

Here are some related questions you might find interesting:

*/