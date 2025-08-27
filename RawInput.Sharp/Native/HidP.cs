using System;
using System.Runtime.InteropServices;

namespace UOM.WinAPI.Windows.RawInput.Native;

public static class HidP
{
	[DllImport ("hid")]
	static extern Vanara.PInvoke.NTStatus HidP_GetCaps ( IntPtr preparsedData, out HidPCaps capabilities );

	[DllImport ("hid")]
	static extern Vanara.PInvoke.NTStatus HidP_GetButtonCaps ( HidPReportType reportType, [Out] HidPButtonCaps[] buttonCaps, ref ushort buttonCapsLength, IntPtr preparsedData );

	[DllImport ("hid")]
	static extern Vanara.PInvoke.NTStatus HidP_GetValueCaps ( HidPReportType reportType, [Out] HidPValueCaps[] valueCaps, ref ushort valueCapsLength, IntPtr preparsedData );

	[DllImport ("hid")]
	static extern Vanara.PInvoke.NTStatus HidP_GetUsages ( HidPReportType reportType, ushort usagePage, ushort linkCollection, [Out] ushort[]? usageList, ref uint usageLength, IntPtr preparsedData, byte[] report, uint reportLength );

	[DllImport ("hid")]
	static extern Vanara.PInvoke.NTStatus HidP_GetUsageValue ( HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, byte[] report, uint reportLength );

	[DllImport ("hid")]
	static extern Vanara.PInvoke.NTStatus HidP_GetScaledUsageValue ( HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, byte[] report, uint reportLength );

	[DllImport ("hid")]
	static extern Vanara.PInvoke.NTStatus HidP_GetUsageValueArray ( HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, [Out] byte[] usageValue, ushort usageValueByteLength, IntPtr preparsedData, byte[] report, uint reportLength );

	public static Vanara.PInvoke.NTStatus TryGetCaps ( IntPtr preparsedData, out HidPCaps capabilities ) =>
		HidP_GetCaps (preparsedData, out capabilities);

	public static Vanara.PInvoke.NTStatus TryGetCaps ( HidPreparsedData preparsedData, out HidPCaps capabilities ) =>
		TryGetCaps ((IntPtr) preparsedData, out capabilities);

	public static HidPCaps GetCaps ( IntPtr preparsedData )
	{
		TryGetCaps (preparsedData, out var capabilities)
			.ThrowIfFailed ();
		return capabilities;
	}

	public static HidPCaps GetCaps ( HidPreparsedData preparsedData ) =>
		GetCaps ((IntPtr) preparsedData);

	public static Vanara.PInvoke.NTStatus TryGetButtonCaps ( IntPtr preparsedData, HidPReportType reportType, out HidPButtonCaps[] buttonCaps )
	{
		var caps = GetCaps (preparsedData);
		var capsCount = reportType switch
		{
			HidPReportType.Input => caps.NumberInputButtonCaps,
			HidPReportType.Output => caps.NumberOutputButtonCaps,
			HidPReportType.Feature => caps.NumberFeatureButtonCaps,
			_ => throw new ArgumentException ($"Invalid HidPReportType: {reportType}", nameof (reportType)),
		};

		buttonCaps = new HidPButtonCaps[ capsCount ];

		return HidP_GetButtonCaps (reportType, buttonCaps, ref capsCount, preparsedData);
	}

	public static Vanara.PInvoke.NTStatus TryGetButtonCaps ( HidPreparsedData preparsedData, HidPReportType reportType, out HidPButtonCaps[] buttonCaps ) =>
		TryGetButtonCaps ((IntPtr) preparsedData, reportType, out buttonCaps);

	public static HidPButtonCaps[] GetButtonCaps ( IntPtr preparsedData, HidPReportType reportType )
	{
		TryGetButtonCaps (preparsedData, reportType, out var buttonCaps)
			.ThrowIfFailed ();

		return buttonCaps;
	}

	public static HidPButtonCaps[] GetButtonCaps ( HidPreparsedData preparsedData, HidPReportType reportType ) =>
		GetButtonCaps ((IntPtr) preparsedData, reportType);

	public static Vanara.PInvoke.NTStatus TryGetValueCaps ( IntPtr preparsedData, HidPReportType reportType, out HidPValueCaps[] valueCaps )
	{
		var caps = GetCaps (preparsedData);
		var capsCount = reportType switch
		{
			HidPReportType.Input => caps.NumberInputValueCaps,
			HidPReportType.Output => caps.NumberOutputValueCaps,
			HidPReportType.Feature => caps.NumberFeatureValueCaps,
			_ => throw new ArgumentException ($"Invalid HidPReportType: {reportType}", nameof (reportType)),
		};

		valueCaps = new HidPValueCaps[ capsCount ];

		return HidP_GetValueCaps (reportType, valueCaps, ref capsCount, preparsedData);
	}

	public static Vanara.PInvoke.NTStatus TryGetValueCaps ( HidPreparsedData preparsedData, HidPReportType reportType, out HidPValueCaps[] valueCaps ) =>
		TryGetValueCaps ((IntPtr) preparsedData, reportType, out valueCaps);

	public static HidPValueCaps[] GetValueCaps ( IntPtr preparsedData, HidPReportType reportType )
	{
		TryGetValueCaps (preparsedData, reportType, out var valueCaps)
			.ThrowIfFailed ();

		return valueCaps;
	}

	public static HidPValueCaps[] GetValueCaps ( HidPreparsedData preparsedData, HidPReportType reportType ) =>
		GetValueCaps ((IntPtr) preparsedData, reportType);

