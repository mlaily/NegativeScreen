//Copyright 2011-2012 Melvyn Laily
//http://arcanesanctum.net

//This file is part of NegativeScreen.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace NegativeScreen
{

	/// <summary>
	/// based on http://delphi32.blogspot.com/2010/09/windows-magnification-api-net.html
	/// </summary>
	internal static class NativeMethods
	{

		#region "User32.dll"
		/// <summary>
		/// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
		/// (SetWindowPos() parameter)
		/// </summary>
		public static IntPtr HWND_TOPMOST = new IntPtr(-1);

		/// <summary>
		/// Places the window at the top of the Z order.
		/// (SetWindowPos() parameter)
		/// </summary>
		public static IntPtr HWND_TOP = new IntPtr(0);

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ms633545%28v=vs.85%29.aspx
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="hWndInsertAfter"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="cx"></param>
		/// <param name="cy"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

		/// <summary>
		/// Sets a new extended window style.
		/// (SetWindowLong() parameter)
		/// </summary>
		public const int GWL_EXSTYLE = -20;
		/// <summary>
		/// Sets a new window style.
		/// (SetWindowLong() parameter)
		/// </summary>
		public const int GWL_STYLE = -16;

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ms633591%28v=vs.85%29.aspx
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="nIndex">GWL_EXSTYLE, GWL_STYLE, [...]</param>
		/// <param name="dwNewLong"></param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ms632680%28v=vs.85%29.aspx
		/// </summary>
		/// <param name="dwExStyle"></param>
		/// <param name="lpClassName"></param>
		/// <param name="lpWindowName"></param>
		/// <param name="dwStyle"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="nWidth"></param>
		/// <param name="nHeight"></param>
		/// <param name="hWndParent"></param>
		/// <param name="hMenu"></param>
		/// <param name="hInstance"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		[DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		public extern static IntPtr CreateWindowEx(
			int dwExStyle,
			string lpClassName,
			string lpWindowName,
			int dwStyle,
			int x,
			int y,
			int nWidth,
			int nHeight,
			IntPtr hWndParent,
			IntPtr hMenu,
			IntPtr hInstance,
			IntPtr lParam);

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ms633540%28v=vs.85%29.aspx
		/// </summary>
		/// <param name="hwnd"></param>
		/// <param name="crKey"></param>
		/// <param name="bAlpha"></param>
		/// <param name="dwFlags"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, LayeredWindowAttributeFlags dwFlags);

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/dd145002%28v=vs.85%29.aspx
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="rect"></param>
		/// <param name="erase"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InvalidateRect(IntPtr hWnd, IntPtr rect, [MarshalAs(UnmanagedType.Bool)] bool erase);

		/// <summary>
		/// </summary>
		/// <param name="hWnd">handle to window</param>
		/// <param name="id">hot key identifier</param>
		/// <param name="fsModifiers">key-modifier options</param>
		/// <param name="vk">virtual-key code</param>
		/// <returns></returns>
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

		/// <summary>
		/// </summary>
		/// <param name="hWnd">handle to window</param>
		/// <param name="id">hot key identifier</param>
		/// <returns></returns>
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ms633543.aspx
		/// </summary>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern bool SetProcessDPIAware();

		/// <summary>
		/// Undocumented function.
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetMagnificationDesktopMagnification(double scale, int x, int y);

		/// <summary>
		/// Undocumented function.
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMagnificationDesktopMagnification(out double scale, out int x, out int y);

		/// <summary>
		/// Undocumented function.
		/// </summary>
		/// <param name="pEffect"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMagnificationDesktopColorEffect(out ColorEffect pEffect);

		/// <summary>
		/// Undocumented function.
		/// </summary>
		/// <param name="pEffect"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetMagnificationDesktopColorEffect(ref ColorEffect pEffect);

		#endregion

		#region "Kernel32.dll"

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/ms683199%28v=vs.85%29.aspx
		/// </summary>
		/// <param name="modName"></param>
		/// <returns></returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string modName);

		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);

		public static bool IsX86InWow64Mode()
		{
			bool retVal;
			IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
			return retVal;
		}

		/// <summary>
		/// Actually useful, combined with HRESULT_FROM_WIN32() and Marshal.GetExceptionForHR().
		/// works with undocumented functions SetMagnificationDesktopColorEffect() etc...
		/// where other last error functions do nothing.
		/// </summary>
		/// <returns></returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern int GetLastError();

		/// <summary>
		/// http://msdn.microsoft.com/en-us/library/windows/desktop/ms680746%28v=vs.85%29.aspx
		/// </summary>
		/// <param name="x">output of GetLastError()</param>
		/// <returns></returns>
		public static int HRESULT_FROM_WIN32(ulong x)
		{
			const int FACILITY_WIN32 = 7;
			return (int)(x) <= 0 ? (int)(x) : (int)(((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
		}

		/// <summary>
		/// Utility function to ease the pain of calling Marshal.GetExceptionForHR(HRESULT_FROM_WIN32((ulong)GetLastError()))...
		/// </summary>
		/// <returns></returns>
		public static Exception GetExceptionForLastError()
		{
			return Marshal.GetExceptionForHR(HRESULT_FROM_WIN32((ulong)GetLastError()));
		}

		#endregion

		#region "Magnification.dll"

		//http://msdn.microsoft.com/en-us/library/ms692402%28v=vs.85%29.aspx

		/// <summary>
		/// Window class of the magnifier control
		/// </summary>
		public const string WC_MAGNIFIER = "Magnifier";

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagInitialize();

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagUninitialize();

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagSetWindowSource(IntPtr hwnd, RECT rect);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagGetWindowSource(IntPtr hwnd, ref RECT pRect);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagSetWindowTransform(IntPtr hwnd, ref Transformation pTransform);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagGetWindowTransform(IntPtr hwnd, ref Transformation pTransform);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		//ref keyword necessary for X86, not sure why... (crash on call otherwise)
		public static extern bool MagSetColorEffect(IntPtr hwnd, ref ColorEffect pEffect);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagGetColorEffect(IntPtr hwnd, ref ColorEffect pEffect);

		#endregion

		#region "DWM API"

		//http://msdn.microsoft.com/en-us/library/aa969540%28v=vs.85%29.aspx

		///ATTENTION! : program must be compiled for x64 or the call will fail!

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref DWMWINDOWATTRIBUTE attrValue, int attrSize);

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref DWMNCRENDERINGPOLICY attrValue, int attrSize);

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref bool attrValue, int attrSize);

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref DWMFLIP3DWINDOWPOLICY attrValue, int attrSize);


		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern bool DwmIsCompositionEnabled();

		#endregion

	}

}