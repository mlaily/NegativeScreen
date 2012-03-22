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
using System.ComponentModel;

namespace NegativeScreen
{
	//based on http://delphi32.blogspot.com/2010/09/windows-magnification-api-net.html

	/// <summary>
	/// Magnifier Window Styles
	/// </summary>
	internal enum MagnifierStyle : int
	{
		MS_SHOWMAGNIFIEDCURSOR = 0x0001,
		MS_CLIPAROUNDCURSOR = 0x0002,
		MS_INVERTCOLORS = 0x0004
	}

	/// <summary>
	/// Magnification matrix
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct Transformation
	{
		public float m00;
		public float m01;
		public float m02;
		public float m10;
		public float m11;
		public float m12;
		public float m20;
		public float m21;
		public float m22;

		public Transformation(float magnificationFactor)
			: this()
		{
			m00 = magnificationFactor;
			m11 = magnificationFactor;
			m22 = 1.0f;
		}
	}

	/// <summary>
	/// Transformation matrix
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ColorEffect
	{
		public float transform00;
		public float transform01;
		public float transform02;
		public float transform03;
		public float transform04;
		public float transform10;
		public float transform11;
		public float transform12;
		public float transform13;
		public float transform14;
		public float transform20;
		public float transform21;
		public float transform22;
		public float transform23;
		public float transform24;
		public float transform30;
		public float transform31;
		public float transform32;
		public float transform33;
		public float transform34;
		public float transform40;
		public float transform41;
		public float transform42;
		public float transform43;
		public float transform44;

		public ColorEffect(float[,] matrix)
		{
			transform00 = matrix[0, 0];
			transform10 = matrix[1, 0];
			transform20 = matrix[2, 0];
			transform30 = matrix[3, 0];
			transform40 = matrix[4, 0];
			transform01 = matrix[0, 1];
			transform11 = matrix[1, 1];
			transform21 = matrix[2, 1];
			transform31 = matrix[3, 1];
			transform41 = matrix[4, 1];
			transform02 = matrix[0, 2];
			transform12 = matrix[1, 2];
			transform22 = matrix[2, 2];
			transform32 = matrix[3, 2];
			transform42 = matrix[4, 2];
			transform03 = matrix[0, 3];
			transform13 = matrix[1, 3];
			transform23 = matrix[2, 3];
			transform33 = matrix[3, 3];
			transform43 = matrix[4, 3];
			transform04 = matrix[0, 4];
			transform14 = matrix[1, 4];
			transform24 = matrix[2, 4];
			transform34 = matrix[3, 4];
			transform44 = matrix[4, 4];
		}

		/// <summary>
		/// 5*5 matrix
		/// </summary>
		/// <param name="matrix"></param>
		public void SetMatrix(float[,] matrix)
		{
			this.transform00 = matrix[0, 0];
			this.transform10 = matrix[1, 0];
			this.transform20 = matrix[2, 0];
			this.transform30 = matrix[3, 0];
			this.transform40 = matrix[4, 0];
			this.transform01 = matrix[0, 1];
			this.transform11 = matrix[1, 1];
			this.transform21 = matrix[2, 1];
			this.transform31 = matrix[3, 1];
			this.transform41 = matrix[4, 1];
			this.transform02 = matrix[0, 2];
			this.transform12 = matrix[1, 2];
			this.transform22 = matrix[2, 2];
			this.transform32 = matrix[3, 2];
			this.transform42 = matrix[4, 2];
			this.transform03 = matrix[0, 3];
			this.transform13 = matrix[1, 3];
			this.transform23 = matrix[2, 3];
			this.transform33 = matrix[3, 3];
			this.transform43 = matrix[4, 3];
			this.transform04 = matrix[0, 4];
			this.transform14 = matrix[1, 4];
			this.transform24 = matrix[2, 4];
			this.transform34 = matrix[3, 4];
			this.transform44 = matrix[4, 4];
		}

		/// <summary>
		/// 5*5 matrix
		/// </summary>
		/// <param name="matrix"></param>
		public float[,] GetMatrix()
		{
			float[,] matrix = new float[5, 5];
			matrix[0, 0] = this.transform00;
			matrix[1, 0] = this.transform10;
			matrix[2, 0] = this.transform20;
			matrix[3, 0] = this.transform30;
			matrix[4, 0] = this.transform40;
			matrix[0, 1] = this.transform01;
			matrix[1, 1] = this.transform11;
			matrix[2, 1] = this.transform21;
			matrix[3, 1] = this.transform31;
			matrix[4, 1] = this.transform41;
			matrix[0, 2] = this.transform02;
			matrix[1, 2] = this.transform12;
			matrix[2, 2] = this.transform22;
			matrix[3, 2] = this.transform32;
			matrix[4, 2] = this.transform42;
			matrix[0, 3] = this.transform03;
			matrix[1, 3] = this.transform13;
			matrix[2, 3] = this.transform23;
			matrix[3, 3] = this.transform33;
			matrix[4, 3] = this.transform43;
			matrix[0, 4] = this.transform04;
			matrix[1, 4] = this.transform14;
			matrix[2, 4] = this.transform24;
			matrix[3, 4] = this.transform34;
			matrix[4, 4] = this.transform44;
			return matrix;
		}
	}

