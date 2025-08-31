using UOM.WinAPI.Windows.RawInput;


const string DEVICE_MFG_MIFARE_READER_LKD = "Sycreader";


using RawInputWindowReciever mowRawInput = new (
	HidUsageAndPage.Keyboard,
	rawKeyboardInputHandler: kbd =>
	{
		//if (!( kbd.Device?.ManufacturerName ?? string.Empty ).Equals (DEVICE_MFG_MIFARE_READER_LKD)) return;

		Console.WriteLine ();
		Console.WriteLine ($"RAW Input: {kbd.Keyboard} at {kbd.Device}");

	},
	deviceAddHandler: dev =>
	{
		//if (!( dev.ManufacturerName ?? string.Empty ).Equals (DEVICE_MFG_MIFARE_READER_LKD)) return;
		Console.WriteLine ();
		Console.WriteLine ($"RAW Input: InputDeviceAdded: {dev}");
	}
	, deviceRemovedHandler: () =>
	{
		Console.WriteLine ();
		Console.WriteLine ("RAW Input: InputDeviceRemoved");
	}

	);


// Get the devices that can be handled with Raw Input.
var devices = RawInputDevice.GetDevices ();

UOM.WinAPI.Windows.RawInput.Native.Hook.GlobalKeyboardHook kh = new (true);
RawInputWindowReciever.MessageLoopEnter ();

/*
// Message loop
while (User32.GetMessage (out var msg, IntPtr.Zero, 0, 0) != 0)
{
	User32.TranslateMessage (msg);
	User32.DispatchMessage (msg);
}
 */

