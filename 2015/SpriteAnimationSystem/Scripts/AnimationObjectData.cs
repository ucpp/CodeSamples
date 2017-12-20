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
	public class AnimationObjectData
	{
		[HideInInspector]
		public AnimationDataAsset DataAsset;
		[HideInInspector]
		public Transform RootTransform;
		[HideInInspector]
		public SpriteRenderer Renderer;

		public bool FlipX
		{
			get
			{
				return _flipX;
			}
			set
			{
				if (_flipX != value)
				{
					_flipX = value;
					if (this.Renderer != null)
						this.Renderer.flipX = _flipX;
					Vector3 v = RootTransform.localPosition;
					RootTransform.localPosition = new Vector3(-1 * v.x, v.y, v.z);
				}
			}
		}

		public bool FlipY
		{
			get
			{
				return _flipY;
			}
			set
			{
				if (_flipY != value)
				{
					_flipY = value;
					if (this.Renderer != null)
						this.Renderer.flipY = _flipY;
					Vector3 v = RootTransform.localPosition;
					RootTransform.localPosition = new Vector3(v.x, -1 * v.y, v.z);
				}

			}
		}

		private bool _flipX = false;
		private bool _flipY = false;

		public void Initialize(Transform transform = null)
		{
			if (transform != null)
			{
				this.RootTransform = transform;
				var renderer = transform.GetComponent<SpriteRenderer>();
				if (renderer != null)
					this.Renderer = renderer;
			}
		}

		public Animation GetAnimation(string name)
		{
			if (DataAsset == null)
				return null;

			for (int i = 0; i < DataAsset.Animations.Count; i++)
			{
				if (DataAsset.Animations[i].name == name)
					return DataAsset.Animations[i];
			}
			return null;
		}

		public void SetSprite(Sprite sprite)
		{
			if (Renderer != null)
				Renderer.sprite = sprite;
		}
	}
}