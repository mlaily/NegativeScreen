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
			currentMatrix = Configuration.Current.InitialColorEffect;
			RegisterHotKey(Configuration.Current.ToggleKey);
			RegisterHotKey(Configuration.Current.ExitKey);
			foreach (var item in Configuration.Current.ColorEffects)
			{
				if (item.Key != HotKey.Empty)
				{
					RegisterHotKey(item.Key);
				}
			}

			InitializeComponent();
			toggleInversionToolStripMenuItem.ShortcutKeyDisplayString = Configuration.Current.ToggleKey.ToString();
			exitToolStripMenuItem.ShortcutKeyDisplayString = Configuration.Current.ExitKey.ToString();
			InitializeContextMenu();
			InitializeRefreshLoop();
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
					SafeChangeColorEffect(effect.Matrix);
				};
				this.changeModeToolStripMenuItem.DropDownItems.Add(menuItem);
			}
		}

		private void InitializeRefreshLoop()
		{
			if (!NativeMethods.MagInitialize())
			{
				throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			ToggleColorEffect(true);
			System.Threading.Thread t = new System.Threading.Thread(RefreshLoop);
			t.SetApartmentState((System.Threading.ApartmentState.STA));
			t.Start();
		}

		private void RefreshLoop()
		{
			bool running = true;
			while (running && !exiting)
			{
				System.Threading.Thread.Sleep(100);
				if (mainLoopPaused)
				{
					ToggleColorEffect(false);
					if (!NativeMethods.MagUninitialize())
					{
						throw new Exception("MagUninitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
					}
					while (mainLoopPaused && !exiting)
					{
						System.Threading.Thread.Sleep(100);
					}
					//we need to reinitialize
					running = false;
				}
			}
			if (!exiting)
			{
				InitializeRefreshLoop();
			}
		}

		public void RegisterHotKey(HotKey hotkey)
		{
			if (!NativeMethods.RegisterHotKey(this.Handle, hotkey.Id, hotkey.Modifiers, hotkey.Key))
			{
				StringBuilder message = new StringBuilder();
				message.Append("Unable to register the hot key \"");
				if (hotkey.Modifiers.HasFlag(KeyModifiers.MOD_ALT))
				{
					message.Append("Alt+");
				}
				if (hotkey.Modifiers.HasFlag(KeyModifiers.MOD_CONTROL))
				{
					message.Append("Ctrl+");
				}
				if (hotkey.Modifiers.HasFlag(KeyModifiers.MOD_SHIFT))
				{
					message.Append("Shift+");
				}
				if (hotkey.Modifiers.HasFlag(KeyModifiers.MOD_WIN))
				{
					message.Append("Win+");
				}
				message.Append(Enum.GetName(typeof(Keys), hotkey.Key));
				message.Append("\"");
				throw new Exception(message.ToString(), Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
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
									SafeChangeColorEffect(item.Value.Matrix);
								}
							}
							break;
					}
					break;
			}
			base.WndProc(ref m);
		}

		private void Exit()
		{
			if (!mainLoopPaused)
			{
				ToggleColorEffect(false);
			}
			this.exiting = true;
			this.Dispose();
			Application.Exit();
		}

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
		/// check if the magnification api is in a state where a color effect can be applied, then proceed.
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

		#endregion

		private void editConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!System.IO.File.Exists(Configuration.DefaultConfigurationFileName))
			{
				System.IO.File.WriteAllText(Configuration.DefaultConfigurationFileName, Configuration.DefaultConfiguration);
			}
			System.Diagnostics.Process.Start("notepad", Configuration.DefaultConfigurationFileName);
		}

		void trayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				Toggle();
			}
		}

	}
}
