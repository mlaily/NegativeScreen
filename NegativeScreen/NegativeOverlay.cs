using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace NegativeScreen
{
	class NegativeOverlay : Form
	{
		private IntPtr hwndMag;

		private const int HALT_HOTKEY_ID = 42;//random id =°
		private const int TOGGLE_HOTKEY_ID = 43;
		private const int RESET_TIMER_HOTKEY_ID = 44;
		private const int INCREASE_TIMER_HOTKEY_ID = 45;
		private const int DECREASE_TIMER_HOTKEY_ID = 46;

		private const int DEFAULT_INCREASE_STEP = 10;
		private const int DEFAULT_SLEEP_TIME = DEFAULT_INCREASE_STEP;
		private const int PAUSE_SLEEP_TIME = 100;

		/// <summary>
		/// allow to control whether the main loop is running or not. (pause inversion)
		/// </summary>
		private bool mainLoopRunning = true;

		private int refreshInterval = DEFAULT_SLEEP_TIME;

		public NegativeOverlay(int refreshIntervalValue = DEFAULT_SLEEP_TIME)
			: base()
		{
			this.refreshInterval = refreshIntervalValue;

			this.TopMost = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
			this.ShowInTaskbar = false;

			//
			if (!NativeMethods.RegisterHotKey(this.Handle, HALT_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.H))
			{
				throw new Exception("RegisterHotKey(win+alt+H)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, TOGGLE_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.N))
			{
				throw new Exception("RegisterHotKey(win+alt+N)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, RESET_TIMER_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.Multiply))
			{
				throw new Exception("RegisterHotKey(win+alt+Multiply)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, INCREASE_TIMER_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.Add))
			{
				throw new Exception("RegisterHotKey(win+alt+Add)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, DECREASE_TIMER_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.Subtract))
			{
				throw new Exception("RegisterHotKey(win+alt+Substract)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			if (!NativeMethods.MagInitialize())
			{
				throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

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
				(int)WindowStyles.WS_VISIBLE |
			(int)MagnifierStyle.MS_INVERTCOLORS,
				0, 0, Screen.GetBounds(this).Right, Screen.GetBounds(this).Bottom,
				this.Handle, IntPtr.Zero, hInst, IntPtr.Zero);

			if (hwndMag == IntPtr.Zero)
			{
				throw new Exception("CreateWindowEx()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			bool preventFading = true;
			if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_EXCLUDED_FROM_PEEK, ref preventFading, sizeof(int)) != 0)
			{
				throw new Exception("DwmSetWindowAttribute(DWMWA_EXCLUDED_FROM_PEEK)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			DWMFLIP3DWINDOWPOLICY threeDPolicy = DWMFLIP3DWINDOWPOLICY.DWMFLIP3D_EXCLUDEABOVE;
			if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_FLIP3D_POLICY, ref threeDPolicy, sizeof(int)) != 0)
			{
				throw new Exception("DwmSetWindowAttribute(DWMWA_FLIP3D_POLICY)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			bool disallowPeek = true;
			if (NativeMethods.DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_DISALLOW_PEEK, ref disallowPeek, sizeof(int)) != 0)
			{
				throw new Exception("DwmSetWindowAttribute(DWMWA_DISALLOW_PEEK)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			this.Show();

			bool noError = true;
			while (noError)
			{
				try
				{
					// Reclaim topmost status, to prevent unmagnified menus from remaining in view. 
					if (!NativeMethods.SetWindowPos(this.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
				   (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE))
					{
						throw new Exception("SetWindowPos()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
					}
					// Force redraw.
					if (!NativeMethods.InvalidateRect(hwndMag, IntPtr.Zero, true))
					{
						throw new Exception("InvalidateRect()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
					}
					//Process Window messages
					Application.DoEvents();
				}
				catch (ObjectDisposedException)
				{
					//application is exiting
					noError = false;
					break;
				}
				catch (Exception)
				{
					throw;
				}

				if (this.refreshInterval > 0)
				{
					System.Threading.Thread.Sleep(this.refreshInterval);
				}

				//pause
				while (!mainLoopRunning)
				{
					this.Visible = false;
					System.Threading.Thread.Sleep(PAUSE_SLEEP_TIME);
					Application.DoEvents();
					if (mainLoopRunning)
					{
						this.Visible = true;
					}
				}
			}

		}

		private void UnregisterHotKeys()
		{
			NativeMethods.UnregisterHotKey(this.Handle, HALT_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, TOGGLE_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, RESET_TIMER_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, INCREASE_TIMER_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, DECREASE_TIMER_HOTKEY_ID);
		}

		protected override void WndProc(ref Message m)
		{
			// Listen for operating system messages.
			switch (m.Msg)
			{
				case (int)WindowMessage.WM_HOTKEY:
					switch ((int)m.WParam)
					{
						case HALT_HOTKEY_ID:
							UnregisterHotKeys();
							NativeMethods.MagUninitialize();
							Application.Exit();
							break;
						case TOGGLE_HOTKEY_ID:
							this.mainLoopRunning = !mainLoopRunning;
							break;
						case RESET_TIMER_HOTKEY_ID:
							this.refreshInterval = DEFAULT_SLEEP_TIME;
							break;
						case INCREASE_TIMER_HOTKEY_ID:
							this.refreshInterval += DEFAULT_INCREASE_STEP;
							break;
						case DECREASE_TIMER_HOTKEY_ID:
							this.refreshInterval -= DEFAULT_INCREASE_STEP;
							if (this.refreshInterval < 0)
							{
								this.refreshInterval = 0;
							}
							break;
						default:
							break;
					}
					break;
			}
			base.WndProc(ref m);
		}

		protected override void Dispose(bool disposing)
		{
			UnregisterHotKeys();
			NativeMethods.MagUninitialize();
			base.Dispose(disposing);
		}

	}
}