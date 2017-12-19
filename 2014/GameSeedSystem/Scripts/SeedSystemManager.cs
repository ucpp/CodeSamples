/*
 * Game Seed System 2014-2015
 * Author: M. Yaroma
 * All rights reserved.
*/

using UnityEngine;

namespace TestTool.GameSeedSystem
{
	public class SeedSystemManager : Singleton<SeedSystemManager>
	{
		public Randomizer Random
		{
			private set
			{
				_random = value;
			}
			get
			{
				if (_random == null)
					StartWithNewSeed(SeedGameVersion);
				return _random;
			}
		}

		public SeedEncrypter Encrypter
		{
			private set
			{
				_encrypter = value;
			}
			get
			{
				return _encrypter;
			}
		}

		public string Seed
		{
			private set;
			get;
		}

		public uint SeedGameVersion
		{
			private set;
			get;
		}

		private const int FullLengthSeed = 8;
		private Randomizer _random = null;
		private SeedEncrypter _encrypter = null;

		public SeedSystemManager()
		{
			Debug.Log("Start seed system manager " + Version.Value);
		}

		// game version 0-9999
		public void StartWithNewSeed(uint gameVersion)
		{
			gameVersion = (uint)Mathf.Clamp(gameVersion, 0, 9999);
			Encrypter = new SeedEncrypter(FullLengthSeed);
			Seed = Encrypter.GetNewSeed(gameVersion);
			Random = new Randomizer(Encrypter.ConvertSeedToInt(Seed));
			SeedGameVersion = gameVersion;
		}

		public void StartWithSeed(string seed)
		{
			if (string.IsNullOrEmpty(seed))
			{
				Debug.Log("Error: start with seed - is null or empty!");
				return;
			}
			Encrypter = new SeedEncrypter(FullLengthSeed);
			Seed = seed;
			Random = new Randomizer(Encrypter.ConvertSeedToInt(seed));
			SeedGameVersion = Encrypter.GetVersion(seed);
		}
	}
}
