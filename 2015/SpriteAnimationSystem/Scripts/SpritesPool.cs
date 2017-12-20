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
	public class SpritesPool
	{
		[SerializeField]
		private List<Sprite> SpritesList;

		/// <summary>
		/// get a sprite by name
		/// TODO: convert to IComparer and sort after filling
		/// </summary>
		/// <param name="name">имя</param>
		public Sprite Get(string name)
		{
			if (SpritesList == null)
				return null;

			for (int i = 0; i < SpritesList.Count; i++) 
			{
				if (SpritesList [i].name == name)
					return SpritesList [i];
			}
			Debug.Log ("Not find sprite " + name);
			return null;
		}

		public void Add(Sprite sprite)
		{
			if (SpritesList == null)
				SpritesList = new List<Sprite> ();

			if (!SpritesList.Contains (sprite))
				SpritesList.Add (sprite);
		}

		public void Clear()
		{
			if(SpritesList != null)
				SpritesList.Clear ();
		}
	}
}