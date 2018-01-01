/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using System;

namespace TestTool.SpriteAnimationSystem
{
	public static class Extensions
	{
		public static bool HasFlag<T>(this Enum type, T value)
		{
			return (((int)(object)type & (int)(object)value) == (int)(object)value);
		}
	}
}
