using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;


namespace Linearstar.Windows.RawInput.Native.Window;


public class RawInputWindowReciever : MessageOnlyWindow
{

	private enum WM_INPUT_DEVICE_CHANGE_ACTIONS : uint
	{
		/// <summary>A new device has been added to the system. You can call GetRawInputDeviceInfo to get more information regarding the device.</summary>
		GIDC_ARRIVAL = 1,

		/// <summary>A device has been removed from the system.</summary>
		GIDC_REMOVAL = 2
	}

	internal const RawInputDeviceFlags DEFAULT_DEVICE_FLAGS = RawInputDeviceFlags.InputSink | RawInputDeviceFlags.DevNotify;


	private readonly Action<RawInputData>? _rawInputHandler;
	private readonly Action<RawInputMouseData>? _rawMouseInputHandler;
	private readonly Action<RawInputKeyboardData>? _rawKeyboardInputHandler;
	private readonly Action<RawInputHidData>? _rawHidInputHandler;
	private readonly Action<RawInputDevice>? _deviceAddHandler;
	private readonly Action? _deviceRemovedHandler;



	public RawInputWindowReciever (
		HidUsageAndPage usageAndPage,
		RawInputDeviceFlags flags = DEFAULT_DEVICE_FLAGS,
		Action<RawInputData>? rawInputHandler = null,
		Action<RawInputMouseData>? rawMouseInputHandler = null,
		Action<RawInputKeyboardData>? rawKeyboardInputHandler = null,
		Action<RawInputHidData>? rawHidInputHandler = null,
		Action<RawInputDevice>? deviceAddHandler = null,
		Action? deviceRemovedHandler = null
		) : base ()
	{
		_rawInputHandler = rawInputHandler;
		_rawMouseInputHandler = rawMouseInputHandler;
		_rawKeyboardInputHandler = rawKeyboardInputHandler;
		_rawHidInputHandler = rawHidInputHandler;
		_deviceAddHandler = deviceAddHandler;
		_deviceRemovedHandler = deviceRemovedHandler;

		// register the keyboard device and you can register device which you need like mouse
		RawInputDevice.RegisterDevice (usageAndPage, flags, (nint) this);
	}


	protected override nint WindowProc ( [In] HWND hWnd, [In] uint int_msg, [In] nint wParam, [In] nint lParam )
	{
		User32.WindowMessage msg = (User32.WindowMessage) int_msg;

		switch (msg)
		{
			case User32.WindowMessage.WM_INPUT:
				{
					// Create an RawInputData from the handle stored in lParam.
					RawInputData? data = RawInputData.FromHandle (lParam);
					OnRawInput (data);

					// You can identify the source device using Header.DeviceHandle or just Device.
					//var sourceDeviceHandle = data.Header.DeviceHandle;
					//var sourceDevice = data.Device;

					// The data will be an instance of either RawInputMouseData, RawInputKeyboardData, or RawInputHidData.
					// They contain the raw input data in their properties.
					switch (data)
					{
						case RawInputMouseData mouse:
							OnRawInputMouse (mouse);

							break;
						case RawInputKeyboardData keyboard:
							OnRawInputKeyboard (keyboard);
							break;

						case RawInputHidData hid:
							OnRawInputHid (hid);
							break;
					}
				}
				break;

			case User32.WindowMessage.WM_INPUT_DEVICE_CHANGE:
				{

					var wp = (WM_INPUT_DEVICE_CHANGE_ACTIONS) wParam;
					switch (wp)
					{
						case WM_INPUT_DEVICE_CHANGE_ACTIONS.GIDC_ARRIVAL:
							try
							{
								OnInputDeviceAdded (
									RawInputDevice.FromHandle ((Linearstar.Windows.RawInput.Native.RawInputDeviceHandle) lParam)
									);
							}
							catch { }//ignore any errors in message loop
							break;

						case WM_INPUT_DEVICE_CHANGE_ACTIONS.GIDC_REMOVAL:
							OnInputDeviceRemoved ();
							break;

						default:
							break;
					}

				}
				break;

			default:
				break;

		}

		// The normal way to quit is sending WM_CLOSE message to the window
		// if (msg == 0x0002) { // WM_DESTORY
		//     PostQuitMessage(0);
		//     return nint.Zero;
		// }

		// handle the messages here
		return base.WindowProc (hWnd, int_msg, wParam, lParam);
	}


	public static bool MessageLoopDoStep ()
	{
		if (User32.GetMessage (out var msg, IntPtr.Zero, 0, 0) != 0)
		{
			User32.TranslateMessage (msg);
			User32.DispatchMessage (msg);
			return true;
		}
		return false;
	}


	[MethodImpl (MethodImplOptions.NoOptimization)]
	public static void MessageLoopEnter ( CancellationTokenSource? ct = null )
	{
		// Message loop
		while (!( ct?.IsCancellationRequested ?? false ) && MessageLoopDoStep ())
		{
			_ = Environment.Is64BitProcess;
			//User32.TranslateMessage (msg);
			//User32.DispatchMessage (msg);
		}
	}


	#region onEvents


	protected virtual void OnRawInput ( RawInputData data )
	{
		_rawInputHandler?.Invoke (data);
		//Console.WriteLine (mouseInput.Mouse);
	}

	protected virtual void OnRawInputMouse ( RawInputMouseData mouseInput )
	{
		_rawMouseInputHandler?.Invoke (mouseInput);
		//Console.WriteLine (mouseInput.Mouse);
	}

	protected virtual void OnRawInputKeyboard ( RawInputKeyboardData keyboardInput )
	{
		_rawKeyboardInputHandler?.Invoke (keyboardInput);
		//Console.WriteLine ($"{keyboardInput.Keyboard} at {keyboardInput.Device}");
	}

	protected virtual void OnRawInputHid ( RawInputHidData hidnput )
	{
		//Console.WriteLine (hidnput.Hid);
		_rawHidInputHandler?.Invoke (hidnput);
	}

	protected virtual void OnInputDeviceAdded ( RawInputDevice device )
	{
		//Console.WriteLine ($"InputDeviceAdded: {device}");
		_deviceAddHandler?.Invoke (device);
	}

	protected virtual void OnInputDeviceRemoved ()
	{
		//Console.WriteLine ("InputDeviceRemoved");
		_deviceRemovedHandler?.Invoke ();
	}


	#endregion

}