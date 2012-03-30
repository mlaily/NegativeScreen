using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NegativeScreen
{
	class Utility
	{
		/// <summary>
		/// For fast access to pixels   
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		public static unsafe byte[] BitmapToByteArray(Bitmap bitmap)
		{
			BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
											 PixelFormat.Format32bppArgb);
			byte[] bytes = new byte[bmd.Height * bmd.Stride];
			byte* pnt = (byte*)bmd.Scan0;
			Marshal.Copy((IntPtr)pnt, bytes, 0, bmd.Height * bmd.Stride);
			bitmap.UnlockBits(bmd);
			return bytes;
		}

		/// <summary>
		/// http://stackoverflow.com/questions/1587674/how-to-identify-black-or-dark-images-in-c
		/// </summary>
		/// <param name="bitmap"></param>
		/// <param name="tolerance">pixel considered dark under this value (0-255)</param>
		/// <param name="darkPercent"></param>
		/// <returns></returns>
		public static bool IsDark(Bitmap bitmap, byte tolerance = 128, double darkPercent = 0.5)
		{
			byte[] bytes = BitmapToByteArray(bitmap);
			int count = 0, all = bitmap.Width * bitmap.Height;
			for (int i = 0; i < bytes.Length; i += 4)
			{
				byte r = bytes[i + 2], g = bytes[i + 1], b = bytes[i];
				byte brightness = (byte)Math.Round((r+g+b)/3.0/*(0.299 * r + 0.5876 * g + 0.114 * b)*/);
				if (brightness <= tolerance)
					count++;
			}
			return (1d * count / all) > darkPercent;
		}
	}
}
