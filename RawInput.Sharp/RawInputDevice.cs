using System;
using System.Linq;

using UOM.WinAPI.Windows.RawInput.Native;


namespace UOM.WinAPI.Windows.RawInput;


public abstract class RawInputDevice ( RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo )
{

	private bool _gotAttributes;
	private string? _productName;
	private string? _manufacturerName;
	private string? _serialNumber;


	protected RawInputDeviceInfo DeviceInfo { get; } = deviceInfo;

	public RawInputDeviceHandle Handle { get; } = device;

	public RawInputDeviceType DeviceType => DeviceInfo.Type;

	public string? DevicePath { get; } = User32_.GetRawInputDeviceName (device);


	public string? ManufacturerName
	{
		get
		{
			if (_manufacturerName == null) GetAttributesOnce ();
			return _manufacturerName;
		}
	}

	public string? ProductName
	{
		get
		{
			if (_productName == null) GetAttributesOnce ();
			return _productName;
		}
	}

	public string? SerialNumber
	{
		get
		{
			if (_serialNumber == null) GetAttributesOnce ();
			return _serialNumber;
		}
	}

	public bool IsConnected =>
		DevicePath != null && CfgMgr32.TryLocateDevNode (DevicePath, CfgMgr32.LocateDevNodeFlags.Normal, out _) == ConfigReturnValue.Success;

	public abstract HidUsageAndPage UsageAndPage { get; }
	public abstract int VendorId { get; }
	public abstract int ProductId { get; }


	private void GetAttributesOnce ()
	{
		if (_gotAttributes) return;
		_gotAttributes = true;

		if (DevicePath == null) return;
		GetAttributesFromHidD ();
		if (_manufacturerName == null || _productName == null) GetAttributesFromCfgMgr ();
	}

	private void GetAttributesFromHidD ()
	{
		if (DevicePath == null || !HidD.TryOpenDevice (DevicePath, out var device)) return;

		try
		{
			_manufacturerName ??= HidD.GetManufacturerString (device);
			_productName ??= HidD.GetProductString (device);
			_serialNumber ??= HidD.GetSerialNumberString (device);
		}
		finally
		{
			device.CloseDevice ();
		}
	}

	private void GetAttributesFromCfgMgr ()
	{
		if (DevicePath == null) return;

		var path = DevicePath.Substring (4).Replace ('#', '\\');
		if (path.Contains ("{")) path = path.Substring (0, path.IndexOf ('{') - 1);

		var device = CfgMgr32.LocateDevNode (path, CfgMgr32.LocateDevNodeFlags.Phantom);

		_manufacturerName ??= CfgMgr32.GetDevNodePropertyString (device, in DevicePropertyKey.DeviceManufacturer);
		_productName ??= CfgMgr32.GetDevNodePropertyString (device, in DevicePropertyKey.DeviceFriendlyName);
		_productName ??= CfgMgr32.GetDevNodePropertyString (device, in DevicePropertyKey.Name);
	}




	public static RawInputDevice FromHandle ( RawInputDeviceHandle device )
	{
		var deviceInfo = User32_.GetRawInputDeviceInfo (device);
		return deviceInfo.Type switch
		{
			RawInputDeviceType.Mouse => new RawInputMouse (device, deviceInfo),
			RawInputDeviceType.Keyboard => new RawInputKeyboard (device, deviceInfo),
			RawInputDeviceType.Hid => RawInputDigitizer.IsSupported (deviceInfo.Hid.UsageAndPage)
								? new RawInputDigitizer (device, deviceInfo)
								: new RawInputHid (device, deviceInfo),
			_ => throw new ArgumentException (),
		};
	}

	/// <summary>
	/// Gets available devices that can be handled with Raw Input.
	/// </summary>
	/// <returns>Array of <see cref="RawInputDevice"/>, which contains mouse as a <see cref="RawInputMouse"/>, keyboard as a <see cref="RawInputKeyboard"/>, and any other HIDs as a <see cref="RawInputHid"/>.</returns>
	public static RawInputDevice[] GetDevices ()
	{
		var devices = User32_.GetRawInputDeviceList ();

		return [ .. devices.Select (i => FromHandle ((RawInputDeviceHandle) i.hDevice.DangerousGetHandle ())) ];
	}

	public byte[] GetPreparsedData () =>
		User32_.GetRawInputDevicePreparsedData (Handle);

	public static void RegisterDevice ( HidUsageAndPage usageAndPage, RawInputDeviceFlags flags, IntPtr hWndTarget ) =>
		RegisterDevice (new RawInputDeviceRegistration (usageAndPage, flags, hWndTarget));

	public static void RegisterDevice ( params RawInputDeviceRegistration[] devices ) =>
		User32_.RegisterRawInputDevices (devices);

	public static void UnregisterDevice ( HidUsageAndPage usageAndPage ) =>
		RegisterDevice (usageAndPage, RawInputDeviceFlags.Remove, IntPtr.Zero);

	public static RawInputDeviceRegistration[] GetRegisteredDevices () =>
		User32_.GetRegisteredRawInputDevices ();

	public override string ToString ()
	{
		return @$"Type: {DeviceType}
Handle: {Handle}
VendorId: {VendorId:X}
ProductId: {ProductId:X}
ManufacturerName: {ManufacturerName}
ProductName: {ProductName}
SerialNumber: {SerialNumber}
DevicePath: {DevicePath}
*** DeviceInfo: {DeviceInfo}";

	}
}