using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace UOM.WinAPI.Windows.RawInput.Native;


public static class User32_
{


	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern uint GetRawInputDeviceInfo ( IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, IntPtr pData, out uint pcbSize );


	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true, CharSet = CharSet.Unicode)]
	internal static extern uint GetRawInputDeviceInfo ( IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, StringBuilder pData, in uint pcbSize );

	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern uint GetRawInputDeviceInfo ( IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, out RawInputDeviceInfo pData, in uint pcbSize );


	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern bool RegisterRawInputDevices ( RawInputDeviceRegistration[] pRawInputDevices, uint uiNumDevices, uint cbSize );

	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern uint GetRegisteredRawInputDevices ( [Out] RawInputDeviceRegistration[]? pRawInputDevices, ref uint puiNumDevices, uint cbSize );

	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern uint GetRawInputData ( IntPtr hRawInput, User32.RID uiBehavior, IntPtr pData, ref uint pcbSize, uint cbSizeHeader );


	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern uint GetRawInputData ( IntPtr hRawInput, User32.RID uiBehavior, out RawInputHeader pData, ref uint pcbSize, uint cbSizeHeader );


	/*

	
	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern uint GetRawInputDeviceInfo ( IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, [Out] byte[] pData, in uint pcbSize );


	[DllImport (Vanara.PInvoke.Lib.User32, SetLastError = true)]
	internal static extern IntPtr DefRawInputProc ( byte[] paRawInput, int nInput, uint cbSizeHeader );
  
	 */

	public static User32.RAWINPUTDEVICELIST[] GetRawInputDeviceList ()
	{
		var size = (uint) MarshalEx.SizeOf<User32.RAWINPUTDEVICELIST> ();

		// Get device count by passing null for pRawInputDeviceList.
		uint deviceCount = 0;
		_ = User32.GetRawInputDeviceList (null, ref deviceCount, size);

		// Now, fill the buffer using the device count.
		var devices = new User32.RAWINPUTDEVICELIST[ deviceCount ];
		User32.GetRawInputDeviceList (devices, ref deviceCount, size)
			.EnsureSuccess ();

		return devices;
	}

	public static string? GetRawInputDeviceName ( RawInputDeviceHandle device )
	{
		var deviceHandle = RawInputDeviceHandle.GetRawValue (device);

		// Get the length of the device name first.
		// For RIDI_DEVICENAME, the value in the pcbSize is the character count instead of the byte count.
		_ = GetRawInputDeviceInfo (deviceHandle, RawInputDeviceInfoBehavior.DeviceName, IntPtr.Zero, out var size);

		if (size <= 2) return null;

		var sb = new StringBuilder ((int) size);
		GetRawInputDeviceInfo (deviceHandle, RawInputDeviceInfoBehavior.DeviceName, sb, in size).EnsureSuccess ();

		return sb.ToString ();
	}

	public static RawInputDeviceInfo GetRawInputDeviceInfo ( RawInputDeviceHandle device )
	{
		var deviceHandle = RawInputDeviceHandle.GetRawValue (device);
		var size = (uint) MarshalEx.SizeOf<RawInputDeviceInfo> ();

		GetRawInputDeviceInfo (deviceHandle, RawInputDeviceInfoBehavior.DeviceInfo, out var deviceInfo, in size).EnsureSuccess ();

		return deviceInfo;
	}

	public static unsafe byte[] GetRawInputDevicePreparsedData ( RawInputDeviceHandle device )
	{
		var deviceHandle = RawInputDeviceHandle.GetRawValue (device);

		_ = GetRawInputDeviceInfo (deviceHandle, RawInputDeviceInfoBehavior.PreparsedData, IntPtr.Zero, out var size);

		if (size == 0) return [];

		var rt = new byte[ size ];
		fixed (byte* ptr = rt)
			User32.GetRawInputDeviceInfo (deviceHandle, (uint) RawInputDeviceInfoBehavior.PreparsedData, (nint) ptr, ref size)
				.EnsureSuccess ();

		return rt;
	}

	public static void RegisterRawInputDevices ( params RawInputDeviceRegistration[] devices )
		=> RegisterRawInputDevices (devices, (uint) devices.Length, (uint) MarshalEx.SizeOf<RawInputDeviceRegistration> ())
		.EnsureSuccess ();


	public static RawInputDeviceRegistration[] GetRegisteredRawInputDevices ()
	{
		var size = (uint) MarshalEx.SizeOf<RawInputDeviceRegistration> ();

		uint count = 0;
		_ = GetRegisteredRawInputDevices (null, ref count, size);

		var rt = new RawInputDeviceRegistration[ count ];
		GetRegisteredRawInputDevices (rt, ref count, size).EnsureSuccess ();

		return rt;
	}

	public static RawInputHeader GetRawInputDataHeader ( RawInputHandle rawInput )
	{
		var hRawInput = RawInputHandle.GetRawValue (rawInput);
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();
		var size = headerSize;

		GetRawInputData (hRawInput, User32.RID.RID_HEADER, out var header, ref size, headerSize)
			.EnsureSuccess ();

		return header;
	}

	public static uint GetRawInputDataSize ( RawInputHandle rawInput )
	{
		var hRawInput = RawInputHandle.GetRawValue (rawInput);
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();
		uint size = 0;

		GetRawInputData (hRawInput, User32.RID.RID_INPUT, IntPtr.Zero, ref size, headerSize);

		return size;
	}

	public static void GetRawInputData ( RawInputHandle rawInput, IntPtr ptr, uint size )
	{
		var hRawInput = RawInputHandle.GetRawValue (rawInput);
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();

		GetRawInputData (hRawInput, User32.RID.RID_INPUT, ptr, ref size, headerSize).EnsureSuccess ();
	}

	public static unsafe RawMouse GetRawInputMouseData ( RawInputHandle rawInput, out RawInputHeader header )
	{
		var size = GetRawInputDataSize (rawInput);
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();
		var bytes = new byte[ size ];

		fixed (byte* bytesPtr = bytes)
		{
			GetRawInputData (rawInput, (IntPtr) bytesPtr, size);

			header = *(RawInputHeader*) bytesPtr;

			return *(RawMouse*) ( bytesPtr + headerSize );
		}
	}

	public static unsafe RawKeyboard GetRawInputKeyboardData ( RawInputHandle rawInput, out RawInputHeader header )
	{
		var size = GetRawInputDataSize (rawInput);
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();
		var bytes = new byte[ size ];

		fixed (byte* bytesPtr = bytes)
		{
			GetRawInputData (rawInput, (IntPtr) bytesPtr, size);

			header = *(RawInputHeader*) bytesPtr;

			return *(RawKeyboard*) ( bytesPtr + headerSize );
		}
	}

	public static unsafe RawHid GetRawInputHidData ( RawInputHandle rawInput, out RawInputHeader header )
	{
		var size = GetRawInputDataSize (rawInput);
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();
		var bytes = new byte[ size ];

		fixed (byte* bytesPtr = bytes)
		{
			GetRawInputData (rawInput, (IntPtr) bytesPtr, size);

			header = *(RawInputHeader*) bytesPtr;

			return RawHid.FromPointer (bytesPtr + headerSize);
		}
	}

	public static uint GetRawInputBufferSize ()
	{
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();
		uint size = 0;
		_ = User32.GetRawInputBuffer (IntPtr.Zero, ref size, headerSize);
		return size;
	}

	public static uint GetRawInputBuffer ( IntPtr ptr, uint size )
	{
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();
		return User32.GetRawInputBuffer (ptr, ref size, headerSize)
			.EnsureSuccess ();
	}

	public static unsafe void DefRawInputProc ( byte[] paRawInput )
	{
		var headerSize = (uint) MarshalEx.SizeOf<RawInputHeader> ();

		//fixed (void* ptr = paRawInput)
		{
			//User32.RAWINPUT[]* ss = (User32.RAWINPUT[]*) ptr;
			User32.DefRawInputProc (( (User32.RAWINPUT[]*) &paRawInput )[ 0 ], paRawInput.Length, headerSize);
		}
	}


}