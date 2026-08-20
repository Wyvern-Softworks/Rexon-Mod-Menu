// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Interface.SimpleJsonParser
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rexon_Menu.Interface;

internal sealed class SimpleJsonParser
{
	private readonly string _json;
	private int _position;

	private SimpleJsonParser(string json)
	{
		_json = json;
	}

	public static object Parse(string json)
	{
		if (json == null)
		{
			return null;
		}

		try
		{
			SimpleJsonParser parser = new SimpleJsonParser(json);
			object value = parser.ReadValue();
			parser.SkipWhitespace();
			return parser.AtEnd ? value : null;
		}
		catch (FormatException)
		{
			return null;
		}
		catch (OverflowException)
		{
			return null;
		}
	}

	private bool AtEnd => _position >= _json.Length;

	private char Current => AtEnd ? '\0' : _json[_position];

	private object ReadValue()
	{
		SkipWhitespace();
		if (AtEnd)
		{
			throw InvalidJson();
		}

		switch (Current)
		{
		case '{':
			return ReadObject();
		case '[':
			return ReadArray();
		case '"':
			return ReadString();
		case 't':
			ReadLiteral("true");
			return true;
		case 'f':
			ReadLiteral("false");
			return false;
		case 'n':
			ReadLiteral("null");
			return null;
		default:
			return ReadNumber();
		}
	}

	private Dictionary<string, object> ReadObject()
	{
		Dictionary<string, object> result = new Dictionary<string, object>();
		Expect('{');
		SkipWhitespace();
		if (Consume('}'))
		{
			return result;
		}

		while (true)
		{
			SkipWhitespace();
			if (Current != '"')
			{
				throw InvalidJson();
			}

			string key = ReadString();
			SkipWhitespace();
			Expect(':');
			result[key] = ReadValue();
			SkipWhitespace();
			if (Consume('}'))
			{
				return result;
			}

			Expect(',');
		}
	}

	private List<object> ReadArray()
	{
		List<object> result = new List<object>();
		Expect('[');
		SkipWhitespace();
		if (Consume(']'))
		{
			return result;
		}

		while (true)
		{
			result.Add(ReadValue());
			SkipWhitespace();
			if (Consume(']'))
			{
				return result;
			}

			Expect(',');
		}
	}

	private string ReadString()
	{
		Expect('"');
		StringBuilder result = new StringBuilder();
		while (!AtEnd)
		{
			char character = _json[_position++];
			if (character == '"')
			{
				return result.ToString();
			}

			if (character < ' ')
			{
				throw InvalidJson();
			}

			if (character != '\\')
			{
				result.Append(character);
				continue;
			}

			if (AtEnd)
			{
				throw InvalidJson();
			}

			switch (_json[_position++])
			{
			case '"':
				result.Append('"');
				break;
			case '\\':
				result.Append('\\');
				break;
			case '/':
				result.Append('/');
				break;
			case 'b':
				result.Append('\b');
				break;
			case 'f':
				result.Append('\f');
				break;
			case 'n':
				result.Append('\n');
				break;
			case 'r':
				result.Append('\r');
				break;
			case 't':
				result.Append('\t');
				break;
			case 'u':
				result.Append(ReadUnicodeEscape());
				break;
			default:
				throw InvalidJson();
			}
		}

		throw InvalidJson();
	}

	private char ReadUnicodeEscape()
	{
		if (_position + 4 > _json.Length)
		{
			throw InvalidJson();
		}

		int value = 0;
		for (int index = 0; index < 4; index++)
		{
			char digit = _json[_position++];
			value <<= 4;
			if (digit >= '0' && digit <= '9')
			{
				value += digit - '0';
			}
			else if (digit >= 'a' && digit <= 'f')
			{
				value += digit - 'a' + 10;
			}
			else if (digit >= 'A' && digit <= 'F')
			{
				value += digit - 'A' + 10;
			}
			else
			{
				throw InvalidJson();
			}
		}

		return (char)value;
	}

	private object ReadNumber()
	{
		int start = _position;
		Consume('-');
		if (Consume('0'))
		{
			if (!AtEnd && char.IsDigit(Current))
			{
				throw InvalidJson();
			}
		}
		else
		{
			ReadDigits(requireAtLeastOne: true);
		}

		bool isFloatingPoint = false;
		if (Consume('.'))
		{
			isFloatingPoint = true;
			ReadDigits(requireAtLeastOne: true);
		}

		if (Consume('e') || Consume('E'))
		{
			isFloatingPoint = true;
			Consume('+');
			Consume('-');
			ReadDigits(requireAtLeastOne: true);
		}

		string number = _json.Substring(start, _position - start);
		if (!isFloatingPoint && long.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out long integer))
		{
			return integer;
		}

		if (double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out double floatingPoint))
		{
			return floatingPoint;
		}

		throw InvalidJson();
	}

	private void ReadDigits(bool requireAtLeastOne)
	{
		int start = _position;
		while (!AtEnd && char.IsDigit(Current))
		{
			_position++;
		}

		if (requireAtLeastOne && start == _position)
		{
			throw InvalidJson();
		}
	}

	private void ReadLiteral(string literal)
	{
		if (_position + literal.Length > _json.Length ||
			!string.Equals(_json.Substring(_position, literal.Length), literal, StringComparison.Ordinal))
		{
			throw InvalidJson();
		}

		_position += literal.Length;
	}

	private void SkipWhitespace()
	{
		while (!AtEnd && char.IsWhiteSpace(Current))
		{
			_position++;
		}
	}

	private bool Consume(char expected)
	{
		if (Current != expected)
		{
			return false;
		}

		_position++;
		return true;
	}

	private void Expect(char expected)
	{
		if (!Consume(expected))
		{
			throw InvalidJson();
		}
	}

	private FormatException InvalidJson()
	{
		return new FormatException("Invalid JSON at character " + _position + ".");
	}
}
