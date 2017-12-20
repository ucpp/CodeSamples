/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using System;
using UnityEditor;
using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	[CustomEditor(typeof(AnimationObject))]
	public class AnimationObjectInspector : Editor
	{
		public AnimationObject Target
		{
			get 
			{ 
				return target as AnimationObject; 
			}
		}

		private SerializedProperty _editorSettings = null;
		private SerializedProperty _playOnAwake = null;
		private SerializedProperty _loopPlayAwake = null;
		private SerializedProperty _indexSelectAnimation = null;
		private SerializedProperty _frameIndex = null;
		private SerializedProperty _timeScale = null;
		private SerializedProperty _flipX = null;
		private SerializedProperty _flipY = null;
		private SerializedProperty _threadName = null;
		private AnimationManager.Threads _thread = AnimationManager.Threads.baseThread;
		private AnimationDataAsset _safeDataAsset = null;
		private string[] _animations = null;
		private bool _isShowThreadsWindow = false;

		private void OnEnable()
		{
			if (!Application.isPlaying)
				Reload ();
			else
				Initialize ();
		}

		private void Initialize()
		{
			try 
			{
				InitializeAniamtionList();
				InitializeProperties();
			}
			catch(Exception exMessage) 
			{
				Debug.Log (exMessage);
			}
		}

		private void InitializeAniamtionList()
		{
			if (Target.data.DataAsset == null)
				return; 
			
			_animations = new string[Target.data.DataAsset.Animations.Count + 1];
			_animations [0] = "<none>";
			for (int i = 0; i < Target.data.DataAsset.Animations.Count; i++)
				_animations [i+1] = Target.data.DataAsset.Animations [i].name;

		}

		private void InitializeProperties()
		{
			_editorSettings = serializedObject.FindProperty("EditorSettingsData");
			_playOnAwake = _editorSettings.FindPropertyRelative("PlayOnAwake");
			_loopPlayAwake = _editorSettings.FindPropertyRelative("LoopPlayAwake");
			_indexSelectAnimation = _editorSettings.FindPropertyRelative("IndexSelectAnimation");
			_frameIndex = _editorSettings.FindPropertyRelative("FrameIndex");
			_timeScale = serializedObject.FindProperty("TimeScale");
			_flipX = _editorSettings.FindPropertyRelative ("FlipX");
			_flipY = _editorSettings.FindPropertyRelative ("FlipY");
			_threadName = serializedObject.FindProperty("ThreadName");
		}

		public override void OnInspectorGUI()
		{
			if (Target == null)
				return;
			
			serializedObject.Update();
			DrawSettingsUI ();
			UpdatePose ();
			serializedObject.ApplyModifiedProperties ();
		}
			
		private void DrawSettingsUI()
		{
			Target.data.DataAsset = (AnimationDataAsset)EditorGUILayout.ObjectField ("Data Asset", Target.data.DataAsset, typeof(AnimationDataAsset), true);
			if (Target.data.DataAsset != _safeDataAsset) 
			{
				Reload ();
				_safeDataAsset = Target.data.DataAsset;
			}
			EditorGUILayout.Space ();
			if (Target.data.DataAsset != null) 
			{
				_playOnAwake.boolValue = EditorGUILayout.Toggle ("Play on awake", _playOnAwake.boolValue);
				EditorGUILayout.BeginHorizontal ();
				_indexSelectAnimation.intValue = EditorGUILayout.Popup (_indexSelectAnimation.intValue, _animations);
				_loopPlayAwake.boolValue = EditorGUILayout.Toggle ("Loop", _loopPlayAwake.boolValue);
				EditorGUILayout.EndHorizontal ();
				_frameIndex.intValue = (int)EditorGUILayout.Slider ((float)_frameIndex.intValue, 0, GetAnimationLength ());
				_timeScale.floatValue = EditorGUILayout.FloatField ("Time Scale", _timeScale.floatValue);

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Flip", GUILayout.MaxWidth(100));
				_flipX.boolValue = EditorGUILayout.ToggleLeft ("X", _flipX.boolValue, GUILayout.MaxWidth(50));
				_flipY.boolValue = EditorGUILayout.ToggleLeft ("Y", _flipY.boolValue, GUILayout.MaxWidth(50));
				Target.FlipX = _flipX.boolValue;
				Target.FlipY = _flipY.boolValue;
				EditorGUILayout.EndHorizontal ();
				DrawThread ();
			}
		}

		private void DrawThread()
		{
			try 
			{
				_thread = (AnimationManager.Threads)Enum.Parse (typeof(AnimationManager.Threads), _threadName.stringValue);
			} 
			catch (Exception exMessage) 
			{
				Debug.Log("ERROR: " + exMessage);
				_thread = AnimationManager.Threads.baseThread;
			}

			if (_isShowThreadsWindow)
			{
				if (GUILayout.Button ("Save"))
				{
					_isShowThreadsWindow = false;
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal ();
				_thread = (AnimationManager.Threads)EditorGUILayout.EnumPopup (_thread);
				if (GUILayout.Button ("+", GUILayout.MaxWidth (20))) 
				{
					_isShowThreadsWindow = true;
				}
				EditorGUILayout.EndHorizontal ();
				_threadName.stringValue = _thread.ToString ();
			}
		}

		private int GetAnimationLength()
		{
			if(Target.data.DataAsset != null && _indexSelectAnimation.intValue > 0) 
				return Target.data.DataAsset.Animations [_indexSelectAnimation.intValue - 1].frames.Length - 1;
			return 0;
		}

		/// <summary>
		/// update animation renderer
		/// </summary>
		private void UpdatePose()
		{
			if (!Application.isPlaying && Target.data.DataAsset != null) 
			{
				if (_indexSelectAnimation.intValue > 0) 
					Target.Player.SetPose (Target.data.DataAsset.Animations [_indexSelectAnimation.intValue - 1].name, _frameIndex.intValue);
				else 
					Target.Player.SetPose (null, 0);
			}
		}

		/// <summary>
		/// reimport animation data
		/// </summary>
		private void Reload()
		{
			if (Application.isPlaying || Target == null)
				return;
			
			Initialize ();

			var root = Target.transform.Find ("root");
			if (root != null)
				Target.data.RootTransform = root;
			Target.data.Initialize (root);
			Target.Awake ();
		}

		/// <summary>
		/// is prefab or scene gameObject
		/// </summary>
		public bool IsPrefab()
		{
			return PrefabUtility.GetPrefabParent (Target.gameObject) == null 
				&& PrefabUtility.GetPrefabObject (Target.gameObject) != null;
		}
	}
}