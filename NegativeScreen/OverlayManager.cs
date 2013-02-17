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
using System.Collections.Concurrent;

namespace NegativeScreen
{
	/// <summary>
	/// inherits from Form so that hot keys can be bound to its message loop
	/// </summary>
	partial class OverlayManager : Form
	{

		/// <summary>
		/// control whether the main loop is paused or not.
		/// </summary>
		private bool mainLoopPaused = false;

		/// <summary>
		/// allow to exit the main loop
		/// </summary>
		private bool exiting = false;

		/// <summary>
		/// memorize the current color matrix.
		/// </summary>
		private float[,] currentMatrix = null;

		// /!\ The full screen magnifier seems not to be thread-safe on Windows 8 at least,
		// so every call after initialization must be done on the same thread.
		#region Inter-thread color effect calls

		/// <summary>
		/// allow to execute magnifer api calls on the right thread.
		/// </summary>
		private ScreenColorEffect invokeColorEffect;
		private bool shouldInvokeColorEffect;
		private object invokeColorEffectLock = new object();

		/// <summary>
		/// Ask for a color effect change to be executed on the right thread.
		/// </summary>
		/// <param name="colorEffect"></param>
		private void InvokeColorEffect(ScreenColorEffect colorEffect)
		{
			lock (invokeColorEffectLock)
			{
				invokeColorEffect = colorEffect;
				shouldInvokeColorEffect = true;
			}
		}

		/// <summary>
		/// Execute every specified actions in the concurrent queue, on the right thread.
		/// </summary>
		private void DoMagnifierApiInvokes()
		{
			lock (invokeColorEffectLock)
			{
				if (shouldInvokeColorEffect)
				{
					SafeChangeColorEffect(invokeColorEffect.Matrix);
				}
				shouldInvokeColorEffect = false;
			}
		}

		#endregion

		private static OverlayManager _Instance;
		public static OverlayManager Instance
		{
			get
			{
				Initialize();
				return _Instance;
			}
		}

		public static void Initialize()
		{
			if (_Instance == null)
			{
				_Instance = new OverlayManager();
			}
		}

		private OverlayManager()
		{
			InitializeComponent();

			currentMatrix = Configuration.Current.InitialColorEffect;
			TryRegisterHotKeys(this.trayIcon);

			toggleInversionToolStripMenuItem.ShortcutKeyDisplayString = Configuration.Current.ToggleKey.ToString();
			exitToolStripMenuItem.ShortcutKeyDisplayString = Configuration.Current.ExitKey.ToString();
			InitializeContextMenu();

			InitializeControlLoop();
		}

		private void TryRegisterHotKeys(NotifyIcon trayIcon)
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
				trayIcon.ShowBalloonTip(4000, "Warning", sb.ToString(), ToolTipIcon.Warning);
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

		private void InitializeContextMenu()
		{
			foreach (var item in Configuration.Current.ColorEffects)
			{
				var menuItem = new ToolStripMenuItem(item.Value.Description);
				menuItem.Tag = item.Value;
				menuItem.ShortcutKeyDisplayString = item.Key.ToString();
				menuItem.Click += (s, e) =>
				{
					var effect = (ScreenColorEffect)((ToolStripMenuItem)s).Tag;
					InvokeColorEffect(effect);
				};
				this.changeModeToolStripMenuItem.DropDownItems.Add(menuItem);
			}
		}

		private void InitializeControlLoop()
		{
			System.Threading.Thread t = new System.Threading.Thread(ControlLoop);
			t.SetApartmentState((System.Threading.ApartmentState.STA));
			t.Start();
		}

