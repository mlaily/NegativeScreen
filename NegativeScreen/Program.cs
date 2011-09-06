using System;
using System.Diagnostics;

namespace NegativeScreen
{
	class Program
	{
		static void Main(string[] args)
		{
			if (IsAnotherInstanceAlreadyRunning())
			{
				//show message and exit
				System.Windows.Forms.MessageBox.Show("The application is already running !", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button1);
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