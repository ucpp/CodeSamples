/*
 * Game Seed System 2014-2015
 * Author: M. Yaroma
 * All rights reserved.
*/

using System.Collections.Generic;

namespace TestTool.GameSeedSystem
{
	public class SeedEncrypter
	{
		private readonly string Symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const int OffsetMin = 1000;
		private const int OffsetMax = 2000;
		private const int SeedVersionPartLength = 3;
		private List<char> _symbolsPull = new List<char>();
		private int _length = 0;

		public SeedEncrypter(int seedLength = 8)
		{
			_length = seedLength - SeedVersionPartLength;
			for (int i = 0; i < Symbols.Length; i++)
				_symbolsPull.Add(Symbols[i]);
		}

		public string GetNewSeed(uint version)
		{
			string result = GetRandomSeed();
			Randomizer rand = new Randomizer(ConvertSeedToInt(result));
			int offset = rand.Range(OffsetMin, OffsetMax);
			VersionEncrypter ver = new VersionEncrypter((uint)offset, Symbols);
			result += ver.GetSeedVersionPart(version);
			return result;
		}

		public uint GetVersion(string seed)
		{
			Randomizer rand = new Randomizer(ConvertSeedToInt(seed));
			int offset = rand.Range(OffsetMin, OffsetMax);
			VersionEncrypter ver = new VersionEncrypter((uint)offset, Symbols);
			seed = seed.Remove(0, _length);
			return ver.GetVersionBySeed(seed);
		}

		public int ConvertSeedToInt(string seed)
		{
			int result = 0;
			for (int i = 0; i < _length; i++)
				result += (int)seed[i];
			result *= seed[0];
			return result;
		}

		private string GetRandomSeed()
		{
			_symbolsPull.Shuffle();
			string result = "";
			for (int i = 0; i < _length; i++)
				result += _symbolsPull[i];
			return result;
		}
	}
}