		/// <summary>
		/// Main loop, in charge of controling the magnification api.
		/// </summary>
		private void ControlLoop()
		{
			if (!Configuration.Current.ActiveOnStartup)
			{
				mainLoopPaused = true;
				PauseLoop();
			}
			while (!exiting)
			{
				if (!NativeMethods.MagInitialize())
				{
					throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
				ToggleColorEffect(fromNormal: true);
				while (!exiting)
				{
					System.Threading.Thread.Sleep(Configuration.Current.MainLoopRefreshTime);
					DoMagnifierApiInvokes();
					if (mainLoopPaused)
					{
						ToggleColorEffect(fromNormal: false);
						if (!NativeMethods.MagUninitialize())
						{
							throw new Exception("MagUninitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
						}
						PauseLoop();
						//we need to reinitialize
						break;
					}
				}
			}
			this.Invoke((Action)(() =>
				{
					this.Dispose();
					Application.Exit();
				}));
		}

		private void PauseLoop()
		{
			while (mainLoopPaused && !exiting)
			{
				System.Threading.Thread.Sleep(Configuration.Current.MainLoopRefreshTime);
				DoMagnifierApiInvokes();
			}
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

		protected override void WndProc(ref Message m)
		{
			// Listen for operating system messages.
			switch (m.Msg)
			{
				case (int)WindowMessage.WM_HOTKEY:
					int HotKeyId = (int)m.WParam;
					switch (HotKeyId)
					{
						case HotKey.ExitKeyId:
							Exit();
							break;
						case HotKey.ToggleKeyId:
							Toggle();
							break;
						default:
							foreach (var item in Configuration.Current.ColorEffects)
							{
								if (item.Key.Id == HotKeyId)
								{
									InvokeColorEffect(item.Value);
								}
							}
							break;
					}
					break;
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// Can be called from any thread.
		/// </summary>
		private void Exit()
		{
			if (!mainLoopPaused)
			{
				mainLoopPaused = true;
			}
			this.exiting = true;
		}

		/// <summary>
		/// Can be called from any thread.
		/// </summary>
		private void Toggle()
		{
			this.mainLoopPaused = !mainLoopPaused;
		}

		private void ToggleColorEffect(bool fromNormal)
		{
			if (fromNormal)
			{
				if (Configuration.Current.SmoothToggles)
				{
					BuiltinMatrices.InterpolateColorEffect(BuiltinMatrices.Identity, currentMatrix);
				}
				else
				{
					BuiltinMatrices.ChangeColorEffect(currentMatrix);
				}
			}
			else
			{
				if (Configuration.Current.SmoothToggles)
				{
					BuiltinMatrices.InterpolateColorEffect(currentMatrix, BuiltinMatrices.Identity);
				}
				else
				{
					BuiltinMatrices.ChangeColorEffect(BuiltinMatrices.Identity);
				}
			}
		}

		/// <summary>
		/// Check if the magnification api is in a state where a color effect can be applied, then proceed.
		/// </summary>
		/// <param name="matrix"></param>
		private void SafeChangeColorEffect(float[,] matrix)
		{
			if (!mainLoopPaused && !exiting)
			{
				if (Configuration.Current.SmoothTransitions)
				{
					BuiltinMatrices.InterpolateColorEffect(currentMatrix, matrix);
				}
				else
				{
					BuiltinMatrices.ChangeColorEffect(matrix);
				}
			}
			currentMatrix = matrix;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				UnregisterHotKeys();
				NativeMethods.MagUninitialize();
			}
			base.Dispose(disposing);
		}

		#region Event Handlers

		private void OverlayManager_FormClosed(object sender, FormClosedEventArgs e)
		{
			Exit();
		}

		private void toggleInversionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Toggle();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Exit();
		}

		private void editConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!System.IO.File.Exists(Configuration.DefaultConfigurationFileName))
			{
				System.IO.File.WriteAllText(Configuration.DefaultConfigurationFileName, Configuration.DefaultConfiguration);
			}
			System.Diagnostics.Process.Start("notepad", Configuration.DefaultConfigurationFileName);
		}

		private void trayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				Toggle();
			}
		}

		#endregion

	}
}
