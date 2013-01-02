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
using System.Diagnostics;
using System.Windows.Forms;

namespace NegativeScreen
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			//check whether the current process is running under WoW64 mode
			if (NativeMethods.IsX86InWow64Mode())
			{
				//see http://social.msdn.microsoft.com/Forums/en-US/windowsaccessibilityandautomation/thread/6cc761ea-8a54-4403-9cca-2fa8680f4409/
				System.Windows.Forms.MessageBox.Show(
@"You are trying to run this program on a 64 bits processor whereas it was compiled for a 32 bits processor.
To avoid known bugs relative to the used APIs, please instead run the 64 bits compiled version.", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button1);
				return;
			}
			//check Windows version
			//TODO: check whether the undocumented functions exist under Windows server 2008 (probably not) and R2 (probably yes)
			if (Environment.OSVersion.Version < new Version(6, 1))
			{
				System.Windows.Forms.MessageBox.Show(
@"Sorry, this version only works on Windows 7 and above. :/
There is a Vista version though. You can download it on
http://x2a.yt?negativescreen", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button1);
				return;
			}
			//check whether aero is enabled
			if (!NativeMethods.DwmIsCompositionEnabled())
			{
				var result = System.Windows.Forms.MessageBox.Show("Windows Aero should be enabled for this program to work properly!\nOtherwise, you may experience bad performances.", "Warning", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
				if (result != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}
			}
			//check whether the current application is already running
			if (IsAnotherInstanceAlreadyRunning())
			{
				System.Windows.Forms.MessageBox.Show("The application is already running!", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
				return;
			}
			//without this call, and with custom DPI settings,
			//the magnified window is either partially out of the screen,
			//or blurry, if the transformation scale is forced to 1.
			NativeMethods.SetProcessDPIAware();

			Configuration.Initialize();

			Application.EnableVisualStyles();
			OverlayManager.Initialize();

			Application.Run();
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.ExceptionObject.ToString(), "Sorry, I'm bailing out!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static bool IsAnotherInstanceAlreadyRunning()
		{
			Process me = Process.GetCurrentProcess();
			Process[] processesWithSameName = Process.GetProcessesByName(me.ProcessName);
			foreach (Process process in processesWithSameName)
			{
				// same process name, was started from the same file name and location.
				if (process.Id != me.Id && process.MainModule.FileName == me.MainModule.FileName)
				{
					return true;
				}
			}
			return false;
		}

	}
}