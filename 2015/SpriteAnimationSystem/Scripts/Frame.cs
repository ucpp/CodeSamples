/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using System;
using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	[Serializable]
	public class Frame
	{
		/// <summary>
		/// frame position relative to the anchor in abscissa
		/// </summary>
		public float x;
		/// <summary>
		/// frame position relative to the anchor in ordinate
		/// </summary>
		public float y;
		public float TimeLife;
		public Sprite Image;
		public string EventName = String.Empty;
		/// <summary>
		/// array of bindings (points on animations to which you can bind)
		/// </summary>
		[HideInInspector]
		public Attachment[] Attachments;
	}
}