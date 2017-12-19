/*
 * Game Seed System 2014-2015
 * Author: M. Yaroma
 * All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Threading;

namespace TestTool.GameSeedSystem
{
	public static class Extension
	{
		// Fisher–Yates shuffle algorithm
		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}

	public static class ThreadSafeRandom
	{
		[ThreadStatic]
		private static Random Local;
		public static Random ThisThreadsRandom
		{
			get
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				int randValue = unchecked(Environment.TickCount * 31 + threadId);
				return Local ?? (Local = new Random(randValue));
			}
		}
	}
}