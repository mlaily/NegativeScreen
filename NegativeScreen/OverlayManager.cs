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
		public const int RESET_TIMER_HOTKEY_ID = 44;
		public const int INCREASE_TIMER_HOTKEY_ID = 45;
		public const int DECREASE_TIMER_HOTKEY_ID = 46;

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

		/// <summary>
		/// memorize the current color matrix.
		/// </summary>
		private float[,] currentMatrix = null;

		public OverlayManager()
		{
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

			TryRegisterHotKeys();

			if (!NativeMethods.MagInitialize())
			{
				throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			currentMatrix = Configuration.Current.InitialColorEffect;

			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

			if (!Configuration.Current.ActiveOnStartup)
			{
				mainLoopPaused = true;
			}
			Initialization();
		}

		private void TryRegisterHotKeys()
		{
			StringBuilder sb = new StringBuilder("Unable to register one or more hot keys:\n");
			bool success = true;
			success &= TryRegisterHotKeyAppendError(Configuration.Current.ToggleKey, sb);
			success &= TryRegisterHotKeyAppendError(Configuration.Current.ExitKey, sb);
			foreach (var item in Configuration.Current.ColorEffects)
			{
				if (item.Key != HotKey.Empty)
				{
					success &= TryRegisterHotKeyAppendError(item.Key, sb);
				}
			}
			if (!success)
			{
				MessageBox.Show(sb.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private bool TryRegisterHotKeyAppendError(HotKey hotkey, StringBuilder appendErrorTo)
		{
			AlreadyRegisteredHotKeyException ex;
			if (!TryRegisterHotKey(hotkey, out ex))
			{
				appendErrorTo.AppendFormat(" - \"{0}\" : {1}", ex.HotKey, (ex.InnerException == null ? "" : ex.InnerException.Message));
				return false;
			}
			return true;
		}

		public bool TryRegisterHotKey(HotKey hotkey, out AlreadyRegisteredHotKeyException exception)
		{
			bool ok = NativeMethods.RegisterHotKey(this.Handle, hotkey.Id, hotkey.Modifiers, hotkey.Key);
			if (!ok)
			{
				exception = new AlreadyRegisteredHotKeyException(hotkey, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				return false;
			}
			else
			{
				exception = null;
				return true;
			}
		}

		private void UnregisterHotKeys()
		{
			try
			{
				NativeMethods.UnregisterHotKey(this.Handle, Configuration.Current.ToggleKey.Id);
				NativeMethods.UnregisterHotKey(this.Handle, Configuration.Current.ExitKey.Id);
			}
			catch (Exception) { }
		}


		void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			//Console.WriteLine(DateTime.Now.ToString());
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
				overlays.Add(new NegativeOverlay(item, currentMatrix));
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
					int HotKeyId = (int)m.WParam;
					switch (HotKeyId)
					{
						case HotKey.ExitKeyId:
							//otherwise, if paused, the application never stops
							mainLoopPaused = false;
							this.Dispose();
							Application.Exit(); break;
						case HotKey.ToggleKeyId:
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
						default:
							foreach (var item in Configuration.Current.ColorEffects)
							{
								if (item.Key.Id == HotKeyId)
								{
									BuiltinMatrices.ChangeColorEffect(overlays, item.Value.Matrix);
								}
							}
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
