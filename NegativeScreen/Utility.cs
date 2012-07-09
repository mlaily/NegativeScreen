using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SlimDX;
using SlimDX.Direct3D9;
using System.Windows.Forms;

namespace NegativeScreen
{
	public static class Utility
	{
		public static bool IsDark()
		{
			Surface surface = DxScreenCapture.CaptureScreen();
			using (surface)
			{
				DataRectangle rectangle = surface.LockRectangle(LockFlags.None);
				DataStream stream = rectangle.Data;

				//Stopwatch sw = Stopwatch.StartNew();
				bool result = IsDark(stream);
				//sw.Stop();

				//Debug.WriteLine("{0} - {1}ms", result, sw.ElapsedMilliseconds);

				surface.UnlockRectangle();
				return result;
			}
		}

		private static bool IsDark(DataStream bitmap, byte tolerance = 128, double darkPercent = 0.5)
		{
			long count = 0, all = bitmap.Length / 400;
			byte[] buffer = new byte[4];
			//int y = 0;

			for (int i = 0; i < bitmap.Length; i += 400)
			{
				//y++;
				bitmap.Position = i;
				bitmap.Read(buffer, 0, 4);
				byte r = buffer[2], g = buffer[1], b = buffer[0];
				byte brightness = (byte)Math.Round((r + g + b) / 3.0/*(0.299 * r + 0.5876 * g + 0.114 * b)*/);
				if (brightness <= tolerance)
					count++;
			}
			return (1d * count / all) > darkPercent;
		}
	}

	public static class DxScreenCapture
	{
		private static Device device;

		static DxScreenCapture()
		{
			PresentParameters presentParams = new PresentParameters();
			presentParams.Windowed = true;
			presentParams.SwapEffect = SwapEffect.Discard;
			device = new Device(new Direct3D(), 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing, presentParams);
		}

		public static Surface CaptureScreen()
		{
			Surface s = Surface.CreateOffscreenPlain(device, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);
			device.GetFrontBufferData(0, s);
			return s;
		}
	}
}
