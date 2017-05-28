// Copyright 2011-2017 Melvyn Laïly
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
		public static float[,] Identity { get; }
		/// <summary>
		/// simple colors transformations
		/// </summary>
		public static float[,] Negative { get; }
		public static float[,] GrayScale { get; }
		public static float[,] Sepia { get; }
		public static float[,] Red { get; }
		public static float[,] HueShift180 { get; }

		public static float[,] NegativeGrayScale { get; }
		public static float[,] NegativeSepia { get; }
		public static float[,] NegativeRed { get; }

		/// <summary>
		/// theoretical optimal transfomation (but ugly desaturated pure colors due to "overflows"...)
		/// Many thanks to Tom MacLeod who gave me the idea for these inversion modes
		/// </summary>
		public static float[,] NegativeHueShift180 { get; }
		/// <summary>
		/// high saturation, good pure colors
		/// </summary>
		public static float[,] NegativeHueShift180Variation1 { get; }
		/// <summary>
		/// overall desaturated, yellows and blue plain bad. actually relaxing and very usable
		/// </summary>
		public static float[,] NegativeHueShift180Variation2 { get; }
		/// <summary>
		/// high saturation. yellows and blues plain bad. actually quite readable
		/// </summary>
		public static float[,] NegativeHueShift180Variation3 { get; }
		/// <summary>
		/// not so readable, good colors (CMY colors a bit desaturated, still more saturated than normal)
		/// </summary>
		public static float[,] NegativeHueShift180Variation4 { get; }

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
			GrayScale = new float[,] {
				{  0.3f,  0.3f,  0.3f,  0.0f,  0.0f },
				{  0.6f,  0.6f,  0.6f,  0.0f,  0.0f },
				{  0.1f,  0.1f,  0.1f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			NegativeGrayScale = Multiply(Negative, GrayScale);
			Red = new float[,] {
				{  1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
				{  0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
			};
			Red = Multiply(GrayScale, Red);
			NegativeRed = Multiply(NegativeGrayScale, Red);
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
				// most simple working method for shifting hue 180deg.
				{  1.0f, -1.0f, -1.0f, 0.0f, 0.0f },
				{ -1.0f,  1.0f, -1.0f, 0.0f, 0.0f },
				{ -1.0f, -1.0f,  1.0f, 0.0f, 0.0f },
				{  0.0f,  0.0f,  0.0f, 1.0f, 0.0f },
				{  1.0f,  1.0f,  1.0f, 0.0f, 1.0f }
			};
			NegativeHueShift180Variation2 = new float[,] {
				// generated with QColorMatrix http://www.codeguru.com/Cpp/G-M/gdi/gdi/article.php/c3667
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

		private static string MatrixToString(float[,] matrix)
		{
			int maxDecimal = 0;
			foreach (var item in matrix)
			{
				string toString = item.ToString("0.#######", System.Globalization.NumberFormatInfo.InvariantInfo);
				int indexOfDot = toString.IndexOf('.');
				int currentMax = indexOfDot >= 0 ? toString.Length - indexOfDot - 1 : 0;
				if (currentMax > maxDecimal)
				{
					maxDecimal = currentMax;
				}
			}
			string format = "0." + new string('0', maxDecimal);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				sb.Append("{ ");
				for (int j = 0; j < matrix.GetLength(1); j++)
				{
					if (matrix[i, j] >= 0)
					{
						// align negative signs
						sb.Append(" ");
					}
					sb.Append(matrix[i, j].ToString(format, System.Globalization.NumberFormatInfo.InvariantInfo));
					if (j < matrix.GetLength(1) - 1)
					{
						sb.Append(", ");
					}
				}
				sb.Append(" }\n");
			}
			return sb.ToString();
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

		public static void ChangeColorEffect(float[,] matrix)
		{
			ColorEffect colorEffect = new ColorEffect(matrix);
			if (!NativeMethods.SetMagnificationDesktopColorEffect(ref colorEffect))
			{
				var inner = new Exception("SetMagnificationDesktopColorEffect()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				throw new CannotChangeColorEffectException("An error occured while applying a color effect. Another application using the same API might be interfering...", inner);
			}
		}

		public static void InterpolateColorEffect(float[,] fromMatrix, float[,] toMatrix, int timeBetweenFrames = 15)
		{
			List<float[,]> transitions = Interpolate(fromMatrix, toMatrix);
			foreach (float[,] item in transitions)
			{
				ChangeColorEffect(item);
				System.Threading.Thread.Sleep(timeBetweenFrames);
				System.Windows.Forms.Application.DoEvents();
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

		public static List<float[,]> Interpolate(float[,] A, float[,] B)
		{
			const int STEPS = 10;
			const int SIZE = 5;

			if (A.GetLength(0) != SIZE ||
				A.GetLength(1) != SIZE ||
				B.GetLength(0) != SIZE ||
				B.GetLength(1) != SIZE)
			{
				throw new ArgumentException();
			}

			List<float[,]> result = new List<float[,]>(STEPS);

			for (int i = 0; i < STEPS; i++)
			{
				result.Add(new float[SIZE, SIZE]);

				for (int x = 0; x < SIZE; x++)
				{
					for (int y = 0; y < SIZE; y++)
					{
						// f(x)=ya+(x-xa)*(yb-ya)/(xb-xa)
						// calculate 10 steps, from 1 to 10 (we don't need 0, as we start from there)
						result[i][x, y] = A[x, y] + (i + 1/*-0*/) * (B[x, y] - A[x, y]) / (STEPS/*-0*/);
					}
				}
			}

			return result;
		}
	}


	[Serializable]
	public class CannotChangeColorEffectException : Exception
	{
		public CannotChangeColorEffectException() { }
		public CannotChangeColorEffectException(string message) : base(message) { }
		public CannotChangeColorEffectException(string message, Exception inner) : base(message, inner) { }
		protected CannotChangeColorEffectException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}
