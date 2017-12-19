/*
 * Game Seed System 2014-2015
 * Author: M. Yaroma
 * All rights reserved.
*/

using UnityEngine;

namespace TestTool.GameSeedSystem
{
	public class SeedSystemTest : MonoBehaviour
	{
		private void Awake()
		{
			TestSeedSystem();
		}

		/*
		 * Create new seed with project version 199
		 * Print seed value and project version
		 * save seed value
		 * Get random number and print it
		 * Start system with saved seed value
		 * Get random number and check it value
		 */
		private void TestSeedSystem()
		{
			uint testProductVersion = 199;
			SeedSystemManager.Instance.StartWithNewSeed(testProductVersion);
			PrintInfo();
			string seed = SeedSystemManager.Instance.Seed;
			SeedSystemManager.Instance.StartWithSeed(seed);
			PrintInfo();
		}

		// print random number, seed value, project version
		private void PrintInfo()
		{
			SeedSystemManager seedMng = SeedSystemManager.Instance;
			Debug.Log(seedMng.Random.Range(0, 10));
			Debug.LogFormat("{0} {1}", seedMng.Seed, seedMng.SeedGameVersion);
		}
	}
}
