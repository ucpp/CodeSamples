/*
 * Simple atlas packer 2013-2014
 * Author: M. Yaroma
 * All rights reserved.
 */

namespace TestTool.AtlasPacker
{
	public class Borders
	{
		public int Left { get; set; }
		public int Right { get; set; }
		public int Top { get; set; }
		public int Bottom { get; set; }

		public int RepeatSize
		{
			get
			{
				return _repeatSize;
			}
			set
			{
				_repeatSize = value;
			}
		}

		public bool IsRepeat
		{
			get
			{
				return _isRepeat;
			}
			set
			{
				_isRepeat = value;
				if (_isRepeat)
					Left = Right = Top = Bottom = 1 + RepeatSize;
			}
		}

		public int Width
		{
			get
			{
				return Left + Right;
			}
		}

		public int Height
		{
			get
			{
				return Top + Bottom;
			}
		}

		private bool _isRepeat = false;
		private int _repeatSize = 1;

		public Borders() { }

		public Borders(int left, int right, int top, int bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}
	}
}