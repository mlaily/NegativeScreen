using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NegativeScreen
{
	/// <summary>
	/// Represents the class storing the parsed key-value pairs from the configuration file.
	/// </summary>
	public interface IConfigurable
	{
		/// <summary>
		/// This method is automatically called when a key from the configuration file
		/// does not match any property marked with a CorrespondToAttribute.
		/// This allows to handle dynamically declared keys in the configuration file.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void HandleDynamicKey(string key, string value);
	}

	/// <summary>
	/// Represents a custom parser for a given type.
	/// This allows a better control over the values' parsing.
	/// </summary>
	public interface ICustomParser
	{
		/// <summary>
		/// Type this custom parser handles.
		/// </summary>
		Type ReturnType { get; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawValue">
		/// Raw value to parse, trimmed from any whitespace.
		/// </param>
		/// <param name="customParameter">
		/// A custom parameter, from the CorrespondToAttribute.
		/// Its behaviour is left to the implementer's discretion.
		/// </param>
		/// <returns></returns>
		object Parse(string rawValue, object customParameter);
	}

	/// <summary>
	/// Mark a property as the storage for a matching key in the configuration file.
	/// All keys are case insensitive.
	/// A custom parameter can be provided, which will be passed to the parser handling this property type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class CorrespondToAttribute : Attribute
	{
		public string Key { get; private set; }

		public CorrespondToAttribute(string key)
		{
			this.Key = key.ToLowerInvariant();
		}

		public object CustomParameter { get; set; }
	}

	/// <summary>
	/// Static class allowing to parse configuration files.
	/// </summary>
	public static class Parser
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content">Raw configuration file content.</param>
		/// <param name="configuration">The configuration object, implementing IConfigurable.</param>
		/// <param name="customParsers">Optional array of parser allowing to parse any type the way you want.</param>
		public static void AssignConfiguration(string content, IConfigurable configuration, params ICustomParser[] customParsers)
		{
			Dictionary<string, string> rawConfiguration = ParseConfiguration(content);
			var configurableProperties =
				(from p in configuration.GetType().GetProperties()
				 let attr = p.GetCustomAttributes(typeof(CorrespondToAttribute), true)
				 where attr.Length == 1
				 select new { Property = p, Attribute = attr.First() as CorrespondToAttribute })
				.ToList();
			foreach (var item in rawConfiguration)
			{
				string key = item.Key.ToLowerInvariant();
				var correspondingProps = configurableProperties.Where(x => x.Attribute.Key == key);
				System.Reflection.PropertyInfo correspondingProp = null;
				CorrespondToAttribute correspondingAttribute = null;
				if (correspondingProps.Any())
				{
					try
					{
						//try to find a matching property for this key
						var single = correspondingProps.Single();
						correspondingProp = single.Property;
						correspondingAttribute = single.Attribute;
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("The key \"{0}\" was found multiple times!", key), ex);
					}
					//parse value
					var appropriateParser = customParsers.FirstOrDefault(x => x.ReturnType == correspondingProp.PropertyType);
					if (appropriateParser != null)
					{
						object parsed = appropriateParser.Parse(item.Value, correspondingAttribute.CustomParameter);
						correspondingProp.SetValue(configuration, parsed, null);
					}
					//default parsers
					else if (correspondingProp.PropertyType == typeof(string))
					{
						correspondingProp.SetValue(configuration, item.Value, null);
					}
					else if (correspondingProp.PropertyType == typeof(bool))
					{
						correspondingProp.SetValue(configuration, ParseBool(item.Value), null);
					}
					//TODO: default parser for other simple types (int, float...)
					else
					{
						throw new Exception(string.Format("Could not find a parser for type \"{0}\"!", correspondingProp.PropertyType));
					}
				}
				else
				{
					//no corresponding assignable property
					configuration.HandleDynamicKey(item.Key, item.Value);
				}
			}
		}

		/// <remarks>
		/// comments: if the character '#' is found, the rest of the line is ignored
		/// quotes: allow to place a '#' inside a value. they do not appear in the final result
		/// i.e. blah="hello #1!" will create a parameter blah with a value of: hello #1!
		/// to place a quotation mark inside quotes, double it
		/// i.e. blah="hello""" will create a parameter blah with a value of: hello "
		/// 
		/// The keys and values are always trimmed from any white space.
		/// </remarks>
		private static Dictionary<string, string> ParseConfiguration(string content)
		{
			var cleanedContent = content.Replace("\r\n", "\n").Replace('\r', '\n');
			int i = 0;
			Dictionary<string, string> parsed = new Dictionary<string, string>();
			StringBuilder left = new StringBuilder();
			StringBuilder right = new StringBuilder();
			bool insideQuotes = false;
			while (i < cleanedContent.Length)
			{
				char read = cleanedContent[i];
				switch (read)
				{
					case '"':
						if (insideQuotes)
						{
							//handle double quotes inside quotation marks
							if (i + 1 < cleanedContent.Length && cleanedContent[i + 1] == '"')
							{
								right.Append('"');
								i++;
								break;
							}
						}
						insideQuotes = !insideQuotes;
						break;
					case '#':
						if (insideQuotes)
						{
							goto default;
						}
						//ignore line
						do
						{
							i++;
						} while (i < cleanedContent.Length && cleanedContent[i] != '\n');
						//include the new line
						i--;
						break;
					case '=':
						if (insideQuotes)
						{
							goto default;
						}
						int indexOfLastLine = right.Length - 1;
						while (indexOfLastLine > 0 && right[indexOfLastLine] != '\n')
						{
							indexOfLastLine--;
						}
						if (left.Length > 0)
						{
							string key = left.ToString().Trim();
							//the last declaration overwrite any existing one
							if (parsed.ContainsKey(key))
							{
								parsed[key] = right.ToString(0, indexOfLastLine).Trim();
							}
							else
							{
								parsed.Add(key, right.ToString(0, indexOfLastLine).Trim());
							}
						}
						left = right.Remove(0, indexOfLastLine > 0 ? indexOfLastLine + 1 : 0);
						right = new StringBuilder();
						break;
					default:
						right.Append(read);
						break;
				}
				i++;
			}
			parsed.Add(left.ToString().Trim(), right.ToString().Trim());
			return parsed;
		}

		private static bool ParseBool(string rawValue)
		{
			string trimmed = rawValue.Trim();
			switch (trimmed.ToLowerInvariant())
			{
				case "true":
					return true;
				case "false":
					return false;
				default:
					throw new Exception("Could not parse a boolean value!");
			}
		}

	}
}
