using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KrokoshaCasualtiesMP;

public static class win32windowtitlechanger
{
	public static bool changed_the_title = false;

	private static IntPtr hwnd = IntPtr.Zero;

	public static void SetTitle(string newname)
	{
		if (hwnd == IntPtr.Zero)
		{
			hwnd = Process.GetCurrentProcess().MainWindowHandle;
		}
		if (!(hwnd == IntPtr.Zero))
		{
			SetWindowText(hwnd, newname);
			changed_the_title = true;
		}
	}

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern bool SetWindowText(IntPtr hwnd, string lpString);
}
