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
	public class NegativeOverlay : Form
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
			this.Location = screen.Bounds.Location;
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

			//initial color transformation: simple negative
			BuiltinMatrices.ChangeColorEffect(hwndMag, BuiltinMatrices.Negative);

			if (!NativeMethods.MagSetWindowSource(this.hwndMag, new RECT(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Right, screen.Bounds.Bottom)))
			{
				throw new Exception("MagSetWindowSource()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			//reset magnification factor to 1.0
			//needed when running without aero (otherwise, the screen appears unicolored)
			Transformation transformation = new Transformation(1.0f);
			if (!NativeMethods.MagSetWindowTransform(this.hwndMag, ref transformation))
			{
				throw new Exception("MagSetWindowTransform()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			try
			{
				//fails on Windows Vista
				bool preventFading = true;
				if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_EXCLUDED_FROM_PEEK, ref preventFading, sizeof(int)) != 0)
				{
					throw new Exception("DwmSetWindowAttribute(DWMWA_EXCLUDED_FROM_PEEK)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
			}
			catch (Exception) { }

            try
            {
                //fails on Windows 8
                DWMFLIP3DWINDOWPOLICY threeDPolicy = DWMFLIP3DWINDOWPOLICY.DWMFLIP3D_EXCLUDEABOVE;
                if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_FLIP3D_POLICY, ref threeDPolicy, sizeof(int)) != 0)
                {
                    throw new Exception("DwmSetWindowAttribute(DWMWA_FLIP3D_POLICY)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
                }
            }
            catch (Exception) { }

			try
			{
				//fails on Windows Vista
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