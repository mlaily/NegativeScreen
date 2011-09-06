using System;
using System.Diagnostics;

namespace NegativeScreen
{
	class Program
	{
		static void Main(string[] args)
		{
			//check whenever the current process is running under WoW64 mode
			if (NativeMethods.IsX86InWow64Mode())
			{
				System.Windows.Forms.MessageBox.Show(
@"You are trying to run this program on a 64 bits processor whereas it was compiled for a 32 bits processor.
To avoid known bugs relative to the used APIs, please instead run the 64 bits compiled version.", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button1);
				return;
			}
			//check whenever the current application is already running
			if (IsAnotherInstanceAlreadyRunning())
			{
				System.Windows.Forms.MessageBox.Show("The application is already running!", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
				return;
			}
			NegativeOverlay overlay = new NegativeOverlay();
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