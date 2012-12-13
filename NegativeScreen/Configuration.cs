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

		const string DefaultConfigurationFileName = "negativescreen.conf";
		const string DefaultConfiguration = @"
# comments: if the character '#' is found, the rest of the line is ignored.
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

InitialColorEffect=""Smart Inversion""

#Matrices definition
# The left hand is used as a description, while the right hand is broken down in two parts:
# - the hot key combination, followed by a new line,
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
			ColorEffects = new Dictionary<HotKey, ScreenColorEffect>();

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
					this.InitialColorEffect = this.ColorEffects.Values.Single(x =>
						x.Description.ToLowerInvariant() == InitialColorEffectName.ToLowerInvariant()).Matrix;
				}
				catch (Exception)
				{
					this.InitialColorEffect = BuiltinMatrices.Negative;
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

		[CorrespondTo("InitialColorEffect")]
		public string InitialColorEffectName { get; protected set; }

		public float[,] InitialColorEffect { get; protected set; }

		public Dictionary<HotKey, ScreenColorEffect> ColorEffects { get; protected set; }

		public void HandleDynamicKey(string key, string value)
		{
			//first part is the hotkey, second part is the matrix
			var splitted = value.Split(new char[] { '\n' }, 2);
			if (splitted.Length < 2)
			{
				throw new Exception(string.Format(
					"The value assigned to \"{0}\" is unexpected. The hotkey must be separated from the matrix by a new line.",
					key));
			}
			this.ColorEffects.Add(HotKeyParser.StaticParse(splitted[0]),
				new ScreenColorEffect(MatrixParser.StaticParseMatrix(splitted[1]), key));
		}
	}

	class HotKeyParser : ICustomParser
	{

		public Type ReturnType
		{
			get { return typeof(HotKey); }
		}

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
						if (!Enum.TryParse(item, out key))
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

		public Type ReturnType
		{
			get { return typeof(float[,]); }
		}

		public object Parse(string rawValue, object customParameter)
		{
			return StaticParseMatrix(rawValue);
		}

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

		public KeyModifiers Modifiers { get; private set; }
		public Keys Key { get; private set; }
		public int Id { get; private set; }

		public HotKey(KeyModifiers modifiers, Keys key, int id = -1)
			: this()
		{
			this.Modifiers = modifiers;
			this.Key = key;
			if (id == -1)
			{
				this.Id = CurrentId;
				CurrentId++;
			}
			else
			{
				this.Id = id;
			}
		}

		public override int GetHashCode()
		{
			return Modifiers.GetHashCode() ^ Key.GetHashCode();
		}
	}

	struct ScreenColorEffect
	{
		public float[,] Matrix { get; private set; }
		public string Description { get; private set; }

		public ScreenColorEffect(float[,] matrix, string description)
			: this()
		{
			this.Matrix = matrix;
			this.Description = description;
		}

	}
}
