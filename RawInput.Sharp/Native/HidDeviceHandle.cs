using System;


namespace UOM.WinAPI.Windows.RawInput.Native;



public class HidDeviceHandle ( IntPtr value ) : Kernel32.SafeHFILE (value, true), IEquatable<HidDeviceHandle>
{

	public static HidDeviceHandle Zero => (HidDeviceHandle) IntPtr.Zero;

	//public static IntPtr GetRawValue ( HidDeviceHandle handle )		=> handle.handle;


	public static explicit operator HidDeviceHandle ( IntPtr value ) => new (value);

	public static bool operator == ( HidDeviceHandle a, HidDeviceHandle b ) => a.Equals (b);

	public static bool operator != ( HidDeviceHandle a, HidDeviceHandle b ) => !a.Equals (b);



	public bool Equals ( HidDeviceHandle? other )
		=> handle.Equals (other?.handle);

	public override bool Equals ( object? obj ) =>
		obj is HidDeviceHandle other &&
		Equals (other);


	public override int GetHashCode () => handle.GetHashCode ();

	public override string ToString () => handle.ToString ();
}