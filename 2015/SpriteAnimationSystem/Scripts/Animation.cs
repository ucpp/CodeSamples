/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using System;
using System.Globalization;
using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	/// <summary>
	/// Animation, stores a set of frames, name and current frame
	/// </summary>
	[Serializable]
	public class Animation
	{
		public string name;

		/// <summary>
		/// The animation data.
		/// </summary>
		public TextAsset timelineData;

		/// <summary>
		/// The frames array
		/// </summary>
		public Frame[] frames;

		private SpritesPool spritesPool = null;

		public void Initialize(SpritesPool pool = null)
		{
			if (pool != null)
				this.spritesPool = pool;
			if (timelineData != null && this.spritesPool != null)
				InitializeAnimation();
		}

		private void InitializeAnimation()
		{
			string[] separators = new string[] { "\n", "\r\n", ((char)13).ToString() };
			string[] framesArray = timelineData.text.Split(separators, StringSplitOptions.None);
			frames = new Frame[framesArray.Length];
			for (int i = 0; i < framesArray.Length; i++)
			{
				frames[i] = new Frame();
				InitializeFrame(frames[i], framesArray[i]);
			}
		}

		private void InitializeFrame(Frame frame, string data)
		{
			string[] frameData = data.Split(' ');
			frame.Image = this.spritesPool.Get(frameData[0]);
			frame.x = float.Parse(frameData[1], CultureInfo.InvariantCulture);
			frame.y = float.Parse(frameData[2], CultureInfo.InvariantCulture);
			frame.TimeLife = float.Parse(frameData[3], CultureInfo.InvariantCulture);
			if (frameData.Length > 4)
				InitializeAttachment(frame, frameData);
		}

		private void InitializeAttachment(Frame frame, string[] frameData)
		{
			int count = int.Parse(frameData[4]);
			frame.Attachments = new Attachment[count];
			for (int i = 0; i < count; i++)
			{
				frame.Attachments[i] = new Attachment();
				int k = 5 + i * 3;
				frame.Attachments[i].Name = frameData[k + 0];
				frame.Attachments[i].x = float.Parse(frameData[k + 1], CultureInfo.InvariantCulture);
				frame.Attachments[i].y = float.Parse(frameData[k + 2], CultureInfo.InvariantCulture);
			}
		}

		public void CopyEvents(Animation copy)
		{
			if (copy != null && copy.name == this.name)
			{
				int count = Mathf.Min(copy.frames.Length, this.frames.Length);
				for (int i = 0; i < count; i++)
					this.frames[i].EventName = copy.frames[i].EventName;
			}
		}
	}
}