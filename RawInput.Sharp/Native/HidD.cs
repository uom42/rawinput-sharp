using System;
using System.Runtime.InteropServices;
using System.Text;


namespace UOM.WinAPI.Windows.RawInput.Native;


public static class HidD
{


	[DllImport ("hid", CharSet = CharSet.Unicode)]
	[return: MarshalAs (UnmanagedType.U1)]
	private static extern bool HidD_GetManufacturerString ( IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength );

	[DllImport ("hid", CharSet = CharSet.Unicode)]
	[return: MarshalAs (UnmanagedType.U1)]
	private static extern bool HidD_GetProductString ( IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength );

	[DllImport ("hid", CharSet = CharSet.Unicode)]
	[return: MarshalAs (UnmanagedType.U1)]
	private static extern bool HidD_GetSerialNumberString ( IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength );

	[DllImport ("hid")]
	[return: MarshalAs (UnmanagedType.U1)]
	private static extern bool HidD_GetPreparsedData ( IntPtr HidDeviceObject, out IntPtr PreparsedData );

	[DllImport ("hid")]
	[return: MarshalAs (UnmanagedType.U1)]
	private static extern bool HidD_FreePreparsedData ( IntPtr PreparsedData );



	public static bool TryOpenDevice ( string devicePath, out HidDeviceHandle device )
	{

		var deviceHandle = Kernel32.CreateFile (
			devicePath,
			0,
			System.IO.FileShare.Read | System.IO.FileShare.Write,
			null,
			System.IO.FileMode.Open,
			0,
			default).EnsureValid ();

		device = deviceHandle.IsInvalid
			? HidDeviceHandle.Zero
			: (HidDeviceHandle) deviceHandle.DangerousGetHandle ();

		return !deviceHandle.IsInvalid;
	}


	public static string? GetManufacturerString ( HidDeviceHandle device )
		=> device.DangerousGetHandle ()
		.GetString (HidD_GetManufacturerString);


	public static string? GetProductString ( HidDeviceHandle device )
		=> device.DangerousGetHandle ()
		.GetString (HidD_GetProductString);


	public static string? GetSerialNumberString ( HidDeviceHandle device )
		=> device.DangerousGetHandle ()
		.GetString (HidD_GetSerialNumberString);


	public static HidPreparsedData GetPreparsedData ( HidDeviceHandle device )
	{
		//var deviceHandle = device.DangerousGetHandle ();
		HidD_GetPreparsedData (device.DangerousGetHandle (), out var preparsedData);
		return (HidPreparsedData) preparsedData;
	}

	public static void FreePreparsedData ( HidPreparsedData preparsedData )
		=> HidD_FreePreparsedData ((IntPtr) preparsedData);


	private static string? GetString ( this IntPtr handle, Func<IntPtr, byte[], uint, bool> proc )
	{
		var buf = new byte[ 256 ];

		if (!proc (handle, buf, (uint) buf.Length))
			return null;

		var str = Encoding.Unicode.GetString (buf, 0, buf.Length);

		return str.Contains ("\0") ? str.Substring (0, str.IndexOf ('\0')) : str;
	}
}
