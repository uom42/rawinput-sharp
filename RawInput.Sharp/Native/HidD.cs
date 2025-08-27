using System;
using System.Runtime.InteropServices;
using System.Text;


namespace UOM.WinAPI.Windows.RawInput.Native;


public static class HidD
{


	[DllImport ("hid", CharSet = CharSet.Unicode)]
	[return: MarshalAs (UnmanagedType.U1)]
	static extern bool HidD_GetManufacturerString ( IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength );

	[DllImport ("hid", CharSet = CharSet.Unicode)]
	[return: MarshalAs (UnmanagedType.U1)]
	static extern bool HidD_GetProductString ( IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength );

	[DllImport ("hid", CharSet = CharSet.Unicode)]
	[return: MarshalAs (UnmanagedType.U1)]
	static extern bool HidD_GetSerialNumberString ( IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength );

	[DllImport ("hid")]
	[return: MarshalAs (UnmanagedType.U1)]
	static extern bool HidD_GetPreparsedData ( IntPtr HidDeviceObject, out IntPtr PreparsedData );

	[DllImport ("hid")]
	[return: MarshalAs (UnmanagedType.U1)]
	private static extern bool HidD_FreePreparsedData ( IntPtr PreparsedData );


	public static HidDeviceHandle OpenDevice ( string devicePath )
	{
		var deviceHandle = Kernel32_.CreateFile (devicePath, System.IO.FileShare.Read | System.IO.FileShare.Write, System.IO.FileMode.Open);
		return (HidDeviceHandle) deviceHandle.DangerousGetHandle ();
	}

	public static bool TryOpenDevice ( string devicePath, out HidDeviceHandle device )
	{
		if (!Kernel32_.TryCreateFile (
				devicePath,
				System.IO.FileShare.Read | System.IO.FileShare.Write,
				System.IO.FileMode.Open,
				out var deviceHandle))
		{
			device = HidDeviceHandle.Zero;
			return false;
		}

		device = (HidDeviceHandle) deviceHandle.DangerousGetHandle ();
		return true;
	}


	public static void CloseDevice ( HidDeviceHandle device )
	{
		var deviceHandle = HidDeviceHandle.GetRawValue (device);
		Kernel32.CloseHandle (deviceHandle);
	}

	public static string? GetManufacturerString ( HidDeviceHandle device )
		=> HidDeviceHandle.GetRawValue (device)
		.GetString (HidD_GetManufacturerString);


	public static string? GetProductString ( HidDeviceHandle device )
		=> HidDeviceHandle.GetRawValue (device).
			GetString (HidD_GetProductString);


	public static string? GetSerialNumberString ( HidDeviceHandle device )
		=> HidDeviceHandle.GetRawValue (device)
		.GetString (HidD_GetSerialNumberString);


	public static HidPreparsedData GetPreparsedData ( HidDeviceHandle device )
	{
		var deviceHandle = HidDeviceHandle.GetRawValue (device);
		HidD_GetPreparsedData (deviceHandle, out var preparsedData);
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
