/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	public class AnimationTrack
	{
		public Animation CurrentAnimation = null;

		public int CurrentFrame
		{
			get
			{
				return _currentFrame;
			}
			set
			{
				_currentFrame = value;

				if (CurrentAnimation != null)
				{
					if (_currentFrame < 0)
						_currentFrame = CurrentAnimation.frames.Length - 1;

					if (_currentFrame > CurrentAnimation.frames.Length - 1)
						_currentFrame = 0;
				}
				else
				{
					_currentFrame = 0;
				}
			}

		}

		private int _currentFrame = 0;

		public static bool IsNullOrEmpty(AnimationTrack track)
		{
			return track == null || track.CurrentAnimation == null;
		}

		public void Set(Animation animation)
		{
			Clear();
			CurrentAnimation = animation;
		}

		/// <summary>
		/// Set the initial frame of the animation randomly
		/// </summary>
		public void SetRandomStartFrame()
		{
			int startFrame = Random.Range(0, CurrentAnimation.frames.Length);
			CurrentFrame = startFrame;
		}

		public void NextFrame()
		{
			CurrentFrame++;
		}

		public void PrevFrame()
		{
			CurrentFrame--;
		}

		public Frame GetCurrentFrame()
		{
			CurrentFrame = Mathf.Clamp(CurrentFrame, 0, CurrentAnimation.frames.Length - 1);
			return CurrentAnimation.frames[CurrentFrame];
		}

		public void Clear()
		{
			CurrentAnimation = null;
			CurrentFrame = 0;
		}
	}
}