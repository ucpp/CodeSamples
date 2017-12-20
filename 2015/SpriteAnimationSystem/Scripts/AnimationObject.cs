/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	public class AnimationObject : MonoBehaviour
	{
		public AnimationPlayer Player
		{
			get
			{
				if (_player.TargetAnimationObject == null)
					_player.TargetAnimationObject = this;
				return _player;
			}
		}

		/// <summary>
		/// settings in inspector
		/// </summary>
		[HideInInspector]
		public EditorSettings EditorSettingsData;

		/// <summary>
		/// animation playback speed
		/// </summary>
		[HideInInspector]
		public float TimeScale = 1.0f;

		public bool FlipX
		{
			get
			{
				return data.FlipX;
			}
			set
			{
				data.FlipX = value;
			}
		}

		public bool FlipY
		{
			get
			{
				return data.FlipY;
			}
			set
			{
				data.FlipY = value;
			}
		}

		public AnimationObjectData data;

		/// <summary>
		/// The name of the thread.
		/// </summary>
		public string ThreadName = AnimationManager.Threads.baseThread.ToString();

		private AnimationPlayer _player = new AnimationPlayer();
		
		/// <summary>
		/// Object initialize
		/// </summary>
		public void Awake()
		{
			if (data != null && data.DataAsset.Animations != null && data.DataAsset.Animations.Count > 0)
			{
				if (EditorSettingsData.PlayOnAwake && EditorSettingsData.IndexSelectAnimation > 0)
				{
					var anim = data.DataAsset.Animations[EditorSettingsData.IndexSelectAnimation - 1];
					if (anim != null)
					{
						FlipX = EditorSettingsData.FlipX;
						FlipY = EditorSettingsData.FlipY;
						Player.SetPose(anim.name, EditorSettingsData.FrameIndex);
						Player.Play(anim.name, EditorSettingsData.LoopPlayAwake);
					}
				}
			}
			if (Application.isPlaying)
			{
				Player.ThreadName = ThreadName;
				AnimationManager.Instance.OnUpdateEventHandler += Player.UpdateTracks;
			}
		}

		private void Update()
		{
			if (AnimationManager.Instance.IsAutoUpdate)
				Player.UpdateTracks(Time.deltaTime, Player.ThreadName);
		}

		public void SetShader(Shader shader)
		{
			if (data == null || data.Renderer == null)
				return;
			data.Renderer.material.shader = shader;
		}

		private void OnDestroy()
		{
			AnimationManager.Instance.OnUpdateEventHandler -= Player.UpdateTracks;
		}
	}
}