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
using System.Runtime.InteropServices;
using System.IO;

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
			OverlayManager manager = new OverlayManager();
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{

			using (var writer = new System.IO.StreamWriter("UnhandledExceptionInfo.log", true, System.Text.Encoding.UTF8))
			{
				writer.WriteLine("sender: {0}", sender);
				writer.WriteLine("IsTerminating: {0}", e.IsTerminating);
				writer.WriteLine("exception: {0}", e.ExceptionObject);
				writer.WriteLine("--------------------------------------------------------------------------------");
			}
			WriteCrashDump();
		}

		[Flags]
		enum MINIDUMP_TYPE : uint
		{
			MiniDumpNormal = 0x00000000,
			MiniDumpWithDataSegs = 0x00000001,
			MiniDumpWithFullMemory = 0x00000002,
			MiniDumpWithHandleData = 0x00000004,
			MiniDumpFilterMemory = 0x00000008,
			MiniDumpScanMemory = 0x00000010,
			MiniDumpWithUnloadedModules = 0x00000020,
			MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
			MiniDumpFilterModulePaths = 0x00000080,
			MiniDumpWithProcessThreadData = 0x00000100,
			MiniDumpWithPrivateReadWriteMemory = 0x00000200,
			MiniDumpWithoutOptionalData = 0x00000400,
			MiniDumpWithFullMemoryInfo = 0x00000800,
			MiniDumpWithThreadInfo = 0x00001000,
			MiniDumpWithCodeSegs = 0x00002000,
			MiniDumpWithoutAuxiliaryState = 0x00004000,
			MiniDumpWithFullAuxiliaryState = 0x00008000,
			MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
			MiniDumpIgnoreInaccessibleMemory = 0x00020000,
			MiniDumpWithTokenInformation = 0x00040000
		}

		[DllImport("DbgHelp.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool MiniDumpWriteDump(
		IntPtr hProcess,
		int ProcessId,
		IntPtr hFile,
		MINIDUMP_TYPE DumpType,
		IntPtr ExceptionParam,
		IntPtr UserStreamParam,
		IntPtr CallbackParam
		);

		static void WriteCrashDump()
		{
			using (var process = Process.GetCurrentProcess())
			using (var file = File.Open("mem.dmp", FileMode.Create, FileAccess.Write))
			{
				var dumpType = MINIDUMP_TYPE.MiniDumpNormal;

				if (!MiniDumpWriteDump(
					process.Handle,
					process.Id,
					file.SafeFileHandle.DangerousGetHandle(),
					dumpType,
					IntPtr.Zero,
					IntPtr.Zero,
					IntPtr.Zero))
				{
					throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
				}
			}
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