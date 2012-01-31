//Copyright 2011 Melvyn Laily
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

		private const int DEFAULT_INCREASE_STEP = 10;
		private const int DEFAULT_SLEEP_TIME = DEFAULT_INCREASE_STEP;
		private const int PAUSE_SLEEP_TIME = 100;

		/// <summary>
		/// control whether the main loop is running or not. (pause inversion)
		/// </summary>
		private bool mainLoopRunning = true;

		private int refreshInterval = DEFAULT_SLEEP_TIME;

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

			if (!NativeMethods.MagInitialize())
			{
				throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			List<NegativeOverlay> overlays = new List<NegativeOverlay>();
			overlays.Add(new NegativeOverlay(Screen.PrimaryScreen));
			RefreshLoop(overlays);
		}

		private void RefreshLoop(List<NegativeOverlay> overlays)
		{
			bool noError = true;
			while (noError)
			{
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
				while (!mainLoopRunning)
				{
					for (int i = 0; i < overlays.Count; i++)
					{
						overlays[i].Visible = false;
					}
					System.Threading.Thread.Sleep(PAUSE_SLEEP_TIME);
					Application.DoEvents();
					if (mainLoopRunning)
					{
						for (int i = 0; i < overlays.Count; i++)
						{
							overlays[i].Visible = true;
						}
					}
				}
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

		//TODO
		public void IncreaseRefreshInterval() { }
		public void DecreaseRefreshInterval() { }
		public void ResetRefreshInterval() { }

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
							//otherwise, if paused, the application never stops
							mainLoopRunning = true;
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