	public static Vanara.PInvoke.NTStatus TryGetUsages ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength, out ushort[] usageList )
	{
		uint usageCount = 0;

		HidP_GetUsages (reportType, usagePage, linkCollection, null, ref usageCount, preparsedData, report, (uint) reportLength);

		usageList = new ushort[ usageCount ];

		return HidP_GetUsages (reportType, usagePage, linkCollection, usageList, ref usageCount, preparsedData, report, (uint) reportLength);
	}

	public static Vanara.PInvoke.NTStatus TryGetUsages ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength, out ushort[] usageList ) =>
		TryGetUsages ((IntPtr) preparsedData, reportType, usagePage, linkCollection, report, reportLength, out usageList);

	public static Vanara.PInvoke.NTStatus TryGetUsages ( IntPtr preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength, out ushort[] usageList ) =>
		TryGetUsages (preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength, out usageList);

	public static Vanara.PInvoke.NTStatus TryGetUsages ( HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength, out ushort[] usageList ) =>
		TryGetUsages (preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength, out usageList);

	public static ushort[] GetUsages ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength )
	{
		TryGetUsages (preparsedData, reportType, usagePage, linkCollection, report, reportLength, out var usageList)
			.ThrowIfFailed ();

		return usageList;
	}

	public static ushort[] GetUsages ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength ) =>
		GetUsages ((IntPtr) preparsedData, reportType, usagePage, linkCollection, report, reportLength);

	public static ushort[] GetUsages ( IntPtr preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength ) =>
		GetUsages (preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength);

	public static ushort[] GetUsages ( HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength ) =>
		GetUsages (preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength);

	public static Vanara.PInvoke.NTStatus TryGetUsageValue ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		HidP_GetUsageValue (reportType, usagePage, linkCollection, usage, out usageValue, preparsedData, report, (uint) reportLength);

	public static Vanara.PInvoke.NTStatus TryGetUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		TryGetUsageValue ((IntPtr) preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out usageValue);

	public static Vanara.PInvoke.NTStatus TryGetUsageValue ( IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		TryGetUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

	public static Vanara.PInvoke.NTStatus TryGetUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		TryGetUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

	public static int GetUsageValue ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength )
	{
		TryGetUsageValue (preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue)
			.ThrowIfFailed ();

		return usageValue;
	}

	public static int GetUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength ) =>
		GetUsageValue ((IntPtr) preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength);

	public static int GetUsageValue ( IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength ) =>
		GetUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

	public static int GetUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength ) =>
		GetUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

	public static Vanara.PInvoke.NTStatus TryGetScaledUsageValue ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		HidP_GetScaledUsageValue (reportType, usagePage, linkCollection, usage, out usageValue, preparsedData, report, (uint) reportLength);

	public static Vanara.PInvoke.NTStatus TryGetScaledUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		TryGetScaledUsageValue ((IntPtr) preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out usageValue);

	public static Vanara.PInvoke.NTStatus TryGetScaledUsageValue ( IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		TryGetScaledUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

	public static Vanara.PInvoke.NTStatus TryGetScaledUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue ) =>
		TryGetScaledUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

	public static int GetScaledUsageValue ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength )
	{
		TryGetScaledUsageValue (preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue)
			.ThrowIfFailed ();

		return usageValue;
	}

	public static int GetScaledUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength ) =>
		GetScaledUsageValue ((IntPtr) preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength);

	public static int GetScaledUsageValue ( IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength ) =>
		GetScaledUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

	public static int GetScaledUsageValue ( HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength ) =>
		GetScaledUsageValue (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

	public static Vanara.PInvoke.NTStatus TryGetUsageValueArray ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength, out byte[] usageValue )
	{
		usageValue = new byte[ usageValueByteLength ];

		return HidP_GetUsageValueArray (reportType, usagePage, linkCollection, usage, usageValue, usageValueByteLength, preparsedData, report, (uint) reportLength);
	}

	public static Vanara.PInvoke.NTStatus TryGetUsageValueArray ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength, out byte[] usageValue ) =>
		TryGetUsageValueArray ((IntPtr) preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength, out usageValue);

	public static Vanara.PInvoke.NTStatus TryGetUsageValueArray ( IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out byte[] usageValue ) =>
		TryGetUsageValueArray (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort) ( valueCaps.BitSize * valueCaps.ReportCount ), report, reportLength, out usageValue);

	public static Vanara.PInvoke.NTStatus TryGetUsageValueArray ( HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out byte[] usageValue ) =>
		TryGetUsageValueArray (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort) ( valueCaps.BitSize * valueCaps.ReportCount ), report, reportLength, out usageValue);

	public static byte[] GetUsageValueArray ( IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength )
	{
		TryGetUsageValueArray (preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength, out var usageValue)
			.ThrowIfFailed ();

		return usageValue;
	}

	public static byte[] GetUsageValueArray ( HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength ) =>
		GetUsageValueArray ((IntPtr) preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength);

	public static byte[] GetUsageValueArray ( IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength ) =>
		GetUsageValueArray (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort) ( valueCaps.BitSize * valueCaps.ReportCount ), report, reportLength);

	public static byte[] GetUsageValueArray ( HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength ) =>
		GetUsageValueArray (preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort) ( valueCaps.BitSize * valueCaps.ReportCount ), report, reportLength);


}