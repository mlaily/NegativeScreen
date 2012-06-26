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
	/// <summary>
	/// inherits from Form so that hot keys can be bound to this "window"...
	/// </summary>
	class OverlayManager : Form
	{
		public const int HALT_HOTKEY_ID = 42;//random id =°
		public const int TOGGLE_HOTKEY_ID = 43;
		public const int RESET_TIMER_HOTKEY_ID = 44;
		public const int INCREASE_TIMER_HOTKEY_ID = 45;
		public const int DECREASE_TIMER_HOTKEY_ID = 46;

		//TODO: maybe I should think about loops and config file...
		public const int MODE1_HOTKEY_ID = 51;
		public const int MODE2_HOTKEY_ID = 52;
		public const int MODE3_HOTKEY_ID = 53;
		public const int MODE4_HOTKEY_ID = 54;
		public const int MODE5_HOTKEY_ID = 55;
		public const int MODE6_HOTKEY_ID = 56;
		public const int MODE7_HOTKEY_ID = 57;
		public const int MODE8_HOTKEY_ID = 58;
		public const int MODE9_HOTKEY_ID = 59;
		public const int MODE10_HOTKEY_ID = 60;

		private const int DEFAULT_INCREASE_STEP = 10;
		private const int DEFAULT_SLEEP_TIME = DEFAULT_INCREASE_STEP;
		private const int PAUSE_SLEEP_TIME = 100;

		/// <summary>
		/// control whether the main loop is paused or not.
		/// </summary>
		private bool mainLoopPaused = false;

		private int refreshInterval = DEFAULT_SLEEP_TIME;

		private List<NegativeOverlay> overlays = new List<NegativeOverlay>();

		private bool resolutionHasChanged = false;

		public OverlayManager()
		{
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

			if (!NativeMethods.RegisterHotKey(this.Handle, MODE1_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F1))
			{
				throw new Exception("RegisterHotKey(win+alt+F1)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE2_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F2))
			{
				throw new Exception("RegisterHotKey(win+alt+F2)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE3_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F3))
			{
				throw new Exception("RegisterHotKey(win+alt+F3)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE4_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F4))
			{
				throw new Exception("RegisterHotKey(win+alt+F4)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE5_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F5))
			{
				throw new Exception("RegisterHotKey(win+alt+F5)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE6_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F6))
			{
				throw new Exception("RegisterHotKey(win+alt+F6)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE7_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F7))
			{
				throw new Exception("RegisterHotKey(win+alt+F7)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE8_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F8))
			{
				throw new Exception("RegisterHotKey(win+alt+F8)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE9_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F9))
			{
				throw new Exception("RegisterHotKey(win+alt+F9)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE10_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F10))
			{
				throw new Exception("RegisterHotKey(win+alt+F10)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			if (!NativeMethods.MagInitialize())
			{
				throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

			Initialization();
		}

		void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			Console.WriteLine(DateTime.Now.ToString());
			//we can't start the loop here, in the event handler, because it seems to block the next events
			resolutionHasChanged = true;
		}

		private void Initialization()
		{
			foreach (var item in overlays)
			{
				item.Dispose();
			}
			overlays = new List<NegativeOverlay>();
			foreach (var item in Screen.AllScreens)
			{
				overlays.Add(new NegativeOverlay(item));
			}
			RefreshLoop(overlays);
		}

		private void RefreshLoop(List<NegativeOverlay> overlays)
		{
			bool noError = true;
			while (noError)
			{

				if (resolutionHasChanged)
				{
					resolutionHasChanged = false;
					//if the screen configuration change, we try to reinitialize all the overlays.
					//we break the loop. the initialization method is called...
					break;
				}

				for (int i = 0; i < overlays.Count; i++)
				{
					noError = RefreshOverlay(overlays[i]);
					if (!noError)
					{
						//application is exiting
						break;
					}
				}

				//Process Window messages
				Application.DoEvents();

				if (this.refreshInterval > 0)
				{
					System.Threading.Thread.Sleep(this.refreshInterval);
				}

				//pause
				while (mainLoopPaused)
				{
					for (int i = 0; i < overlays.Count; i++)
					{
						overlays[i].Visible = false;
					}
					System.Threading.Thread.Sleep(PAUSE_SLEEP_TIME);
					Application.DoEvents();
					if (!mainLoopPaused)
					{
						for (int i = 0; i < overlays.Count; i++)
						{
							overlays[i].Visible = true;
						}
					}
				}
			}
			if (noError)
			{
				//the loop broke because of a screen resolution change
				Initialization();
			}
		}

		/// <summary>
		/// return true on success, false on failure.
		/// </summary>
		/// <returns></returns>
		private bool RefreshOverlay(NegativeOverlay overlay)
		{
			try
			{
				// Reclaim topmost status. 
				if (!NativeMethods.SetWindowPos(overlay.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
			   (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE))
				{
					throw new Exception("SetWindowPos()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
				// Force redraw.
				if (!NativeMethods.InvalidateRect(overlay.HwndMag, IntPtr.Zero, true))
				{
					throw new Exception("InvalidateRect()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
				return true;
			}
			catch (ObjectDisposedException)
			{
				//application is exiting
				return false;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void UnregisterHotKeys()
		{
			NativeMethods.UnregisterHotKey(this.Handle, HALT_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, TOGGLE_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, RESET_TIMER_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, INCREASE_TIMER_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, DECREASE_TIMER_HOTKEY_ID);

			NativeMethods.UnregisterHotKey(this.Handle, MODE1_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE2_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE3_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE4_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE5_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE6_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE7_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE8_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE9_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE10_HOTKEY_ID);
		}

		protected override void WndProc(ref Message m)
		{
			// Listen for operating system messages.
			switch (m.Msg)
			{
				case (int)WindowMessage.WM_DWMCOMPOSITIONCHANGED:
					//aero has been enabled/disabled. It causes the magnified control to stop working
					if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0)
					{
						//running Vista.
						//The creation of the magnification Window on this OS seems to change desktop composition,
						//leading to infinite loop
					}
					else
					{
						Initialization();
					}
					break;
				case (int)WindowMessage.WM_HOTKEY:
					switch ((int)m.WParam)
					{
						case HALT_HOTKEY_ID:
							//otherwise, if paused, the application never stops
							mainLoopPaused = false;
							this.Dispose();
							Application.Exit();
							break;
						case TOGGLE_HOTKEY_ID:
							this.mainLoopPaused = !mainLoopPaused;
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
						case MODE1_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.Negative);
							break;
						case MODE2_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeHueShift180);
							break;
						case MODE3_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeHueShift180Variation1);
							break;
						case MODE4_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeHueShift180Variation2);
							break;
						case MODE5_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeHueShift180Variation3);
							break;
						case MODE6_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeHueShift180Variation4);
							break;
						case MODE7_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeSepia);
							break;
						case MODE8_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeGrayScale);
							break;
						case MODE9_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.NegativeRed);
							break;
						case MODE10_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(overlays, BuiltinMatrices.Red);
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
