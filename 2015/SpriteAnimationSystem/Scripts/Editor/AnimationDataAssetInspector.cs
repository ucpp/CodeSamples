/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

using UnityEditor;
using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	[CustomEditor(typeof(AnimationDataAsset))]
	public class AnimationDataAssetInspector : Editor
	{
		public AnimationDataAsset Target
		{
			get
			{
				return target as AnimationDataAsset;
			}
		}

		private const float ScalingSpeed = 0.2f;

		private PreviewRenderUtility _previewRenderUtility = null;
		private AnimationTrack _currentTrack = new AnimationTrack();
		private Sprite _currentFrame = null;
		private SerializedProperty _isEditorMode = null;
		private bool _showAnimations = true;
		private bool _isPlaying = false;
		private int _indexPlaying = 0;
		private float _dt = 0.0f;
		private float _time = 0.0f;
		private float _previewSize = 3.0f;

		/// <summary>
		/// scale of the image in the preview window from 0.8 to 8
		/// </summary>
		private float PreviewSize
		{
			get
			{
				return _previewSize;
			}
			set
			{
				_previewSize = value;
				_previewSize = Mathf.Clamp(_previewSize, 0.8f, 8.0f);
			}
		}

		private void OnEnable()
		{
			InitializeProperties();
			EditorApplication.update += Update;
			_time = Time.realtimeSinceStartup;
		}

		private void InitializeProperties()
		{
			_isEditorMode = serializedObject.FindProperty("IsEditorMode");
		}

		public override void OnInspectorGUI()
		{
			if (Target == null)
				return;

			serializedObject.Update();
			DrawAnimationsListUI();
			EditorGUILayout.Space();
			_isEditorMode.boolValue = EditorGUILayout.Foldout(_isEditorMode.boolValue, "Details");
			serializedObject.ApplyModifiedProperties();
			if (_isEditorMode.boolValue)
			{
				EditorGUI.indentLevel++;
				base.OnInspectorGUI();
				EditorGUI.indentLevel--;
			}
		}

		/// <summary>
		/// Drawing a list of animations and play/stop buttons
		/// </summary>
		private void DrawAnimationsListUI()
		{
			_showAnimations = EditorGUILayout.Foldout(_showAnimations, "Animations");
			if (_showAnimations)
			{
				for (int i = 0; i < Target.Animations.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					GUI.contentColor = _isPlaying && i == _indexPlaying ? Color.black : Color.white;
					if (GUILayout.Button("\u25BA", GUILayout.Width(24)))
					{
						if (_indexPlaying == i)
							_isPlaying = !_isPlaying;

						_indexPlaying = i;
						_currentTrack.Set(Target.Animations[i]);
					}
					GUI.contentColor = Color.white;
					var currentAnimation = Target.Animations[i];
					string animationData = currentAnimation.name + " (" + currentAnimation.frames.Length + " frames)";
					EditorGUILayout.LabelField(animationData);
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		/// <summary>
		/// Initializing the preview window
		/// </summary>
		private void ValidateData()
		{
			if (_previewRenderUtility == null)
			{
				_previewRenderUtility = new PreviewRenderUtility();

				_previewRenderUtility.m_Camera.transform.position = new Vector3(0, 0, -10);
				_previewRenderUtility.m_Camera.transform.rotation = Quaternion.identity;
				_previewRenderUtility.m_Camera.orthographic = true;
			}
		}

		/// <summary>
		/// Enable and initialize the preview window
		/// </summary>
		public override bool HasPreviewGUI()
		{
			ValidateData();
			return true;
		}

		private void OnDestroy()
		{
			if (_previewRenderUtility != null)
				_previewRenderUtility.Cleanup();
			EditorApplication.update -= Update;
		}

		/// <summary>
		/// Draw preview window
		/// </summary>
		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				var img = _currentFrame;
				if (_currentFrame == null)
					img = Target.Animations[0].frames[0].Image;

				float w = img.rect.width * PreviewSize;
				float h = img.rect.height * PreviewSize;
				float x = r.x + r.width / 2 - w / 2;
				float y = r.y + r.height / 2 - h / 2;
				GUI.DrawTexture(new Rect(x, y, w, h), img.texture);
			}

			if (Event.current.type == EventType.ScrollWheel)
			{
				Repaint();
				if (Event.current.delta.y > 0)
					PreviewSize += ScalingSpeed;
				else if (Event.current.delta.y < 0)
					PreviewSize -= ScalingSpeed;
			}
		}

		/// <summary>
		/// Called by the editor's callback
		/// </summary>
		private void Update()
		{
			// calculating Time.deltaTime in the editor
			float deltaTime = Time.realtimeSinceStartup - _time;
			UpdateTracks(deltaTime);
			_time = Time.realtimeSinceStartup;
		}

		private void UpdateTracks(float deltaTime)
		{
			if (_isPlaying)
			{
				var frame = _currentTrack.GetCurrentFrame();
				if (_dt == 0)
				{
					if (frame != null)
						_currentFrame = frame.Image;
					Repaint();
				}
				_dt += deltaTime;
				if (_dt >= frame.TimeLife)
				{
					_dt = 0;
					_currentTrack.NextFrame();
				}
			}
		}
	}
}