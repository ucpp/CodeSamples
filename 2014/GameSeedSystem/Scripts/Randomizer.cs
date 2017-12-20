/*
 * Game Seed System 2014-2015
 * Author: M. Yaroma
 * All rights reserved.
*/

using UnityEngine;

namespace TestTool.GameSeedSystem
{
	// George Marsaglia 2003
	public class Randomizer
	{
		public string Seed
		{
			get;
			private set;
		}

		private int _random;
		
		public Randomizer(int key, string seed)
		{
			_random = key;
			Seed = seed;
		}

		public Randomizer(int key)
		{
			_random = key;
		}

		public int Range(int min, int max)
		{
			return min + Get(max - min);
		}

		public int GetKey()
		{
			return _random;
		}

		public void SetKey(int key)
		{
			_random = key;
		}

		private int Get(int max = 100)
		{
			_random ^= (_random << 21);
			_random ^= (_random >> 35);
			_random ^= (_random << 4);
			int result = 0;
			if (max != 0)
				result = _random % max;
			return Mathf.Abs(result);
		}
	}
}