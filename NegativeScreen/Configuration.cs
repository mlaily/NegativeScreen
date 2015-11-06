//Copyright 2011-2014 Melvyn Laily
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
using System.Linq;

namespace NegativeScreen
{
	class Configuration : IConfigurable
	{
		#region Default configuration

		public const string DefaultConfigurationFileName = "negativescreen.conf";
		public const string DefaultConfiguration =
@"# comments: if the character '#' is found, the rest of the line is ignored.
# quotes: allow to place a '#' inside a value. they do not appear in the final result.
# i.e. blah=""hello #1!"" will create a parameter blah with a value of: hello #1!
# To place a quotation mark inside quotes, double it.
# i.e. blah=""hello"""""" will create a parameter blah with a value of: hello""

#Predefined keys
# You can use the following modifiers: alt, ctrl, shift, win
# and a key from http://msdn.microsoft.com/en-us/library/system.windows.forms.keys%28v=vs.71%29.aspx
# You can either use its textual representation, or its numerical value.
# WARNING: if the key is not valid, the program will probably crash...

Toggle=win+alt+N
Exit=win+alt+H

SmoothTransitions=true
SmoothToggles=true

# in miliseconds
MainLoopRefreshTime=100

InitialColorEffect=""Smart Inversion""

ActiveOnStartup=true

ShowAeroWarning=true

#Matrices definition
# The left hand is used as a description, while the right hand is broken down in two parts:
# - the hot key combination, followed by a new line, (this part is optional)
# - the matrix definition, with or without new lines between rows.
# The matrices must have 5 rows and 5 columns,
# each line between curved brackets,
# the elements separated by commas.
# The decimal separator is a dot.

Simple Inversion=win+alt+F1
{ -1,  0,  0,  0,  0 }
{  0, -1,  0,  0,  0 }
{  0,  0, -1,  0,  0 }
{  0,  0,  0,  1,  0 }
{  1,  1,  1,  0,  1 }

# Theoretical optimal transfomation (but ugly desaturated pure colors due to ""overflows""...)
# Many thanks to Tom MacLeod who gave me the idea for these inversion modes.
Smart Inversion=win+alt+F2
{  0.3333333, -0.6666667, -0.6666667,  0.0000000,  0.0000000 }
{ -0.6666667,  0.3333333, -0.6666667,  0.0000000,  0.0000000 }
{ -0.6666667, -0.6666667,  0.3333333,  0.0000000,  0.0000000 }
{  0.0000000,  0.0000000,  0.0000000,  1.0000000,  0.0000000 }
{  1.0000000,  1.0000000,  1.0000000,  0.0000000,  1.0000000 }

# High saturation, good pure colors.
Smart Inversion Alt 1=win+alt+F3
{  1, -1, -1,  0,  0 }
{ -1,  1, -1,  0,  0 }
{ -1, -1,  1,  0,  0 }
{  0,  0,  0,  1,  0 }
{  1,  1,  1,  0,  1 }

# Overall desaturated, yellows and blue plain bad. actually relaxing and very usable.
Smart Inversion Alt 2=win+alt+F4
{  0.39, -0.62, -0.62,  0.00,  0.00 }
{ -1.21, -0.22, -1.22,  0.00,  0.00 }
{ -0.16, -0.16,  0.84,  0.00,  0.00 }
{  0.00,  0.00,  0.00,  1.00,  0.00 }
{  1.00,  1.00,  1.00,  0.00,  1.00 }

# High saturation. yellows and blues plain bad. actually quite readable.
Smart Inversion Alt 3=win+alt+F5
{  1.0895080, -0.9326327, -0.9326330,  0.0000000,  0.0000000 }
{ -1.8177180,  0.1683074, -1.8416920,  0.0000000,  0.0000000 }
{ -0.2445895, -0.2478156,  1.7621850,  0.0000000,  0.0000000 }
{  0.0000000,  0.0000000,  0.0000000,  1.0000000,  0.0000000 }
{  1.0000000,  1.0000000,  1.0000000,  0.0000000,  1.0000000 }

# Not so readable, good colors (CMY colors a bit desaturated, still more saturated than normal).
Smart Inversion Alt 4=win+alt+F6
{  0.50, -0.78, -0.78,  0.00,  0.00 }
{ -0.56,  0.72, -0.56,  0.00,  0.00 }
{ -0.94, -0.94,  0.34,  0.00,  0.00 }
{  0.00,  0.00,  0.00,  1.00,  0.00 }
{  1.00,  1.00,  1.00,  0.00,  1.00 }

Negative Sepia=win+alt+F7
{ -0.393, -0.349, -0.272,  0.000,  0.000 }
{ -0.769, -0.686, -0.534,  0.000,  0.000 }
{ -0.189, -0.168, -0.131,  0.000,  0.000 }
{  0.000,  0.000,  0.000,  1.000,  0.000 }
{  1.351,  1.203,  0.937,  0.000,  1.000 }

Negative Grayscale=win+alt+F8
{ -0.3, -0.3, -0.3,  0.0,  0.0 }
{ -0.6, -0.6, -0.6,  0.0,  0.0 }
{ -0.1, -0.1, -0.1,  0.0,  0.0 }
{  0.0,  0.0,  0.0,  1.0,  0.0 }
{  1.0,  1.0,  1.0,  0.0,  1.0 }

#Grayscaled
Negative Red=win+alt+F9
{ -0.3,  0.0,  0.0,  0.0,  0.0 }
{ -0.6,  0.0,  0.0,  0.0,  0.0 }
{ -0.1,  0.0,  0.0,  0.0,  0.0 }
{  0.0,  0.0,  0.0,  1.0,  0.0 }
{  1.0,  0.0,  0.0,  0.0,  1.0 }

#Grayscaled
Red=win+alt+F10
{  0.3,  0.0,  0.0,  0.0,  0.0 }
{  0.6,  0.0,  0.0,  0.0,  0.0 }
{  0.1,  0.0,  0.0,  0.0,  0.0 }
{  0.0,  0.0,  0.0,  1.0,  0.0 }
{  0.0,  0.0,  0.0,  0.0,  1.0 }

Grayscale=win+alt+F11
{ 0.3,  0.3,  0.3,  0.0,  0.0 }
{ 0.6,  0.6,  0.6,  0.0,  0.0 }
{ 0.1,  0.1,  0.1,  0.0,  0.0 }
{ 0.0,  0.0,  0.0,  1.0,  0.0 }
{ 0.0,  0.0,  0.0,  0.0,  1.0 }
";
		#endregion

