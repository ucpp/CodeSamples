/*
 * Game Seed System 2014-2015
 * Author: M. Yaroma
 * All rights reserved.
*/

using System;

namespace TestTool.GameSeedSystem
{
	public abstract class Singleton<T> where T : class
	{
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
							_instance = (T)Activator.CreateInstance(typeof(T), true);
					}
				}
				return _instance;
			}
		}

		private static T _instance = default(T);
		private static readonly object _lock = new object();
	}
}
