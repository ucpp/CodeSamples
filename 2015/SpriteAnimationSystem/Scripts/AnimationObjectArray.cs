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
	public class AnimationObjectArray
	{
		public AnimationObject[] animationObjects;

		public AnimationPlayer Player
		{
			get
			{
				if (animationObjects.Length > 0)
				{
					if (animationObjects[0] != null)
					{
						_player = animationObjects[0].Player;
						_player.Clones = new AnimationObject[animationObjects.Length - 1];
						for (int i = 0; i < _player.Clones.Length; i++)
							_player.Clones[i] = animationObjects[i + 1];
					}
				}
				return _player;
			}
		}

		private AnimationPlayer _player = new AnimationPlayer();
		
		public AnimationObjectArray() { }

		public AnimationObjectArray(AnimationObject[] array)
		{
			animationObjects = array;
		}

		public void SetShader(Shader shader)
		{
			foreach (var obj in animationObjects)
			{
				if (obj != null)
					obj.SetShader(shader);
			}
		}

	}
}
