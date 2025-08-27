using System;

namespace Linearstar.Windows.RawInput.Native;

internal static class Kernel32_
{


	internal static Kernel32.SafeHFILE CreateFile (
			string fileName,
			System.IO.FileShare shareMode,
			System.IO.FileMode creationDisposition,
			Kernel32.FileAccess FileAccess = 0,
			FileFlagsAndAttributes flagsAndAttributes = 0,
			IntPtr templateFile = default )
			=> Kernel32.CreateFile (
				fileName,
				FileAccess,
				shareMode,
				null,
				creationDisposition,
				flagsAndAttributes,
				templateFile)
			.EnsureValid ();


	internal static bool TryCreateFile (
		string fileName,
		System.IO.FileShare shareMode,
		System.IO.FileMode creationDisposition,
		out Kernel32.SafeHFILE handle,
		Kernel32.FileAccess FileAccess = 0,
		FileFlagsAndAttributes flagsAndAttributes = 0,
		IntPtr templateFile = default )
	{
		handle = Kernel32.CreateFile (
			fileName,
			FileAccess,
			shareMode,
			null,
			creationDisposition,
			flagsAndAttributes,
			templateFile).EnsureValid ();

		return !handle.IsInvalid;
	}



	/*
	public static Kernel32.SafeHINSTANCE GetModuleHandle ( string moduleName )
		=> Kernel32.GetModuleHandle (moduleName)
		.EnsureValid ();


	public static IntPtr GetProcAddress ( IntPtr hModule, string procName )
		=> Kernel32.GetProcAddress (hModule, procName)
		.EnsureValid ();
	 */



}