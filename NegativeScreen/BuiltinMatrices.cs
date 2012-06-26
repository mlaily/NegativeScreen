using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NegativeScreen
{
	/// <summary>
	/// Store various built in ColorMatrix
	/// </summary>
	public static class BuiltinMatrices
	{
		/// <summary>
		/// no color transformation
		/// </summary>
		public static float[,] Identity { get; private set; }
		/// <summary>
		/// simple colors transformations
		/// </summary>
		public static float[,] Negative { get; private set; }
		public static float[,] GrayScale { get; private set; }
		public static float[,] Sepia { get; private set; }
		public static float[,] Red { get; private set; }
		public static float[,] HueShift180 { get; private set; }

		public static float[,] NegativeGrayScale { get; private set; }
		public static float[,] NegativeSepia { get; private set; }
		public static float[,] NegativeRed { get; private set; }

		/// <summary>
		/// theoretical optimal transfomation (but ugly desaturated pure colors due to "overflows"...)
		/// Many thanks to Tom MacLeod who gave me the idea for these inversion modes
		/// </summary>
		public static float[,] NegativeHueShift180 { get; private set; }
		/// <summary>
		/// high saturation, good pure colors
		/// </summary>
		public static float[,] NegativeHueShift180Variation1 { get; private set; }
		/// <summary>
		/// overall desaturated, yellows and blue plain bad. actually relaxing and very usable
		/// </summary>
		public static float[,] NegativeHueShift180Variation2 { get; private set; }
		/// <summary>
		/// high saturation. yellows and blues plain bad. actually quite readable
		/// </summary>
		public static float[,] NegativeHueShift180Variation3 { get; private set; }
		/// <summary>
		/// //not so readable, good colors (CMY colors a bit desaturated, still more saturated than normal)
		/// </summary>
		public static float[,] NegativeHueShift180Variation4 { get; private set; }

		static BuiltinMatrices()
		{
			Identity = new float[,] {
				{  1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			Negative = new float[,] {
				{ -1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f, -1.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f, -1.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  1.0f,  1.0f,  1.0f,  0.0f,  1.0f }
			};
			Red = new float[,] {
				{  1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			NegativeRed = Multiply(Negative, Red);
			GrayScale = new float[,] {
				{  0.3f,  0.3f,  0.3f,  0.0f,  0.0f },
				{  0.6f,  0.6f,  0.6f,  0.0f,  0.0f },
				{  0.1f,  0.1f,  0.1f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			NegativeGrayScale = Multiply(Negative, GrayScale);
			Sepia = new float[,] {
				{ .393f, .349f, .272f, 0.0f, 0.0f},
				{ .769f, .686f, .534f, 0.0f, 0.0f},
				{ .189f, .168f, .131f, 0.0f, 0.0f},
				{  0.0f,  0.0f,  0.0f, 1.0f, 0.0f},
				{  0.0f,  0.0f,  0.0f, 0.0f, 1.0f}
			};
			NegativeSepia = Multiply(Negative, Sepia);
			HueShift180 = new float[,] {
				{ -0.3333333f,  0.6666667f,  0.6666667f, 0.0f, 0.0f },
				{  0.6666667f, -0.3333333f,  0.6666667f, 0.0f, 0.0f },
				{  0.6666667f,  0.6666667f, -0.3333333f, 0.0f, 0.0f },
				{  0.0f,              0.0f,        0.0f, 1.0f, 0.0f },
				{  0.0f,              0.0f,        0.0f, 0.0f, 1.0f }
			};
			NegativeHueShift180 = Multiply(Negative, HueShift180);
			NegativeHueShift180Variation1 = new float[,] {
				//most simple working method for shifting hue 180deg.
				{  1.0f, -1.0f, -1.0f, 0.0f, 0.0f },
				{ -1.0f,  1.0f, -1.0f, 0.0f, 0.0f },
				{ -1.0f, -1.0f,  1.0f, 0.0f, 0.0f },
				{  0.0f,  0.0f,  0.0f, 1.0f, 0.0f },
				{  1.0f,  1.0f,  1.0f, 0.0f, 1.0f }
			};
			NegativeHueShift180Variation2 = new float[,] {
				//generated with QColorMatrix http://www.codeguru.com/Cpp/G-M/gdi/gdi/article.php/c3667
				{  0.39f, -0.62f, -0.62f, 0.0f, 0.0f },
				{ -1.21f, -0.22f, -1.22f, 0.0f, 0.0f },
				{ -0.16f, -0.16f,  0.84f, 0.0f, 0.0f },
				{   0.0f,   0.0f,   0.0f, 1.0f, 0.0f },
				{   1.0f,   1.0f,   1.0f, 0.0f, 1.0f }
			};
			NegativeHueShift180Variation3 = new float[,] {
				{     1.089508f,   -0.9326327f, -0.932633042f,  0.0f,  0.0f },
				{  -1.81771779f,    0.1683074f,  -1.84169245f,  0.0f,  0.0f },
				{ -0.244589478f, -0.247815639f,    1.7621845f,  0.0f,  0.0f },
				{          0.0f,          0.0f,          0.0f,  1.0f,  0.0f },
				{          1.0f,          1.0f,          1.0f,  0.0f,  1.0f }
			};
			NegativeHueShift180Variation4 = new float[,] {
				{  0.50f, -0.78f, -0.78f, 0.0f, 0.0f },
				{ -0.56f,  0.72f, -0.56f, 0.0f, 0.0f },
				{ -0.94f, -0.94f,  0.34f, 0.0f, 0.0f },
				{   0.0f,   0.0f,   0.0f, 1.0f, 0.0f },
				{   1.0f,   1.0f,   1.0f, 0.0f, 1.0f }
			};
		}

		public static float[,] Multiply(float[,] a, float[,] b)
		{
			if (a.GetLength(1) != b.GetLength(0))
			{
				throw new Exception("a.GetLength(1) != b.GetLength(0)");
			}
			float[,] c = new float[a.GetLength(0), b.GetLength(1)];
			for (int i = 0; i < c.GetLength(0); i++)
			{
				for (int j = 0; j < c.GetLength(1); j++)
				{
					for (int k = 0; k < a.GetLength(1); k++) // k<b.GetLength(0)
					{
						c[i, j] = c[i, j] + a[i, k] * b[k, j];
					}
				}
			}
			return c;
		}

		public static void ChangeColorEffect(IntPtr hwndMag, float[,] matrix)
		{
			ColorEffect colorEffect = new ColorEffect(matrix);
			if (!NativeMethods.MagSetColorEffect(hwndMag, ref colorEffect))
			{
				throw new Exception("MagSetColorEffect()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
		}

		public static void ChangeColorEffect(IEnumerable<NegativeOverlay> overlays, float[,] matrix)
		{
			foreach (var item in overlays)
			{
				ChangeColorEffect(item.HwndMag, matrix);
			}
		}

		public static float[,] MoreBlue(float[,] colorMatrix)
		{
			float[,] temp = (float[,])colorMatrix.Clone();
			temp[2, 4] += 0.1f;//or remove 0.1 off the red
			return temp;
		}

		public static float[,] MoreGreen(float[,] colorMatrix)
		{
			float[,] temp = (float[,])colorMatrix.Clone();
			temp[1, 4] += 0.1f;
			return temp;
		}

		public static float[,] MoreRed(float[,] colorMatrix)
		{
			float[,] temp = (float[,])colorMatrix.Clone();
			temp[0, 4] += 0.1f;
			return temp;
		}
	}
}