		private static Configuration _current;
		public static Configuration Current
		{
			get
			{
				Initialize();
				return _current;
			}
		}

		public static void Initialize()
		{
			if (_current == null)
			{
				_current = new Configuration();
			}
		}

		private Configuration()
		{
			ColorEffects = new List<KeyValuePair<HotKey, ScreenColorEffect>>();

			string configFileContent;
			try
			{
				configFileContent = System.IO.File.ReadAllText(DefaultConfigurationFileName);
			}
			catch (Exception)
			{
				configFileContent = DefaultConfiguration;
			}
			Parser.AssignConfiguration(configFileContent, this, new HotKeyParser(), new MatrixParser());
			if (!string.IsNullOrWhiteSpace(InitialColorEffectName))
			{
				try
				{
					this.InitialColorEffect = this.ColorEffects.Single(x =>
						x.Value.Description.ToLowerInvariant() == InitialColorEffectName.ToLowerInvariant()).Value;
				}
				catch (Exception)
				{
					//probably not ideal
					this.InitialColorEffect = new ScreenColorEffect(BuiltinMatrices.Negative, "Negative");
				}

			}
		}

		[CorrespondTo("Toggle", CustomParameter = HotKey.ToggleKeyId)]
		public HotKey ToggleKey { get; protected set; }

		[CorrespondTo("Exit", CustomParameter = HotKey.ExitKeyId)]
		public HotKey ExitKey { get; protected set; }

		[CorrespondTo("SmoothTransitions")]
		public bool SmoothTransitions { get; protected set; }

		[CorrespondTo("SmoothToggles")]
		public bool SmoothToggles { get; protected set; }

		[CorrespondTo("MainLoopRefreshTime", CustomParameter = 100)]
		public int MainLoopRefreshTime { get; protected set; }

		[CorrespondTo("ActiveOnStartup", CustomParameter = true)]
		public bool ActiveOnStartup { get; protected set; }

		[CorrespondTo("ShowAeroWarning", CustomParameter = true)]
		public bool ShowAeroWarning { get; protected set; }

		[CorrespondTo("InitialColorEffect")]
		public string InitialColorEffectName { get; protected set; }

		public ScreenColorEffect InitialColorEffect { get; protected set; }

		public List<KeyValuePair<HotKey, ScreenColorEffect>> ColorEffects { get; protected set; }

		public void HandleDynamicKey(string key, string value)
		{
			//value is already trimmed
			if (value.StartsWith("{"))
			{
				//no hotkey
				this.ColorEffects.Add(new KeyValuePair<HotKey, ScreenColorEffect>(
					HotKey.Empty,
					new ScreenColorEffect(MatrixParser.StaticParseMatrix(value), key)));
			}
			else
			{
				//first part is the hotkey, second part is the matrix
				var splitted = value.Split(new char[] { '\n' }, 2);
				if (splitted.Length < 2)
				{
					throw new Exception(string.Format(
						"The value assigned to \"{0}\" is unexpected. The hotkey must be separated from the matrix by a new line.",
						key));
				}
				this.ColorEffects.Add(new KeyValuePair<HotKey, ScreenColorEffect>(
					HotKeyParser.StaticParse(splitted[0]),
					new ScreenColorEffect(MatrixParser.StaticParseMatrix(splitted[1]), key)));
			}
		}
	}

