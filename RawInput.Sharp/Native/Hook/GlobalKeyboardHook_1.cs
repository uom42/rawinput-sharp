namespace UOM.WinAPI.Windows.RawInput.Native.Hook;


public class GlobalKeyboardHook
{
	//public delegate IntPtr HookProc ( int nCode, IntPtr wParam, IntPtr lParam );

	private User32.SafeHHOOK _hHook = User32.SafeHHOOK.Null;

	~GlobalKeyboardHook () => UnHook ();


	public void SetHook ()
	{
		UnHook ();

		_hHook = SetHook (LowLevelKeyboardProc);
	}


	public void UnHook ()
	{
		if (!_hHook.IsInvalid) User32.UnhookWindowsHookEx (_hHook);
		_hHook = User32.SafeHHOOK.Null;
	}

	private User32.SafeHHOOK SetHook ( User32.HookProc proc )
	{
		using (System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess ())
		using (System.Diagnostics.ProcessModule currentModule = currentProcess.MainModule!)
		{
			var moduleHandle = Kernel32.GetModuleHandle (currentModule.ModuleName);
			return User32.SetWindowsHookEx (User32.HookType.WH_KEYBOARD_LL, proc, moduleHandle, 0);
		}
	}

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

		var result = User32.CallNextHookEx (_hHook, nCode, wParam, lParam);

		return result;
	}


	protected unsafe virtual bool OnHookCallback ( int nCode, User32.WindowMessage msg, User32.KBDLLHOOKSTRUCT* kb )
	{
		Console.WriteLine ();
		Console.WriteLine (@$"HookCallback: {nCode} {msg}
vkCode: {kb->vkCode}
scanCode: {kb->scanCode}
flags: {kb->flags}
time: {kb->time}
dwExtraInfo: {kb->dwExtraInfo}
"
);
		//Console.WriteLine ();



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