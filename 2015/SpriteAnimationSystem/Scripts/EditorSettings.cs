/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/
using System;

namespace TestTool.SpriteAnimationSystem
{
	[Serializable]
	public class EditorSettings
	{
		public bool PlayOnAwake = false;
		public bool LoopPlayAwake = false;
		public int FrameIndex = 0;
		public int IndexSelectAnimation;
		public bool FlipX = false;
		public bool FlipY = false;
	}
}