	class HotKeyParser : ICustomParser
	{

		public Type ReturnType => typeof(HotKey);

		public object Parse(string rawValue, object customParameter)
		{
			int defaultId = -1;
			if (customParameter is int)
			{
				defaultId = (int)customParameter;
			}
			return StaticParse(rawValue, defaultId);
		}

		public static HotKey StaticParse(string rawValue, int defaultId = -1)
		{
			KeyModifiers modifiers = KeyModifiers.NONE;
			Keys key = Keys.None;
			string trimmed = rawValue.Trim();
			var splitted = trimmed.Split('+');
			foreach (var item in splitted)
			{
				//modifier
				switch (item.ToLowerInvariant())
				{
					case "alt":
						modifiers |= KeyModifiers.MOD_ALT;
						break;
					case "ctrl":
						modifiers |= KeyModifiers.MOD_CONTROL;
						break;
					case "shift":
						modifiers |= KeyModifiers.MOD_SHIFT;
						break;
					case "win":
						modifiers |= KeyModifiers.MOD_WIN;
						break;
					default:
						//key
						if (!Enum.TryParse(item, true, out key))
						{
							//try to parse numeric value
							int numericValue;
							if (int.TryParse(item, out numericValue))
							{
								if (Enum.IsDefined(typeof(Keys), numericValue))
								{
									key = (Keys)numericValue;
								}
							}
						}
						break;
				}

			}
			return new HotKey(modifiers, key, defaultId);
		}
	}

	class MatrixParser : ICustomParser
	{

		public Type ReturnType => typeof(float[,]);

		public object Parse(string rawValue, object customParameter)
			=> StaticParseMatrix(rawValue);

		public static float[,] StaticParseMatrix(string rawValue)
		{
			float[,] matrix = new float[5, 5];
			var rows = System.Text.RegularExpressions.Regex.Matches(rawValue, @"{(?<row>.*?)}",
				System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
			if (rows.Count != 5)
			{
				throw new Exception("The matrices must have 5 rows.");
			}
			for (int x = 0; x < rows.Count; x++)
			{
				var row = rows[x];
				var columnSplit = row.Groups["row"].Value.Split(',');
				if (columnSplit.Length != 5)
				{
					throw new Exception("The matrices must have 5 columns.");
				}
				for (int y = 0; y < matrix.GetLength(1); y++)
				{
					float value;
					if (!float.TryParse(columnSplit[y],
						System.Globalization.NumberStyles.Float,
						System.Globalization.NumberFormatInfo.InvariantInfo,
						out value))
					{
						throw new Exception(string.Format("Unable to parse \"{0}\" to a float.", columnSplit[y]));
					}
					matrix[x, y] = value;
				}
			}
			return matrix;
		}
	}

	struct HotKey
	{
		public const int ToggleKeyId = 42;
		public const int ExitKeyId = 43;

		private static int CurrentId = 100;

		public static readonly HotKey Empty;

		public KeyModifiers Modifiers { get; private set; }
		public Keys Key { get; private set; }
		public int Id { get; private set; }

		static HotKey()
		{
			Empty = new HotKey()
			{
				Id = 0,
				Key = Keys.None,
				Modifiers = KeyModifiers.NONE
			};
		}

		public HotKey(KeyModifiers modifiers, Keys key, int id = -1)
			: this()
		{
			Modifiers = modifiers;
			//65535
			Key = key & Keys.KeyCode;
			if (id == -1)
			{
				Id = CurrentId;
				CurrentId++;
			}
			else
			{
				Id = id;
			}
		}

		public override int GetHashCode()
			=> (int)Key | (int)Modifiers << 16 | Id << 20;

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is HotKey)
			{
				return obj.GetHashCode() == this.GetHashCode();
			}
			else
			{
				return false;
			}
		}

		public static bool operator ==(HotKey a, HotKey b)
			=> a.GetHashCode() == b.GetHashCode();

		public static bool operator !=(HotKey a, HotKey b)
			=> a.GetHashCode() != b.GetHashCode();

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (Modifiers.HasFlag(KeyModifiers.MOD_ALT))
			{
				sb.Append("Alt+");
			}
			if (Modifiers.HasFlag(KeyModifiers.MOD_CONTROL))
			{
				sb.Append("Ctrl+");
			}
			if (Modifiers.HasFlag(KeyModifiers.MOD_SHIFT))
			{
				sb.Append("Shift+");
			}
			if (Modifiers.HasFlag(KeyModifiers.MOD_WIN))
			{
				sb.Append("Win+");
			}
			sb.Append(Enum.GetName(typeof(Keys), Key) ?? ((int)Key).ToString());
			return sb.ToString();
		}
	}

	struct ScreenColorEffect
	{
		public float[,] Matrix { get; }
		public string Description { get; }

		public ScreenColorEffect(float[,] matrix, string description)
			: this()
		{
			Matrix = matrix;
			Description = description;
		}

	}
}
