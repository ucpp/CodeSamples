/*
 * Game Seed System 2014-2015
 * Author: M. Yaroma
 * All rights reserved.
*/

using System.Collections.Generic;

namespace TestTool.GameSeedSystem
{
	public class VersionEncrypter
	{
		private string _symbols;
		private string _value;
		private uint _notation;
		private uint _offset;

		public VersionEncrypter(uint offset, string symbols)
		{
			_offset = offset;
			_symbols = symbols;
			_notation = (uint)symbols.Length;
		}

		public string GetSeedVersionPart(uint version)
		{
			_value = "";
			uint number = version + _offset;
			List<char> reverseArray = new List<char>();
			while (number > 0)
			{
				reverseArray.Add(_symbols[(int)number % (int)_notation]);
				number /= _notation;
			}
			for (int i = reverseArray.Count - 1; i >= 0; i--)
				_value += reverseArray[i];
			return _value;
		}

		public uint GetVersionBySeed(string value)
		{
			_value = value;
			uint number = 0;
			for (int i = 0; i < value.Length; i++)
				number = number * _notation + (uint)_symbols.IndexOf(value[i]);
			return number - _offset;
		}
	}
}