	/// <summary>
	/// A Wrapper for a RECT struct
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RECT"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		/// <summary>
		/// Position of left edge
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public int left;

		/// <summary>
		/// Position of top edge
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public int top;

		/// <summary>
		/// Position of right edge
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public int right;

		/// <summary>
		/// Position of bottom edge
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public int bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		public RECT(int width, int height)
		{
			this.left = 0;
			this.top = 0;
			this.right = width;
			this.bottom = height;
		}

		public override bool Equals(object obj)
		{
			RECT r = (RECT)obj;
			return (r.left == left && r.right == right && r.top == top && r.bottom == bottom);
		}

		public override int GetHashCode()
		{
			// Attempting a minor degree of "hash-ness" here
			return ((left ^ top) ^ right) ^ bottom;
		}

		public static bool operator ==(RECT a, RECT b)
		{
			return (a.left == b.left && a.right == b.right && a.top == b.top && a.bottom == b.bottom);
		}

		public static bool operator !=(RECT a, RECT b)
		{
			return !(a == b);
		}

	}


	/// <summary>
	/// Specifies the style of the window being created
	/// </summary>
	[FlagsAttribute]
	[Description("Specifies the style of the window being created")]
	internal enum WindowStyles : int
	{
		/// <summary>
		/// Creates an overlapped window. An overlapped window has a title bar and a border 
		/// </summary>
		WS_OVERLAPPED = 0x00000000,
		/// <summary>
		/// Creates a pop-up window
		/// </summary>
		WS_POPUP = -2147483648,
		/// <summary>
		/// Creates a child window. A window with this style cannot have a menu bar. 
		/// This style cannot be used with the WS_POPUP style.
		/// </summary>
		WS_CHILD = 0x40000000,
		/// <summary>
		/// Creates a window that is initially minimized. 
		/// Same as the WS_ICONIC style.
		/// </summary>
		WS_MINIMIZE = 0x20000000,
		/// <summary>
		/// Creates a window that is initially visible.
		/// </summary>
		WS_VISIBLE = 0x10000000,
		/// <summary>
		/// Creates a window that is initially disabled. 
		/// A disabled window cannot receive input from the user
		/// </summary>
		WS_DISABLED = 0x08000000,
		/// <summary>
		/// Clips child windows relative to each other; that is, when a particular child window 
		/// receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping 
		/// child windows out of the region of the child window to be updated. 
		/// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, 
		/// when drawing within the client area of a child window, to draw within the client area 
		/// of a neighboring child window.
		/// </summary>
		WS_CLIPSIBLINGS = 0x04000000,
		/// <summary>
		/// Excludes the area occupied by child windows when drawing occurs within the parent window. 
		/// This style is used when creating the parent window.
		/// </summary>
		WS_CLIPCHILDREN = 0x02000000,
		/// <summary>
		/// Creates a window that is initially maximized.
		/// </summary>
		WS_MAXIMIZE = 0x01000000,
		/// <summary>
		/// Creates a window that has a title bar (includes the WS_BORDER style).
		/// </summary>
		WS_CAPTION = 0x00C00000,
		/// <summary>
		/// Creates a window that has a thin-line border.
		/// </summary>
		WS_BORDER = 0x00800000,
		/// <summary>
		/// Creates a window that has a border of a style typically used with dialog boxes. 
		/// A window with this style cannot have a title bar.
		/// </summary>
		WS_DLGFRAME = 0x00400000,
		/// <summary>
		/// Creates a window that has a vertical scroll bar.
		/// </summary>
		WS_VSCROLL = 0x00200000,
		/// <summary>
		/// Creates a window that has a horizontal scroll bar.
		/// </summary>
		WS_HSCROLL = 0x00100000,
		/// <summary>
		/// Creates a window that has a window menu on its title bar. 
		/// The WS_CAPTION style must also be specified.
		/// </summary>
		WS_SYSMENU = 0x00080000,
		/// <summary>
		/// Creates a window that has a sizing border. 
		/// Same as the WS_SIZEBOX style.
		/// </summary>
		WS_THICKFRAME = 0x00040000,
		/// <summary>
		/// Specifies the first control of a group of controls. 
		/// The group consists of this first control and all controls defined after it, 
		/// up to the next control with the WS_GROUP style. The first control in each group 
		/// usually has the WS_TABSTOP style so that the user can move from group to group. 
		/// The user can subsequently change the keyboard focus from one control in the group 
		/// to the next control in the group by using the direction keys.
		/// </summary>
		WS_GROUP = 0x00020000,
		/// <summary>
		/// Specifies a control that can receive the keyboard focus when the user presses the TAB key. 
		/// Pressing the TAB key changes the keyboard focus to the next control with the 
		/// WS_TABSTOP style. 
		/// </summary>
		WS_TABSTOP = 0x00010000,
		/// <summary>
		/// Creates a window that has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP 
		/// style. The WS_SYSMENU style must also be specified. 
		/// </summary>
		WS_MINIMIZEBOX = 0x00020000,
		/// <summary>
		/// Creates a window that has a maximize button. Cannot be combined with the 
		/// WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified. 
		/// </summary>
		WS_MAXIMIZEBOX = 0x00010000
	}

	/// <summary>
	///Common window styles
	/// </summary>
	[Description("Common window styles")]
	internal enum CommonWindowStyles : int
	{
		/// <summary>
		///Creates an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style. 
		/// </summary>
		WS_TILED = WindowStyles.WS_OVERLAPPED,
		/// <summary>
		///Creates a window that is initially minimized. Same as the WS_MINIMIZE style.
		/// </summary>
		WS_ICONIC = WindowStyles.WS_MINIMIZE,
		/// <summary>
		///Creates a window that has a sizing border. Same as the WS_THICKFRAME style.
		/// </summary>
		WS_SIZEBOX = WindowStyles.WS_THICKFRAME,
		/// <summary>
		/// Creates an overlapped window with the WS_OVERLAPPED, WS_CAPTION, WS_SYSMENU, WS_THICKFRAME, WS_MINIMIZEBOX, and WS_MAXIMIZEBOX styles. Same as the WS_TILEDWINDOW style. 
		/// </summary>
		WS_OVERLAPPEDWINDOW = (WindowStyles.WS_OVERLAPPED | WindowStyles.WS_CAPTION | WindowStyles.WS_SYSMENU | WindowStyles.WS_THICKFRAME | WindowStyles.WS_MINIMIZEBOX | WindowStyles.WS_MAXIMIZEBOX),
		/// <summary>
		///Creates an overlapped window with the WS_OVERLAPPED, WS_CAPTION, WS_SYSMENU, WS_THICKFRAME, WS_MINIMIZEBOX, and WS_MAXIMIZEBOX styles. Same as the WS_OVERLAPPEDWINDOW style.  
		/// </summary>
		WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
		/// <summary>
		///Creates a pop-up window with WS_BORDER, WS_POPUP, and WS_SYSMENU styles. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.
		/// </summary>
		WS_POPUPWINDOW = (WindowStyles.WS_POPUP |
						  WindowStyles.WS_BORDER |
						  WindowStyles.WS_SYSMENU),
		/// <summary>
		///Same as the WS_CHILD style.
		/// </summary>
		WS_CHILDWINDOW = (WindowStyles.WS_CHILD)
	}

	[FlagsAttribute]
	internal enum SetWindowPosFlags : int
	{
		SWP_NOSIZE = 1,
		SWP_NOMOVE = 2,
		SWP_NOZORDER = 4,
		SWP_NOREDRAW = 8,
		SWP_NOACTIVATE = 0x10,
		SWP_FRAMECHANGED = 0x20,
		SWP_SHOWWINDOW = 0x40,
		SWP_HIDEWINDOW = 0x80,
		SWP_NOCOPYBITS = 0x100,
		SWP_NOOWNERZORDER = 0x200,
		SWP_NOSENDCHANGING = 0x400
	}

	/// <summary>
	/// Specifies the extended style of the window
	/// </summary>
	[FlagsAttribute]
	[Description("Specifies the extended style of the window")]
	internal enum ExtendedWindowStyles : int
	{
		/// <summary>
		/// Creates a window that has a double border; the window can, optionally, 
		/// be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
		/// </summary>
		WS_EX_DLGMODALFRAME = 0x00000001,
		/// <summary>
		/// Specifies that a child window created with this style does not send 
		/// the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
		/// </summary>
		WS_EX_NOPARENTNOTIFY = 0x00000004,
		/// <summary>
		/// Specifies that a window created with this style should be placed above all nontopmost 
		/// windows and stay above them even when the window is deactivated
		/// </summary>
		WS_EX_TOPMOST = 0x00000008,
		/// <summary>
		/// Windows that can accept dragged objects must be created with this style so that 
		/// Windows can determine that the window will accept objects and can change the drag/drop 
		/// cursor as the user drags an object over the window. 
		/// </summary>
		WS_EX_ACCEPTFILES = 0x00000010,
		/// <summary>
		/// The WS_EX_TRANSPARENT style makes a window transparent; that is, the window can be seen through, 
		/// and anything under the window is still visible. Transparent windows are not transparent 
		/// to mouse or keyboard events. A transparent window receives paint messages when anything 
		/// under it changes. Transparent windows are useful for drawing drag handles on top of other 
		/// windows or for implementing "hot-spot" areas without having to hit test because the transparent 
		/// window receives click messages. 
		/// </summary>
		WS_EX_TRANSPARENT = 0x00000020,
		/// <summary>
		/// Creates an MDI child window. 
		/// </summary>
		WS_EX_MDICHILD = 0x00000040,
		/// <summary>
		/// Creates a tool window, which is a window intended to be used as a floating toolbar. 
		/// A tool window has a title bar that is shorter than a normal title bar, and the window title 
		/// is drawn using a smaller font. A tool window does not appear in the task bar or in the window 
		/// that appears when the user presses ALT+TAB. 
		/// </summary>
		WS_EX_TOOLWINDOW = 0x00000080,
		/// <summary>
		/// Specifies that a window has a border with a raised edge. 
		/// </summary>
		WS_EX_WINDOWEDGE = 0x00000100,
		/// <summary>
		///  Specifies that a window has a 3D look — that is, a border with a sunken edge. 
		/// </summary>
		WS_EX_CLIENTEDGE = 0x00000200,
		/// <summary>
		/// Includes a question mark in the title bar of the window. 
		/// When the user clicks the question mark, the cursor changes to a question mark with a pointer. 
		/// If the user then clicks a child window, the child receives a WM_HELP message. 
		/// </summary>
		WS_EX_CONTEXTHELP = 0x00000400,
		/// <summary>
		/// Gives a window generic right-aligned properties. This depends on the window class. 
		/// </summary>
		WS_EX_RIGHT = 0x00001000,
		/// <summary>
		/// Gives window generic left-aligned properties. This is the default. 
		/// </summary>
		WS_EX_LEFT = 0x00000000,
		/// <summary>
		/// Displays the window text using right-to-left reading order properties. 
		/// </summary>
		WS_EX_RTLREADING = 0x00002000,
		/// <summary>
		/// Displays the window text using left-to-right reading order properties. This is the default. 
		/// </summary>
		WS_EX_LTRREADING = 0x00000000,
		/// <summary>
		/// Places a vertical scroll bar to the left of the client area. 
		/// </summary>
		WS_EX_LEFTSCROLLBAR = 0x00004000,
		/// <summary>
		/// Places a vertical scroll bar (if present) to the right of the client area. This is the default. 
		/// </summary>
		WS_EX_RIGHTSCROLLBAR = 0x00000000,
		/// <summary>
		/// Allows the user to navigate among the child windows of the window by using the TAB key. 
		/// </summary>
		WS_EX_CONTROLPARENT = 0x00010000,
		/// <summary>
		/// Creates a window with a three-dimensional border style intended to be used for items that 
		/// do not accept user input. 
		/// </summary>
		WS_EX_STATICEDGE = 0x00020000,
		/// <summary>
		/// Forces a top-level window onto the taskbar when the window is visible. 
		/// </summary>
		WS_EX_APPWINDOW = 0x00040000,
		/// <summary>
		/// Creates a layered window. Note that this cannot be used for child windows
		/// </summary>
		WS_EX_LAYERED = 0x00080000,
		/// <summary>
		/// A window created with this style does not pass its window layout to its child windows.
		/// </summary>
		WS_EX_NOINHERITLAYOUT = 0x00100000,
		/// <summary>
		/// Creates a window whose horizontal origin is on the right edge. 
		/// Increasing horizontal values advance to the left. 
		/// </summary>
		WS_EX_LAYOUTRTL = 0x00400000,
		/// <summary>
		/// Paints all descendants of a window in bottom-to-top painting order using double-buffering. 
		/// </summary>
		WS_EX_COMPOSITED = 0x02000000,
		/// <summary>
		/// A top-level window created with this style does not become the foreground window when the user 
		/// clicks it. The system does not bring this window to the foreground when the user minimizes 
		/// or closes the foreground window. 
		/// </summary>
		WS_EX_NOACTIVATE = 0x08000000
	}

	/// <summary>
	/// Common extended window styles
	/// </summary>
	[Description("Common extended window styles")]
	internal enum CommonExtendedWindowStyles : int
	{
		/// <summary>
		/// Combines the WS_EX_CLIENTEDGE and WS_EX_WINDOWEDGE styles.
		/// </summary>
		WS_EX_OVERLAPPEDWINDOW = (ExtendedWindowStyles.WS_EX_WINDOWEDGE |
								  ExtendedWindowStyles.WS_EX_CLIENTEDGE),
		/// <summary>
		/// Combines the WS_EX_WINDOWEDGE, WS_EX_TOOLWINDOW, and WS_EX_TOPMOST styles.
		/// </summary>
		WS_EX_PALETTEWINDOW = (ExtendedWindowStyles.WS_EX_WINDOWEDGE |
								  ExtendedWindowStyles.WS_EX_TOOLWINDOW |
								  ExtendedWindowStyles.WS_EX_TOPMOST)
	}

	/// <summary>
	/// Layered window flags
	/// </summary>
	[FlagsAttribute]
	[Description("Layered window flags")]
	internal enum LayeredWindowAttributeFlags : int
	{
		/// <summary>
		/// Use key as a transparency color
		/// </summary>
		LWA_COLORKEY = 0x00000001,
		/// <summary>
		/// Use Alpha to determine the opacity of the layered window.
		/// </summary>
		LWA_ALPHA = 0x00000002
	}

	[FlagsAttribute]
	internal enum LayeredWindowUpdateFlags : int
	{
		ULW_COLORKEY = 0x00000001,
		ULW_ALPHA = 0x00000002,
		ULW_OPAQUE = 0x00000004
	}

	internal enum ShowWindowStyles : short
	{
		SW_HIDE = 0,
		SW_SHOWNORMAL = 1,
		SW_NORMAL = 1,
		SW_SHOWMINIMIZED = 2,
		SW_SHOWMAXIMIZED = 3,
		SW_MAXIMIZE = 3,
		SW_SHOWNOACTIVATE = 4,
		SW_SHOW = 5,
		SW_MINIMIZE = 6,
		SW_SHOWMINNOACTIVE = 7,
		SW_SHOWNA = 8,
		SW_RESTORE = 9,
		SW_SHOWDEFAULT = 10,
		SW_FORCEMINIMIZE = 11,
		SW_MAX = 11
	}

	internal enum WindowMessage : int
	{
		WM_NULL = 0x00,
		WM_CREATE = 0x01,
		WM_DESTROY = 0x02,
		WM_MOVE = 0x03,
		WM_SIZE = 0x05,
		WM_ACTIVATE = 0x06,
		WM_SETFOCUS = 0x07,
		WM_KILLFOCUS = 0x08,
		WM_ENABLE = 0x0A,
		WM_SETREDRAW = 0x0B,
		WM_SETTEXT = 0x0C,
		WM_GETTEXT = 0x0D,
		WM_GETTEXTLENGTH = 0x0E,
		WM_PAINT = 0x0F,
		WM_CLOSE = 0x10,
		WM_QUERYENDSESSION = 0x11,
		WM_QUIT = 0x12,
		WM_QUERYOPEN = 0x13,
		WM_ERASEBKGND = 0x14,
		WM_SYSCOLORCHANGE = 0x15,
		WM_ENDSESSION = 0x16,
		WM_SYSTEMERROR = 0x17,
		WM_SHOWWINDOW = 0x18,
		WM_CTLCOLOR = 0x19,
		WM_WININICHANGE = 0x1A,
		WM_SETTINGCHANGE = 0x1A,
		WM_DEVMODECHANGE = 0x1B,
		WM_ACTIVATEAPP = 0x1C,
		WM_FONTCHANGE = 0x1D,
		WM_TIMECHANGE = 0x1E,
		WM_CANCELMODE = 0x1F,
		WM_SETCURSOR = 0x20,
		WM_MOUSEACTIVATE = 0x21,
		WM_CHILDACTIVATE = 0x22,
		WM_QUEUESYNC = 0x23,
		WM_GETMINMAXINFO = 0x24,
		WM_PAINTICON = 0x26,
		WM_ICONERASEBKGND = 0x27,
		WM_NEXTDLGCTL = 0x28,
		WM_SPOOLERSTATUS = 0x2A,
		WM_DRAWITEM = 0x2B,
		WM_MEASUREITEM = 0x2C,
		WM_DELETEITEM = 0x2D,
		WM_VKEYTOITEM = 0x2E,
		WM_CHARTOITEM = 0x2F,

		WM_SETFONT = 0x30,
		WM_GETFONT = 0x31,
		WM_SETHOTKEY = 0x32,
		WM_GETHOTKEY = 0x33,
		WM_QUERYDRAGICON = 0x37,
		WM_COMPAREITEM = 0x39,
		WM_COMPACTING = 0x41,
		WM_WINDOWPOSCHANGING = 0x46,
		WM_WINDOWPOSCHANGED = 0x47,
		WM_POWER = 0x48,
		WM_COPYDATA = 0x4A,
		WM_CANCELJOURNAL = 0x4B,
		WM_NOTIFY = 0x4E,
		WM_INPUTLANGCHANGEREQUEST = 0x50,
		WM_INPUTLANGCHANGE = 0x51,
		WM_TCARD = 0x52,
		WM_HELP = 0x53,
		WM_USERCHANGED = 0x54,
		WM_NOTIFYFORMAT = 0x55,
		WM_CONTEXTMENU = 0x7B,
		WM_STYLECHANGING = 0x7C,
		WM_STYLECHANGED = 0x7D,
		WM_DISPLAYCHANGE = 0x7E,
		WM_GETICON = 0x7F,
		WM_SETICON = 0x80,

		WM_NCCREATE = 0x81,
		WM_NCDESTROY = 0x82,
		WM_NCCALCSIZE = 0x83,
		WM_NCHITTEST = 0x84,
		WM_NCPAINT = 0x85,
		WM_NCACTIVATE = 0x86,
		WM_GETDLGCODE = 0x87,
		WM_NCMOUSEMOVE = 0xA0,
		WM_NCLBUTTONDOWN = 0xA1,
		WM_NCLBUTTONUP = 0xA2,
		WM_NCLBUTTONDBLCLK = 0xA3,
		WM_NCRBUTTONDOWN = 0xA4,
		WM_NCRBUTTONUP = 0xA5,
		WM_NCRBUTTONDBLCLK = 0xA6,
		WM_NCMBUTTONDOWN = 0xA7,
		WM_NCMBUTTONUP = 0xA8,
		WM_NCMBUTTONDBLCLK = 0xA9,

		WM_KEYFIRST = 0x100,
		WM_KEYDOWN = 0x100,
		WM_KEYUP = 0x101,
		WM_CHAR = 0x102,
		WM_DEADCHAR = 0x103,
		WM_SYSKEYDOWN = 0x104,
		WM_SYSKEYUP = 0x105,
		WM_SYSCHAR = 0x106,
		WM_SYSDEADCHAR = 0x107,
		WM_KEYLAST = 0x108,

		WM_IME_STARTCOMPOSITION = 0x10D,
		WM_IME_ENDCOMPOSITION = 0x10E,
		WM_IME_COMPOSITION = 0x10F,
		WM_IME_KEYLAST = 0x10F,

		WM_INITDIALOG = 0x110,
		WM_COMMAND = 0x111,
		WM_SYSCOMMAND = 0x112,
		WM_TIMER = 0x113,
		WM_HSCROLL = 0x114,
		WM_VSCROLL = 0x115,
		WM_INITMENU = 0x116,
		WM_INITMENUPOPUP = 0x117,
		WM_MENUSELECT = 0x11F,
		WM_MENUCHAR = 0x120,
		WM_ENTERIDLE = 0x121,

		WM_CTLCOLORMSGBOX = 0x132,
		WM_CTLCOLOREDIT = 0x133,
		WM_CTLCOLORLISTBOX = 0x134,
		WM_CTLCOLORBTN = 0x135,
		WM_CTLCOLORDLG = 0x136,
		WM_CTLCOLORSCROLLBAR = 0x137,
		WM_CTLCOLORSTATIC = 0x138,

		WM_MOUSEFIRST = 0x200,
		WM_MOUSEMOVE = 0x200,
		WM_LBUTTONDOWN = 0x201,
		WM_LBUTTONUP = 0x202,
		WM_LBUTTONDBLCLK = 0x203,
		WM_RBUTTONDOWN = 0x204,
		WM_RBUTTONUP = 0x205,
		WM_RBUTTONDBLCLK = 0x206,
		WM_MBUTTONDOWN = 0x207,
		WM_MBUTTONUP = 0x208,
		WM_MBUTTONDBLCLK = 0x209,
		WM_MOUSEWHEEL = 0x20A,
		WM_MOUSEHWHEEL = 0x20E,

		WM_PARENTNOTIFY = 0x210,
		WM_ENTERMENULOOP = 0x211,
		WM_EXITMENULOOP = 0x212,
		WM_NEXTMENU = 0x213,
		WM_SIZING = 0x214,
		WM_CAPTURECHANGED = 0x215,
		WM_MOVING = 0x216,
		WM_POWERBROADCAST = 0x218,
		WM_DEVICECHANGE = 0x219,

		WM_MDICREATE = 0x220,
		WM_MDIDESTROY = 0x221,
		WM_MDIACTIVATE = 0x222,
		WM_MDIRESTORE = 0x223,
		WM_MDINEXT = 0x224,
		WM_MDIMAXIMIZE = 0x225,
		WM_MDITILE = 0x226,
		WM_MDICASCADE = 0x227,
		WM_MDIICONARRANGE = 0x228,
		WM_MDIGETACTIVE = 0x229,
		WM_MDISETMENU = 0x230,
		WM_ENTERSIZEMOVE = 0x231,
		WM_EXITSIZEMOVE = 0x232,
		WM_DROPFILES = 0x233,
		WM_MDIREFRESHMENU = 0x234,

		WM_IME_SETCONTEXT = 0x281,
		WM_IME_NOTIFY = 0x282,
		WM_IME_CONTROL = 0x283,
		WM_IME_COMPOSITIONFULL = 0x284,
		WM_IME_SELECT = 0x285,
		WM_IME_CHAR = 0x286,
		WM_IME_KEYDOWN = 0x290,
		WM_IME_KEYUP = 0x291,

		WM_MOUSEHOVER = 0x2A1,
		WM_NCMOUSELEAVE = 0x2A2,
		WM_MOUSELEAVE = 0x2A3,

		WM_CUT = 0x300,
		WM_COPY = 0x301,
		WM_PASTE = 0x302,
		WM_CLEAR = 0x303,
		WM_UNDO = 0x304,

		WM_RENDERFORMAT = 0x305,
		WM_RENDERALLFORMATS = 0x306,
		WM_DESTROYCLIPBOARD = 0x307,
		WM_DRAWCLIPBOARD = 0x308,
		WM_PAINTCLIPBOARD = 0x309,
		WM_VSCROLLCLIPBOARD = 0x30A,
		WM_SIZECLIPBOARD = 0x30B,
		WM_ASKCBFORMATNAME = 0x30C,
		WM_CHANGECBCHAIN = 0x30D,
		WM_HSCROLLCLIPBOARD = 0x30E,
		WM_QUERYNEWPALETTE = 0x30F,
		WM_PALETTEISCHANGING = 0x310,
		WM_PALETTECHANGED = 0x311,

		/// <summary>
		/// Posted when the user presses a hot key registered by the RegisterHotKey function.
		/// The message is placed at the top of the message queue associated with the thread that registered the hot key. 
		/// </summary>
		WM_HOTKEY = 0x312,
		WM_PRINT = 0x317,
		WM_PRINTCLIENT = 0x318,

		WM_HANDHELDFIRST = 0x358,
		WM_HANDHELDLAST = 0x35F,
		WM_PENWINFIRST = 0x380,
		WM_PENWINLAST = 0x38F,
		WM_COALESCE_FIRST = 0x390,
		WM_COALESCE_LAST = 0x39F,
		WM_DDE_FIRST = 0x3E0,
		WM_DDE_INITIATE = 0x3E0,
		WM_DDE_TERMINATE = 0x3E1,
		WM_DDE_ADVISE = 0x3E2,
		WM_DDE_UNADVISE = 0x3E3,
		WM_DDE_ACK = 0x3E4,
		WM_DDE_DATA = 0x3E5,
		WM_DDE_REQUEST = 0x3E6,
		WM_DDE_POKE = 0x3E7,
		WM_DDE_EXECUTE = 0x3E8,
		WM_DDE_LAST = 0x3E8,

		WM_USER = 0x400,
		WM_APP = 0x8000,

		WM_DWMCOMPOSITIONCHANGED = 0x031E,
		WM_DWMNCRENDERINGCHANGED = 0x031F,
		WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
		WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

	}

	/// <summary>
	/// The keys that must be pressed in combination with the key specified by the uVirtKey parameter in order to generate the WM_HOTKEY message.
	/// The fsModifiers parameter can be a combination of the following values.
	/// </summary>
	internal enum KeyModifiers : int
	{
		NONE = 0,
		/// <summary>
		/// Either ALT key must be held down.
		/// </summary>
		MOD_ALT = 0x0001,
		/// <summary>
		/// Either CTRL key must be held down.
		/// </summary>
		MOD_CONTROL = 0x0002,
		/// <summary>
		/// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.
		///    Windows Vista and Windows XP/2000:  This flag is not supported.
		/// </summary>
		MOD_NOREPEAT = 0x4000,
		/// <summary>
		/// Either SHIFT key must be held down.
		/// </summary>
		MOD_SHIFT = 0x0004,
		/// <summary>
		/// Either WINDOWS key was held down. These keys are labeled with the Windows logo.
		/// Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
		/// </summary>
		MOD_WIN = 0x0008,
	}

	[Flags]
	internal enum DWMWINDOWATTRIBUTE : int
	{
		/// <summary>
		/// Use with DwmGetWindowAttribute. Discovers whether non-client rendering is enabled.
		/// The retrieved value is of type BOOL. TRUE if non-client rendering is enabled; otherwise, FALSE.
		/// </summary>
		DWMWA_NCRENDERING_ENABLED = 1,
		/// <summary>
		/// Use with DwmSetWindowAttribute. Sets the non-client rendering policy.
		/// The pvAttribute parameter points to a value from the DWMNCRENDERINGPOLICY enumeration.
		/// </summary>
		DWMWA_NCRENDERING_POLICY,
		/// <summary>
		/// Use with DwmSetWindowAttribute. Enables or forcibly disables DWM transitions.
		/// The pvAttribute parameter points to a value of TRUE to disable transitions or FALSE to enable transitions.
		/// </summary>
		DWMWA_TRANSITIONS_FORCEDISABLED,
		/// <summary>
		/// Use with DwmSetWindowAttribute. Enables content rendered in the non-client area to be visible on the frame drawn by DWM.
		/// The pvAttribute parameter points to a value of TRUE to enable content rendered in the non-client area to be visible on the frame; otherwise, it points to FALSE.
		/// </summary>
		DWMWA_ALLOW_NCPAINT,
		/// <summary>
		/// Use with DwmGetWindowAttribute. Retrieves the bounds of the caption button area in the window-relative space.
		/// The retrieved value is of type RECT.
		/// </summary>
		DWMWA_CAPTION_BUTTON_BOUNDS,
		/// <summary>
		/// Use with DwmSetWindowAttribute. Specifies whether non-client content is right-to-left (RTL) mirrored.
		/// The pvAttribute parameter points to a value of TRUE if the non-client content is right-to-left (RTL) mirrored; otherwise, it points to FALSE.
		/// </summary>
		DWMWA_NONCLIENT_RTL_LAYOUT,
		/// <summary>
		/// Use with DwmSetWindowAttribute. Forces the window to display an iconic thumbnail or peek representation (a static bitmap), even if a live or snapshot representation of the window is available.
		/// This value normally is set during a window's creation and not changed throughout the window's lifetime. Some scenarios, however, might require the value to change over time.
		/// The pvAttribute parameter points to a value of TRUE to require a iconic thumbnail or peek representation; otherwise, it points to FALSE.
		/// </summary>
		DWMWA_FORCE_ICONIC_REPRESENTATION,
		/// <summary>
		/// Use with DwmSetWindowAttribute. Sets how Flip3D treats the window.
		/// The pvAttribute parameter points to a value from the DWMFLIP3DWINDOWPOLICY enumeration.
		/// </summary>
		DWMWA_FLIP3D_POLICY,
		/// <summary>
		/// Use with DwmGetWindowAttribute. Retrieves the extended frame bounds rectangle in screen space.
		/// The retrieved value is of type RECT.
		/// </summary>
		DWMWA_EXTENDED_FRAME_BOUNDS,
		/// <summary>
		/// Use with DwmSetWindowAttribute. The window will provide a bitmap for use by DWM as an iconic thumbnail or peek representation (a static bitmap) for the window.
		/// DWMWA_HAS_ICONIC_BITMAP can be specified with DWMWA_FORCE_ICONIC_REPRESENTATION.
		/// DWMWA_HAS_ICONIC_BITMAP normally is set during a window's creation and not changed throughout the window's lifetime.
		/// Some scenarios, however, might require the value to change over time.
		/// The pvAttribute parameter points to a value of TRUE to inform DWM that the window will provide an iconic thumbnail or peek representation; otherwise, it points to FALSE.
		/// Windows Vista and earlier:  This value is not supported.
		/// </summary>
		DWMWA_HAS_ICONIC_BITMAP,
		/// <summary>
		///  Use with DwmSetWindowAttribute. Do not show peek preview for the window.
		///  The peek view shows a full-sized preview of the window when the mouse hovers over the window's thumbnail in the taskbar.
		///  If this attribute is set, hovering the mouse pointer over the window's thumbnail dismisses peek (in case another window in the group has a peek preview showing).
		///  The pvAttribute parameter points to a value of TRUE to prevent peek functionality or FALSE to allow it.
		/// Windows Vista and earlier:  This value is not supported.
		/// </summary>
		DWMWA_DISALLOW_PEEK,
		/// <summary>
		/// Use with DwmSetWindowAttribute. Prevents a window from fading to a glass sheet when peek is invoked.
		/// The pvAttribute parameter points to a value of TRUE to prevent the window from fading during another window's peek or FALSE for normal behavior.
		/// Windows Vista and earlier:  This value is not supported.
		/// </summary>
		DWMWA_EXCLUDED_FROM_PEEK,
		/// <summary>
		///  The maximum recognized DWMWINDOWATTRIBUTE value, used for validation purposes.
		/// </summary>
		DWMWA_LAST,
	}

	internal enum DWMNCRENDERINGPOLICY
	{
		/// <summary>
		/// The non-client rendering area is rendered based on the window style.
		/// </summary>
		DWMNCRP_USEWINDOWSTYLE,
		/// <summary>
		/// The non-client area rendering is disabled; the window style is ignored.
		/// </summary>
		DWMNCRP_DISABLED,
		/// <summary>
		/// The non-client area rendering is enabled; the window style is ignored.
		/// </summary>
		DWMNCRP_ENABLED,
		/// <summary>
		/// The maximum recognized DWMNCRENDERINGPOLICY value, used for validation purposes.
		/// </summary>
		DWMNCRP_LAST,
	}

	internal enum DWMFLIP3DWINDOWPOLICY
	{
		/// <summary>
		///  Use the window's style and visibility settings to determine whether to hide or include the window in Flip3D rendering.
		/// </summary>
		DWMFLIP3D_DEFAULT,
		/// <summary>
		/// Exclude the window from Flip3D and display it below the Flip3D rendering.
		/// </summary>
		DWMFLIP3D_EXCLUDEBELOW,
		/// <summary>
		/// Exclude the window from Flip3D and display it above the Flip3D rendering.
		/// </summary>
		DWMFLIP3D_EXCLUDEABOVE,
		/// <summary>
		/// The maximum recognized DWMFLIP3DWINDOWPOLICY value, used for validation purposes.
		/// </summary>
		DWMFLIP3D_LAST,
	}

}