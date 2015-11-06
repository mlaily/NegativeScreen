// Copyright 2011-2014 Melvyn Laily
// http://arcanesanctum.net

// This file is part of NegativeScreen.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

namespace NegativeScreen
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			// check whether the current process is running under WoW64 mode
			if (NativeMethods.IsX86InWow64Mode())
			{
				// see http://social.msdn.microsoft.com/Forums/en-US/windowsaccessibilityandautomation/thread/6cc761ea-8a54-4403-9cca-2fa8680f4409/
				MessageBox.Show(
@"You are trying to run this program on a 64 bits processor whereas it was compiled for a 32 bits processor.
To avoid known bugs relative to the used APIs, please instead run the 64 bits compiled version.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}

			// check Windows version
			// TODO: check whether the undocumented functions exist under Windows server 2008 (probably not) and R2 (probably yes)
			if (Environment.OSVersion.Version < new Version(6, 1))
			{
				System.Windows.Forms.MessageBox.Show(
@"Sorry, this version only works on Windows 7 and above. :/
There is a Vista version though. You can download it on
http://x2a.yt?negativescreen", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}

			// forces the working directory to be the one of the executable
			Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			Configuration.Initialize();

			// check whether aero is enabled
			if (Configuration.Current.ShowAeroWarning && !NativeMethods.DwmIsCompositionEnabled())
			{
				var result = MessageBox.Show("Windows Aero must be enabled for this program to work properly!", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
				if (result != DialogResult.OK)
				{
					return;
				}
			}
			// check whether the current application is already running
			Process aleadyRunningInstance;
			if (IsAnotherInstanceAlreadyRunning(out aleadyRunningInstance))
			{
				// There is no way to know which thread is the main thread (where the message loop is)
				// so we don't take any chance...
				foreach (ProcessThread thread in aleadyRunningInstance.Threads)
				{
					// The goal is to enable the already running instance color effect:
					NativeMethods.PostThreadMessage((uint)thread.Id, UserMessageFilter.WM_ENABLE_COLOR_EFFECT, IntPtr.Zero, IntPtr.Zero);
				}
				return;
			}
			// without this call, and with custom DPI settings,
			// the magnified window is either partially out of the screen,
			// or blurry, if the transformation scale is forced to 1.
			NativeMethods.SetProcessDPIAware();

			Application.EnableVisualStyles();
			Application.AddMessageFilter(new UserMessageFilter());

			OverlayManager.Initialize();

			Application.Run();
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.ExceptionObject.ToString(), "Sorry, I'm bailing out!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static bool IsAnotherInstanceAlreadyRunning(out Process alreadyRunningInstance)
		{
			Process me = Process.GetCurrentProcess();
			Process[] processesWithSameName = Process.GetProcessesByName(me.ProcessName);
			foreach (Process process in processesWithSameName)
			{
				// same process name, was started from the same file name and location.
				if (process.Id != me.Id && process.MainModule.FileName == me.MainModule.FileName)
				{
					alreadyRunningInstance = process;
					return true;
				}
			}
			alreadyRunningInstance = null;
			return false;
		}
	}

	/// <summary>
	/// This class is required to intercept Windows messages sent via PostThreadMessage()
	/// as for unknown reasons, the overridden WndProc() of a <see cref="Form"/> never receives them...
	/// </summary>
	public class UserMessageFilter : IMessageFilter
	{
		public const int WM_ENABLE_COLOR_EFFECT = (int)WindowMessage.WM_USER + 0;

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == WM_ENABLE_COLOR_EFFECT)
			{
				// Handle a custom WM_ENABLE_COLOR_EFFECT message
				// so that NegativeScreen can be enabled from another process/instance.
				OverlayManager.Instance.Enable();
			}
			return false;
		}
	}
}