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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;


namespace NegativeScreen
{
	class NegativeOverlay : Form
	{
		private IntPtr hwndMag;
		/// <summary>
		/// used when refreshing the control
		/// </summary>
		public IntPtr HwndMag { get { return hwndMag; } }

		public NegativeOverlay(Screen screen)
			: base()
		{

			this.StartPosition = FormStartPosition.Manual;
			this.Location = screen.WorkingArea.Location;
			this.Size = new Size(screen.Bounds.Width, screen.Bounds.Height);
			this.TopMost = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Normal;
			this.ShowInTaskbar = false;

			IntPtr hInst = NativeMethods.GetModuleHandle(null);
			if (hInst == IntPtr.Zero)
			{
				throw new Exception("GetModuleHandle()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			//set WS_EX_LAYERED a layered window (required)
			//and WS_EX_TRANSPARENT (mouse and keyboard events pass through the window)
			if (NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE, (int)ExtendedWindowStyles.WS_EX_LAYERED | (int)ExtendedWindowStyles.WS_EX_TRANSPARENT) == 0)
			{
				throw new Exception("SetWindowLong()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			// Make the window opaque.
			if (!NativeMethods.SetLayeredWindowAttributes(this.Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA))
			{
				throw new Exception("SetLayeredWindowAttributes()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			// Create a magnifier control that fills the client area.
			hwndMag = NativeMethods.CreateWindowEx(0,
				NativeMethods.WC_MAGNIFIER,
				"MagnifierWindow",
				(int)WindowStyles.WS_CHILD |
				/*(int)MagnifierStyle.MS_SHOWMAGNIFIEDCURSOR |*/
				(int)WindowStyles.WS_VISIBLE,
				0, 0, screen.Bounds.Width, screen.Bounds.Height,
				this.Handle, IntPtr.Zero, hInst, IntPtr.Zero);

			if (hwndMag == IntPtr.Zero)
			{
				throw new Exception("CreateWindowEx()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			ColorEffect a = new ColorEffect();
			ColorEffect b = new ColorEffect();
			float x = 0.8f;
			float y = 0.8f;
			float z = -0.1f;
			a.SetMatrix(new float[,]{

			{  x-z,-x,-x, 0.0f, 0.0f },
			{ -x,  x-z, -x, 0.0f, 0.0f },
			{ -x,-x, x-z, 0.0f, 0.0f },
			{  0.0f,  0.0f,  0.0f,  0.0f, 0.0f },
			{  y,  y,  y,  0.0f, 1.0f }

			//saturation 0.8
			//{  0.75f,-0.89f,-0.89f, 0.0f, 0.0f },
			//{ -0.78f,  0.86f, -0.78f, 0.0f, 0.0f },
			//{ -0.97f,-0.97f, 0.67f, 0.0f, 0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f, 0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f, 1.0f }

		//a bit more readable (desaturated, colors not exact)
			//{  0.50f,-0.78f,-0.78f, 0.0f, 0.0f },
			//{ -0.56f,  0.72f, -0.56f, 0.0f, 0.0f },
			//{ -0.94f,-0.94f, 0.34f, 0.0f, 0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f, 0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f, 1.0f }

			//OWI
			//{  1f,-1f,-1f, 0.0f, 0.0f },
			//{ -1f,  1f, -1f, 0.0f, 0.0f },
			//{ -1f,-1f, 1f, 0.0f, 0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f, 0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f, 1.0f }

			//approximately what we want (neutral whites)
			//{  0.386976182f,  -0.619064033f,  -0.619064152f,  0.0f,  0.0f },
			//{  -1.21055317f,  -0.2224809f,  -1.22248077f,  0.0f,  0.0f },
			//{  -0.162890315f,  -0.164495364f,  0.8355047f,  0.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f,  1.0f }
			//use this one. clearer:
			//{  0.39f,-0.62f,-0.62f, 0.0f, 0.0f },
			//{ -1.21f,-0.22f,-1.22f, 0.0f, 0.0f },
			//{ -0.16f,-0.16f, 0.84f, 0.0f, 0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f, 0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f, 1.0f }

			//approximately what we want (too saturated, yellows and blues not so good)
			//{  1.089508f,  -0.9326327f,  -0.932633042f,  0.0f,  0.0f },
			//{  -1.81771779f,  0.1683074f,  -1.84169245f,  0.0f,  0.0f },
			//{  -0.244589478f,  -0.247815639f,  1.7621845f,  0.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f,  1.0f }
			
			//approximately what we want (red ambience)
			//{ 0.410639971f,  -0.629626155f,  -0.6296263f,  0.0f,  0.0f },
			//{  -1.1638236f, -0.2433382f,  -1.24333811f,  0.0f,  0.0f },
			//{ -0.1566024f,  -0.167301863f, 0.832698166f,  0.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f,  1.0f }

			//approximately what we want (blue ambience)
			//{  0.32f, -0.59f, -0.59f,  0.0f,  0.0f },
			//{  -1.34f,  -0.16f, -1.16f,  0.0f,  0.0f },
			//{  -0.18f,  -0.16f,  0.84f,  1.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f,  1.0f }

			//gray scale
			//{  0.3f,  0.3f,  0.3f,  0.0f,  0.0f },
			//{  0.6f,  0.6f,  0.6f,  0.0f,  0.0f },
			//{  0.1f,  0.1f,  0.1f,  0.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f } 
			//inversion
			//{ -1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
			//{  0.0f, -1.0f,  0.0f,  0.0f,  0.0f },
			//{  0.0f,  0.0f, -1.0f,  0.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
			//{  1.0f,  1.0f,  1.0f,  0.0f,  1.0f }
			//identity
			//{  1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
			//{  0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
			//{  0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
			//{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
		});
			var xx = NativeMethods.MagSetColorEffect(hwndMag, ref a);
			var yy = NativeMethods.MagGetColorEffect(hwndMag, ref b);

			if (!NativeMethods.MagSetWindowSource(this.hwndMag, new RECT(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Right, screen.Bounds.Bottom)))
			{
				throw new Exception("MagSetWindowSource()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			try
			{
				//fail on Windows Vista
				bool preventFading = true;
				if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_EXCLUDED_FROM_PEEK, ref preventFading, sizeof(int)) != 0)
				{
					throw new Exception("DwmSetWindowAttribute(DWMWA_EXCLUDED_FROM_PEEK)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
			}
			catch (Exception) { }

			DWMFLIP3DWINDOWPOLICY threeDPolicy = DWMFLIP3DWINDOWPOLICY.DWMFLIP3D_EXCLUDEABOVE;
			if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_FLIP3D_POLICY, ref threeDPolicy, sizeof(int)) != 0)
			{
				throw new Exception("DwmSetWindowAttribute(DWMWA_FLIP3D_POLICY)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			try
			{
				//fail on Windows Vista
				bool disallowPeek = true;
				if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_DISALLOW_PEEK, ref disallowPeek, sizeof(int)) != 0)
				{
					throw new Exception("DwmSetWindowAttribute(DWMWA_DISALLOW_PEEK)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
			}
			catch (Exception) { }

			this.Show();
		}

	}
}