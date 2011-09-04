using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Karna.Magnification
{

	internal static class NativeMethods
	{

		public static IntPtr HWND_TOPMOST = new IntPtr(-1);
		public static IntPtr HWND_TOP = new IntPtr(0);

		public const int USER_TIMER_MINIMUM = 0x0000000A;
		public const int SM_ARRANGE = 0x38;
		public const int SM_CLEANBOOT = 0x43;
		public const int SM_CMONITORS = 80;
		public const int SM_CMOUSEBUTTONS = 0x2b;
		public const int SM_CXBORDER = 5;
		public const int SM_CXCURSOR = 13;
		public const int SM_CXDOUBLECLK = 0x24;
		public const int SM_CXDRAG = 0x44;
		public const int SM_CXEDGE = 0x2d;
		public const int SM_CXFIXEDFRAME = 7;
		public const int SM_CXFOCUSBORDER = 0x53;
		public const int SM_CXFRAME = 0x20;
		public const int SM_CXHSCROLL = 0x15;
		public const int SM_CXHTHUMB = 10;
		public const int SM_CXICON = 11;
		public const int SM_CXICONSPACING = 0x26;
		public const int SM_CXMAXIMIZED = 0x3d;
		public const int SM_CXMAXTRACK = 0x3b;
		public const int SM_CXMENUCHECK = 0x47;
		public const int SM_CXMENUSIZE = 0x36;
		public const int SM_CXMIN = 0x1c;
		public const int SM_CXMINIMIZED = 0x39;
		public const int SM_CXMINSPACING = 0x2f;
		public const int SM_CXMINTRACK = 0x22;
		public const int SM_CXSCREEN = 0;
		public const int SM_CXSIZE = 30;
		public const int SM_CXSIZEFRAME = 0x20;
		public const int SM_CXSMICON = 0x31;
		public const int SM_CXSMSIZE = 0x34;
		public const int SM_CXVIRTUALSCREEN = 0x4e;
		public const int SM_CXVSCROLL = 2;
		public const int SM_CYBORDER = 6;
		public const int SM_CYCAPTION = 4;
		public const int SM_CYCURSOR = 14;
		public const int SM_CYDOUBLECLK = 0x25;
		public const int SM_CYDRAG = 0x45;
		public const int SM_CYEDGE = 0x2e;
		public const int SM_CYFIXEDFRAME = 8;
		public const int SM_CYFOCUSBORDER = 0x54;
		public const int SM_CYFRAME = 0x21;
		public const int SM_CYHSCROLL = 3;
		public const int SM_CYICON = 12;
		public const int SM_CYICONSPACING = 0x27;
		public const int SM_CYKANJIWINDOW = 0x12;
		public const int SM_CYMAXIMIZED = 0x3e;
		public const int SM_CYMAXTRACK = 60;
		public const int SM_CYMENU = 15;
		public const int SM_CYMENUCHECK = 0x48;
		public const int SM_CYMENUSIZE = 0x37;
		public const int SM_CYMIN = 0x1d;
		public const int SM_CYMINIMIZED = 0x3a;
		public const int SM_CYMINSPACING = 0x30;
		public const int SM_CYMINTRACK = 0x23;
		public const int SM_CYSCREEN = 1;
		public const int SM_CYSIZE = 0x1f;
		public const int SM_CYSIZEFRAME = 0x21;
		public const int SM_CYSMCAPTION = 0x33;
		public const int SM_CYSMICON = 50;
		public const int SM_CYSMSIZE = 0x35;
		public const int SM_CYVIRTUALSCREEN = 0x4f;
		public const int SM_CYVSCROLL = 20;
		public const int SM_CYVTHUMB = 9;
		public const int SM_DBCSENABLED = 0x2a;
		public const int SM_DEBUG = 0x16;
		public const int SM_MENUDROPALIGNMENT = 40;
		public const int SM_MIDEASTENABLED = 0x4a;
		public const int SM_MOUSEPRESENT = 0x13;
		public const int SM_MOUSEWHEELPRESENT = 0x4b;
		public const int SM_NETWORK = 0x3f;
		public const int SM_PENWINDOWS = 0x29;
		public const int SM_REMOTESESSION = 0x1000;
		public const int SM_SAMEDISPLAYFORMAT = 0x51;
		public const int SM_SECURE = 0x2c;
		public const int SM_SHOWSOUNDS = 70;
		public const int SM_SWAPBUTTON = 0x17;
		public const int SM_XVIRTUALSCREEN = 0x4c;
		public const int SM_YVIRTUALSCREEN = 0x4d;

		public const string WC_MAGNIFIER = "Magnifier";

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr SetTimer(IntPtr hWnd, int nIDEvent, int uElapse, IntPtr lpTimerFunc);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool KillTimer(IntPtr hwnd, int idEvent);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetClientRect(IntPtr hWnd, [In, Out] ref RECT rect);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

		/*[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
*/
		//gets information about the windows
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		public const int GWL_EXSTYLE = -20;
		//sets bigflags that control the windows styles
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);



		//        HWND WINAPI CreateWindowEx(
		//  __in      DWORD dwExStyle,
		//  __in_opt  LPCTSTR lpClassName,
		//  __in_opt  LPCTSTR lpWindowName,
		//  __in      DWORD dwStyle,
		//  __in      int x,
		//  __in      int y,
		//  __in      int nWidth,
		//  __in      int nHeight,
		//  __in_opt  HWND hWndParent,
		//  __in_opt  HMENU hMenu,
		//  __in_opt  HINSTANCE hInstance,
		//  __in_opt  LPVOID lpParam
		//);

		//        HWND WINAPI CreateWindow(
		//  __in_opt  LPCTSTR lpClassName,
		//  __in_opt  LPCTSTR lpWindowName,
		//  __in      DWORD dwStyle,
		//  __in      int x,
		//  __in      int y,
		//  __in      int nWidth,
		//  __in      int nHeight,
		//  __in_opt  HWND hWndParent,
		//  __in_opt  HMENU hMenu,
		//  __in_opt  HINSTANCE hInstance,
		//  __in_opt  LPVOID lpParam
		//);
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




		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, LayeredWindowAttributeFlags dwFlags);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string modName);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(ref POINT pt);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InvalidateRect(IntPtr hWnd, IntPtr rect, [MarshalAs(UnmanagedType.Bool)] bool erase);

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
		public static extern bool MagSetWindowFilterList(IntPtr hwnd, int dwFilterMode, int count, IntPtr pHWND);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern int MagGetWindowFilterList(IntPtr hwnd, IntPtr pdwFilterMode, int count, IntPtr pHWND);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagSetColorEffect(IntPtr hwnd, ref ColorEffect pEffect);

		[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MagGetColorEffect(IntPtr hwnd, ref ColorEffect pEffect);

		/// <summary>
		/// Posted when the user presses a hot key registered by the RegisterHotKey function.
		/// The message is placed at the top of the message queue associated with the thread that registered the hot key. 
		/// </summary>
		public const int WM_HOTKEY = 0x0312;
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterHotKey(
			IntPtr hWnd, // handle to window    
			int id, // hot key identifier    
			KeyModifiers fsModifiers, // key-modifier options    
			Keys vk    // virtual-key code    
			);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnregisterHotKey(
			IntPtr hWnd, // handle to window    
			int id      // hot key identifier    
			);
		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref DWMWINDOWATTRIBUTE attrValue, int attrSize);

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref DWMNCRENDERINGPOLICY attrValue, int attrSize);

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref bool attrValue, int attrSize);

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref DWMFLIP3DWINDOWPOLICY attrValue, int attrSize);

	}

}