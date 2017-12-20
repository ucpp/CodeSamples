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
	public class Attachment
	{
		public string Name;
		public float x;
		public float y;

		public Vector2 position
		{
			get 
			{
				return new Vector2 (x, y);
			}
		}
	
		public Attachment(){}

		public Attachment(Attachment clone)
		{
			Name = clone.Name;
			this.x = clone.x;
			this.y = clone.y;
		}	
	}
}