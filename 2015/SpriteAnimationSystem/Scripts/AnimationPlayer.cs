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
	public delegate void OnStartEventHandler(string animationName, bool loop);
	public delegate void OnEndEventHandler(string animationName);
	public delegate void OnAnimationEvent(string eventName);
	public delegate void OnUpdateAnimation(Frame frame);

	public class AnimationPlayer
	{
		/// <summary>
		/// animation playback mode
		/// </summary>
		[Flags]
		public enum Mode
		{
			None             = 0x0000,
			CheckIdentity    = 0x0001, // do not start the animation with the same name until the previous one is over
			RandomStartFrame = 0x0002, // run animation from a random frame
			RandomAnimation  = 0x0004, // internal parameter for determining the random animation mode
			PlayTrackList    = 0x0008, // internal parameter for determining the playback mode of track animation
			PlayReverseTrack = 0x0010
		}

		public AnimationObject TargetAnimationObject = null;
		public AnimationObject[] Clones = null;

		/// <summary>
		/// the event at the start of the animation is called at the start
		/// of each animation except for looped animations. 
		/// For them, the call occurs in the first cycle
		/// </summary>
		public event OnStartEventHandler OnStart
		{
			add
			{
				_onStart -= value;
				_onStart += value;
			}
			remove
			{
				_onStart -= value;
			}
		}

		/// <summary>
		/// animation end events - in the last frame of each animation, except for looped animation
		/// </summary>
		public event OnEndEventHandler OnEnd
		{
			add
			{
				_onEnd -= value;
				_onEnd += value;
			}
			remove
			{
				_onEnd -= value;
			}
		}

		public event OnEndEventHandler OnComplete
		{
			add
			{
				_onComplete -= value;
				_onComplete += value;
			}
			remove
			{
				_onComplete -= value;
			}
		}

		public event OnAnimationEvent OnEvent
		{
			add
			{
				_onEvent -= value;
				_onEvent += value;
			}
			remove
			{
				_onEvent -= value;
			}
		}

		public event OnUpdateAnimation OnUpdate
		{
			add
			{
				_onUpdateAnimation -= value;
				_onUpdateAnimation += value;
			}
			remove
			{
				_onUpdateAnimation -= value;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return _isPlaying && !_isPause;
			}
			private set
			{
				_isPlaying = value;
			}
		}

		public string CurrentAnimationName { get; private set; }

		public string LastPlayedAnimationName { get; private set; }

		public bool Loop { get; private set; }

		public int TrackListLength
		{
			get
			{
				if (_trackList == null)
					return 0;
				return _trackList.Count;
			}
		}

		public bool IsReverse
		{
			get
			{
				return _isReverse;
			}
		}

		public string ThreadName = "";

		private AnimationTrack currentTrack
		{
			get
			{
				if (_trackList.Count == 0)
					_trackList.Add(new AnimationTrack());
				return _trackList[_trackList.Count - 1];
			}
		}

		private event OnEndEventHandler _onEnd = null;
		private event OnEndEventHandler _onComplete = null;
		private event OnAnimationEvent _onEvent = null;
		private event OnUpdateAnimation _onUpdateAnimation = null;
		private event OnStartEventHandler _onStart = null;
		private bool _isPlaying = false;
		private bool _isPause = false;
		private List<AnimationTrack> _trackList = new List<AnimationTrack>();
		private bool _isReverse = false;
		private string[] _animationsArray = null;
		private Mode _mode = Mode.None;
		private float _dt = 0;
		private Dictionary<string, Attachment> _attachments = new Dictionary<string, Attachment>();

		/// <summary>
		/// Setting the starting position of the object to the specified animation and frame
		/// </summary>
		/// <param name="animationName">animation name</param>
		/// <param name="frameIndex">frame number</param>
		public void SetPose(string animationName, int frameIndex)
		{
			if (string.IsNullOrEmpty(animationName))
			{
				currentTrack.Set(TargetAnimationObject.data.DataAsset.Animations[0]);
			}
			else
			{
				currentTrack.Set(TargetAnimationObject.data.GetAnimation(animationName));
				if (currentTrack != null)
				{
					frameIndex = Mathf.Clamp(frameIndex, 0, currentTrack.CurrentAnimation.frames.Length - 1);
					currentTrack.CurrentFrame = frameIndex;
				}
			}
			NormalizeFrame();
		}

		public void Play(string[] animationsArray, bool loop = false, Mode mode = Mode.None)
		{
			if (CheckIdentityMode(mode, animationsArray))
				return;
			if (animationsArray == null)
				return;
			string animationName = animationsArray[UnityEngine.Random.Range(0, animationsArray.Length)];
			this._animationsArray = animationsArray;
			this._mode = mode;
			Play(animationName, loop, mode | Mode.RandomAnimation);
		}

		public void Play(string animationName, Mode mode)
		{
			Play(animationName, false, mode);
		}

		public void Play(string animationName, bool loop = false, Mode mode = Mode.None)
		{
			if (CheckIdentityMode(mode, animationName))
				return;
			CheckRandomMode(mode);
			CheckTrackListMode(mode);
			_isReverse = CheckReverseTrack(mode);

			if (AddTrack(animationName))
				CurrentAnimationName = animationName;
			if (currentTrack.CurrentAnimation != null)
			{
				IsPlaying = true;
				Loop = loop;
			}
			CheckRandomFrameMode(mode);
			UpdateTracks(0, ThreadName);
		}

		private void CheckRandomMode(Mode mode)
		{
			if ((mode & Mode.RandomAnimation) != Mode.RandomAnimation && (this._mode & Mode.RandomAnimation) == Mode.RandomAnimation)
			{
				this._animationsArray = null;
				this._mode &= ~Mode.RandomAnimation;
			}
		}

		private void CheckRandomFrameMode(Mode mode)
		{
			if ((mode & Mode.RandomStartFrame) == Mode.RandomStartFrame)
				currentTrack.SetRandomStartFrame();
		}

		private void CheckTrackListMode(Mode mode)
		{
			if ((mode & Mode.PlayTrackList) != Mode.PlayTrackList)
				ClearTracks();
		}

		private bool CheckIdentityMode(Mode mode, string animationName)
		{
			if ((mode & Mode.CheckIdentity) == Mode.CheckIdentity && animationName == CurrentAnimationName)
			{
				if (!string.IsNullOrEmpty(CurrentAnimationName))
					return true;
			}
			return false;
		}

		private bool CheckIdentityMode(Mode mode, string[] animationNames)
		{
			if ((mode & Mode.CheckIdentity) == Mode.CheckIdentity && Equals(animationNames, _animationsArray))
			{
				if (_animationsArray != null)
					return true;
			}
			return false;
		}

		private bool CheckReverseTrack(Mode mode)
		{
			return (mode & Mode.PlayReverseTrack) == Mode.PlayReverseTrack;
		}

		private bool Equals(string[] a, string[] b)
		{
			if (a == null || b == null)
				return false;

			if (a.Length == b.Length)
			{
				for (int i = 0; i < a.Length; i++)
				{
					if (!a[i].Equals(b[i]))
						return false;
				}
				return true;
			}
			return false;
		}

		public void PlayTrackList()
		{
			_trackList.Reverse();
			PlayReverseTrackList();
		}

		public void PlayReverseTrackList()
		{
			for (int i = 0; i < _trackList.Count; i++)
			{
				if (_trackList[i] == null || _trackList[i].CurrentAnimation == null)
					_trackList.Remove(_trackList[i]);
			}
			Play(null, Mode.PlayTrackList);
		}

		public bool AddTrack(string animationName, int countRepeat = 1)
		{
			var anim = TargetAnimationObject.data.GetAnimation(animationName);
			if (anim != null)
			{
				var track = new AnimationTrack();
				track.Set(anim);
				for (int i = 0; i < countRepeat; i++)
					_trackList.Add(track);
				return true;
			}
			return false;
		}

		public void ClearTracks()
		{
			_trackList.Clear();
		}

		public void Pause()
		{
			_isPause = true;
		}

		public void UnPause()
		{
			_isPause = false;
		}

		/// <summary>
		/// stop animation and clear track list
		/// </summary>
		public void Stop()
		{
			_trackList.Remove(_trackList[_trackList.Count - 1]);
			if (_trackList.Count == 0)
			{
				IsPlaying = false;
				currentTrack.Clear();

				Loop = false;
				_animationsArray = null;
				_mode = Mode.None;
				CurrentAnimationName = string.Empty;
			}
		}

		private void NormalizeFrame()
		{
			if (!AnimationTrack.IsNullOrEmpty(currentTrack))
			{
				var frame = currentTrack.GetCurrentFrame();
				if (frame != null)
					SetFrameData(frame);
			}
		}

		public void UpdateTracks(float deltaTime = 0, string thread = "")
		{
			if (this.ThreadName != thread)
				return;

			if (IsPlaying)
			{
				var frame = currentTrack.GetCurrentFrame();
				if (_dt == 0)
					SetFrame(frame);
				_dt += deltaTime * TargetAnimationObject.timeScale;
				if (_dt >= frame.TimeLife)
				{
					_dt = 0;
					if (IsReverse)
						currentTrack.PrevFrame();
					else
						currentTrack.NextFrame();
					EndListener();
				}
			}
		}

		private void SetFrame(Frame frame)
		{
			if (frame != null)
			{
				SetFrameData(frame);
				StartListener();
				EventListener(frame);
			}
		}

		private void SetFrameData(Frame frame)
		{
			TargetAnimationObject.data.SetSprite(frame.Image);

			int dx = TargetAnimationObject.FlipX ? -1 : 1;
			int dy = TargetAnimationObject.FlipY ? -1 : 1;
			float x = dx * frame.x * TargetAnimationObject.data.DataAsset.PixelsPerUnits;
			float y = dy * frame.y * TargetAnimationObject.data.DataAsset.PixelsPerUnits;
			float z = TargetAnimationObject.data.RootTransform.localPosition.z;
			TargetAnimationObject.data.RootTransform.localPosition = new Vector3(x, y, z);
			if (Clones != null)
			{
				for (int i = 0; i < Clones.Length; i++)
				{
					Clones[i].data.SetSprite(frame.Image);
					Clones[i].data.RootTransform.localPosition = new Vector3(x, y, z);
				}
			}
			_attachments.Clear();
			if (frame.Attachments != null)
			{
				for (int i = 0; i < frame.Attachments.Length; i++)
				{
					if (!_attachments.ContainsKey(frame.Attachments[i].Name))
					{
						Attachment at = new Attachment(frame.Attachments[i]);
						at.x = dx * at.x * TargetAnimationObject.data.DataAsset.PixelsPerUnits;
						at.y = dy * at.y * TargetAnimationObject.data.DataAsset.PixelsPerUnits;
						_attachments.Add(at.Name, at);
					}
				}
			}
			if (_onUpdateAnimation != null)
				_onUpdateAnimation(frame);
		}

		private void EventListener(Frame frame)
		{
			if (!string.IsNullOrEmpty(frame.EventName))
			{
				if (_onEvent != null)
					_onEvent(frame.EventName);
			}
		}

		private void StartListener()
		{
			if (currentTrack.CurrentFrame == 0)
			{
				CurrentAnimationName = currentTrack.CurrentAnimation.name;
				LastPlayedAnimationName = currentTrack.CurrentAnimation.name;
				if (_onStart != null)
					_onStart(CurrentAnimationName, Loop);
			}

		}

		private void EndListener()
		{
			if (currentTrack.CurrentFrame == 0)
			{
				if (_onComplete != null)
					_onComplete(CurrentAnimationName);
				if (Loop)
				{
					if (_animationsArray != null)
						Play(_animationsArray, Loop, _mode &= ~Mode.CheckIdentity);
				}
				else
				{
					string currentAnimName = CurrentAnimationName;
					Stop();
					if (_onEnd != null)
						_onEnd(currentAnimName);
				}
			}
		}

		/// <summary>
		/// Get attachment point. 
		/// For optimal performance, it is recommended to call on the OnUpdate event
		/// </summary>
		public Attachment GetAttachment(string name)
		{
			if (_attachments.ContainsKey(name))
				return _attachments[name];
			return null;
		}
	}
}
