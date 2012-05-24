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

		/// <summary>
		/// control whether the main loop is paused or not.
		/// </summary>
		private bool mainLoopPaused = false;

		/// <summary>
		/// allow to exit the main loop
		/// </summary>
		private bool exiting = false;

		/// <summary>
		/// memorize the current color matrix. start with simple negative
		/// </summary>
		private float[,] currentMatrix = BuiltinMatrices.Negative;

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

			Initialization();
		}

		private void Initialization()
		{
			if (!NativeMethods.MagInitialize())
			{
				throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			BuiltinMatrices.InterpolateColorEffect(BuiltinMatrices.Identity,currentMatrix);
			RefreshLoop();
		}

		private void RefreshLoop()
		{
			bool running = true;
			while (running && !exiting)
			{
				System.Threading.Thread.Sleep(100);
				Application.DoEvents();
				if (mainLoopPaused)
				{
					BuiltinMatrices.InterpolateColorEffect(currentMatrix, BuiltinMatrices.Identity);
					if (!NativeMethods.MagUninitialize())
					{
						throw new Exception("MagUninitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
					}
					while (mainLoopPaused && !exiting)
					{
						System.Threading.Thread.Sleep(100);
						Application.DoEvents();
					}
					//we need to reinitialize
					running = false;
				}
			}
			if (!exiting)
			{
				Initialization();
			}
		}

		private void UnregisterHotKeys()
		{
			try
			{
				NativeMethods.UnregisterHotKey(this.Handle, HALT_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, TOGGLE_HOTKEY_ID);

				NativeMethods.UnregisterHotKey(this.Handle, MODE1_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE2_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE3_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE4_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE5_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE6_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE7_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE8_HOTKEY_ID);
				NativeMethods.UnregisterHotKey(this.Handle, MODE9_HOTKEY_ID);
			}
			catch (Exception) { }
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
							BuiltinMatrices.InterpolateColorEffect(currentMatrix, BuiltinMatrices.Identity);
							this.exiting = true;
							this.Dispose();
							Application.Exit();
							break;
						case TOGGLE_HOTKEY_ID:
							this.mainLoopPaused = !mainLoopPaused;
							break;
						case MODE1_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.Negative);
							break;
						case MODE2_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.NegativeHueShift180);
							break;
						case MODE3_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.NegativeHueShift180Variation1);
							break;
						case MODE4_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.NegativeHueShift180Variation2);
							break;
						case MODE5_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.NegativeHueShift180Variation3);
							break;
						case MODE6_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.NegativeHueShift180Variation4);
							break;
						case MODE7_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.NegativeSepia);
							break;
						case MODE8_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.Sepia);
							break;
						case MODE9_HOTKEY_ID:
							SafeChangeColorEffect(BuiltinMatrices.GrayScale);
							break;
						default:
							break;
					}
					break;
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// check if the magnification api is in a state where a color effect can be applied, then proceed.
		/// </summary>
		/// <param name="matrix"></param>
		private void SafeChangeColorEffect(float[,] matrix)
		{

			if (!mainLoopPaused && !exiting)
			{
				BuiltinMatrices.InterpolateColorEffect(currentMatrix, matrix);
			}
			currentMatrix = matrix;
		}

		protected override void Dispose(bool disposing)
		{
			UnregisterHotKeys();
			NativeMethods.MagUninitialize();
			base.Dispose(disposing);
		}

	}
}
