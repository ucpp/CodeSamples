/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

namespace TestTool.SpriteAnimationSystem
{
	public delegate void OnUpdate(float dt, string thread);

	public class AnimationManager
	{
		public static AnimationManager Instance
		{
			get 
			{
				if (_instance == null) 
					_instance = new AnimationManager ();
				return _instance;
			}
		}
		
		public event OnUpdate OnUpdateEventHandler
		{
			add
			{
				_onUpdateEventHandler -= value;
				_onUpdateEventHandler += value;
			}
			remove 
			{
				_onUpdateEventHandler -= value;
			}
		}

		/// <summary>
		/// All animations are update in MonoBehaviour.Update()
		/// </summary>
		public bool IsAutoUpdate
		{
			get 
			{
				return _isAutoUpdate;
			}
			set 
			{
				_isAutoUpdate = value;
			}
		}

		private static AnimationManager _instance = null;
		private event OnUpdate _onUpdateEventHandler = null;
		private bool _isAutoUpdate = true;

		public enum Threads
		{
			baseThread,
			thread1,
			thread2,
			thread3,
			thread4,
			thread5
		}

		public void Update(float dt, string thread = "")
		{
			if (!IsAutoUpdate) 
			{
				if (_onUpdateEventHandler != null)
					_onUpdateEventHandler (dt, thread);
			}
		}
	}
}