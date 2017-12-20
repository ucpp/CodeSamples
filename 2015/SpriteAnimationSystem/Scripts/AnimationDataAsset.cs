/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	[Serializable]
	public class AnimationDataAsset : ScriptableObject
	{
		/// <summary>
		/// all animations
		/// </summary>
		public List<Animation> Animations;

		/// <summary>
		/// Full sprites pool used by the object
		/// </summary>
		[HideInInspector]
		public SpritesPool CurrentSpritesPool;

		/// <summary>
		/// pixel to unity unit ratio
		/// </summary>
		[HideInInspector]
		public int PixelsPerUnits;

		[HideInInspector]
		public bool IsEditorMode;

		public void Clone(AnimationDataAsset copy)
		{
			Animations.Clear();
			for (int i = 0; i < copy.Animations.Count; i++)
				Animations.Add(copy.Animations[i]);

			CurrentSpritesPool = copy.CurrentSpritesPool;
			PixelsPerUnits = copy.PixelsPerUnits;
			IsEditorMode = copy.IsEditorMode;
		}
	}
}