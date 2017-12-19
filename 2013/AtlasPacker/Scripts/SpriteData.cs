/*
 * Simple atlas packer 2013-2014
 * Author: M. Yaroma
 * All rights reserved.
 */

using UnityEngine;

namespace TestTool.AtlasPacker
{
	// class for keep sprite data
	public class SpriteData
	{
		private string _name = string.Empty;
		private int _x = 0;
		private int _y = 0;
		private int _width = 0;
		private int _height = 0;

		// initialize sprite
		public SpriteData(string name, int x, int y, int width, int height)
		{
			_name = name;
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}

		public Rect GetRect()
		{
			return new Rect(_x, _y, _width, _height);
		}

		public string GetName()
		{
			return _name;
		}
	}
}