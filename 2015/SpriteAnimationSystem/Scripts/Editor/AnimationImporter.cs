/*
 * Sprite Animation System 2015-2016
 * Author: M. Yaroma
 * All rights reserved.
*/

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
#define OLD_UNITY // in new versions, the window name is assigned via GUIContent
#endif

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TestTool.SpriteAnimationSystem
{
	public class AnimationImporter : EditorWindow
	{
		private UnityEngine.Object _directory = null;
		// importing progress (disable)
		private int _progress = 100;
		private int _pixelsPerUnits = 1;
		private bool _updateSpriteSettings = false;
		private Rect _progressBarRect = default(Rect);

		private Rect progressBarRect
		{
			get
			{
				if (_progressBarRect == default(Rect))
				{
					float offset = 5;
					float hieght = 20;
					float width = this.minSize.x - offset * 2;
					float x = offset;
					float y = this.minSize.y - hieght - offset;

					_progressBarRect = new Rect(x, y, width, hieght);
				}
				return _progressBarRect;
			}
		}

		[MenuItem("TestTools/Sprite Animation Importer")]
		private static void CreateWindow()
		{
			var window = CreateInstance<AnimationImporter>();
			Vector2 windowSize = new Vector2(350.0f, 150.0f);
			string title = "Sprite Animation Importer (ver." + Version.ToolsAndUtilities + ")";
#if OLD_UNITY
			window.title = title;
#else
			window.titleContent = new GUIContent(title);
#endif
			window.maxSize = windowSize;
			window.minSize = windowSize;
			window.ShowUtility();
		}

		private void OnGUI()
		{
			EditorGUILayout.HelpBox("Set directory for Import of Reimport animations", MessageType.Info, true);
			_directory = EditorGUILayout.ObjectField("Animations Directory:", _directory, typeof(UnityEngine.Object), true);
			_pixelsPerUnits = EditorGUILayout.IntField("Pixels per Units", _pixelsPerUnits);

			_updateSpriteSettings = EditorGUILayout.Toggle("Update sprites settings", _updateSpriteSettings);

			if (GUILayout.Button("Import") && _directory != null)
				Execute(AssetDatabase.GetAssetPath(_directory));

			EditorGUI.ProgressBar(progressBarRect, _progress, string.Format("Complete: {0}%", _progress));
		}

		private void Execute(string path)
		{
			if (Directory.Exists(path))
			{
				var directoryInfo = new DirectoryInfo(path);
				if (directoryInfo != null)
					CheckDirectory(directoryInfo);
			}
			AssetDatabase.SaveAssets();
		}

		private void CheckDirectory(DirectoryInfo directoryInfo)
		{
			if (ExistsAnimation(directoryInfo))
				RunImport(directoryInfo);
			var subDirectories = directoryInfo.GetDirectories();
			if (subDirectories.Length > 0)
			{
				foreach (var subDirectory in subDirectories)
					CheckDirectory(subDirectory);
			}
		}

		private void RunImport(DirectoryInfo directoryInfo)
		{
			var timelineFiles = GetTextAssets(directoryInfo);
			var imageFiles = GetSprites(directoryInfo);
			if (timelineFiles.Length > 0)
			{
				var anim = CreateAnimationObject(timelineFiles[0].name);
				var child = CreateChild(anim.gameObject);
				string objectName = anim.gameObject.name;
				if (anim != null)
				{
					anim.data = new AnimationObjectData();
					anim.data.DataAsset = CreateAsset(directoryInfo.FullName, objectName);
					anim.data.DataAsset.PixelsPerUnits = _pixelsPerUnits;
					FillSpritesPool(imageFiles, anim.data.DataAsset);
					SetAnimations(timelineFiles, anim);
					anim.data.Initialize(child.transform);
					anim.data.Renderer.sprite = imageFiles[0];
					EditorUtility.SetDirty(anim.data.DataAsset);
				}
				SavePrefab(anim.gameObject, directoryInfo.FullName);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		private void FillSpritesPool(Sprite[] sprites, AnimationDataAsset dataAsset)
		{
			dataAsset.CurrentSpritesPool = new SpritesPool();
			for (int i = 0; i < sprites.Length; i++)
			{
				if (sprites[i] != null)
					dataAsset.CurrentSpritesPool.Add(sprites[i]);
			}
		}

		private void SetAnimations(TextAsset[] assets, AnimationObject animationObject)
		{
			List<Animation> tmp = null;
			if (animationObject.data.DataAsset != null && animationObject.data.DataAsset.Animations != null)
				tmp = new List<Animation>(animationObject.data.DataAsset.Animations);

			animationObject.data.DataAsset.Animations = new List<Animation>();

			for (int i = 0; i < assets.Length; i++)
			{
				string animName = assets[i].name.Replace(animationObject.name, "").TrimStart('_');
				if (animationObject.data.GetAnimation(animName) == null)
				{
					var animation = new Animation();
					animation.name = animName;
					animation.timelineData = assets[i];
					animation.Initialize(animationObject.data.DataAsset.CurrentSpritesPool);
					animationObject.data.DataAsset.Animations.Add(animation);
					if (tmp != null && tmp.Count > i)
						animationObject.data.DataAsset.Animations[i].CopyEvents(tmp[i]);
				}
			}
		}

		private AnimationObject CreateAnimationObject(string name)
		{
			var animationObject = new GameObject(GetNameObject(name), typeof(AnimationObject));
			var anim = animationObject.GetComponent<AnimationObject>();

			return anim;
		}

		private void SavePrefab(GameObject animationObject, string path)
		{
			string fullPath = GetAssetPath(path, animationObject.name + ".prefab");
			if (AssetDatabase.LoadAssetAtPath(fullPath, typeof(GameObject)))
			{
				GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(fullPath, typeof(GameObject));
				prefab.GetComponent<AnimationObject>().data.DataAsset = animationObject.GetComponent<AnimationObject>().data.DataAsset;
			}
			else
				PrefabUtility.CreatePrefab(fullPath, animationObject, ReplacePrefabOptions.ConnectToPrefab);
			DestroyImmediate(animationObject);
			AssetDatabase.Refresh();
		}

		private string GetAssetPath(string path, string name)
		{
			int index = path.IndexOf("Assets");
			int length = path.Length - index;
			return Path.Combine(path.Substring(index, length), name).Replace("\\", "/");
		}

		private AnimationDataAsset CreateAsset(string path, string name)
		{
			string fullPath = GetAssetPath(path, name + ".asset");
			AnimationDataAsset dataAsset = (AnimationDataAsset)AssetDatabase.LoadAssetAtPath(fullPath, typeof(AnimationDataAsset));
			if (dataAsset == null)
			{
				dataAsset = ScriptableObject.CreateInstance<AnimationDataAsset>();
				AssetDatabase.CreateAsset(dataAsset, fullPath);
			}
			return dataAsset;
		}

		/// <summary>
		/// Create renderer for animation
		/// </summary>
		public GameObject CreateChild(GameObject animationObject)
		{
			if (!animationObject.transform.Find("root"))
			{
				animationObject.transform.position = Vector3.zero;
				var child = new GameObject("root", typeof(SpriteRenderer));
				child.transform.parent = animationObject.transform;
				//child.hideFlags = HideFlags.HideInHierarchy;
				return child;
			}
			return null;
		}

		private string GetNameObject(string fullName)
		{
			return fullName.Split('_')[0];
		}

		private TextAsset[] GetTextAssets(DirectoryInfo directoryInfo)
		{
			string[] paths;
			paths = GetPaths(directoryInfo, "*.txt");
			TextAsset[] files = new TextAsset[paths.Length];
			for (int i = 0; i < paths.Length; i++)
				files[i] = AssetDatabase.LoadAssetAtPath<TextAsset>(paths[i]);

			return files;
		}

		private Sprite[] GetSprites(DirectoryInfo directoryInfo)
		{
			string[] paths;
			paths = GetPaths(directoryInfo, "*.png");
			Sprite[] files = new Sprite[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				files[i] = AssetDatabase.LoadAssetAtPath<Sprite>(paths[i]);
				if (_updateSpriteSettings)
					SetTextureSettings(files[i].texture);
			}

			return files;
		}

		private string[] GetPaths(DirectoryInfo directoryInfo, string mask)
		{
			int index = directoryInfo.FullName.IndexOf("Assets");
			int length = directoryInfo.FullName.Length - index;
			string[] fullPaths = Directory.GetFiles(directoryInfo.FullName, mask);
			string[] shortPaths = new string[fullPaths.Length];
			for (int i = 0; i < fullPaths.Length; i++)
				shortPaths[i] = Path.Combine(fullPaths[i].Substring(index, length), Path.GetFileName(fullPaths[i]));
			return shortPaths;
		}

		private bool ExistsAnimation(DirectoryInfo directoryInfo)
		{
			string[] timelineDataFiles = GetPaths(directoryInfo, "*.txt");
			string[] imageFiles = GetPaths(directoryInfo, "*.png");
			if (timelineDataFiles.Length > 0 && imageFiles.Length > 0)
				return true;
			return false;
		}

		private void SetTextureSettings(Texture2D texture2D)
		{
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2D)) as TextureImporter;
			textureImporter.textureType = TextureImporterType.Sprite;
			TextureImporterSettings settings = new TextureImporterSettings();
			textureImporter.ReadTextureSettings(settings);
			settings.spriteAlignment = (int)SpriteAlignment.BottomLeft;
			textureImporter.SetTextureSettings(settings);
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.mipmapEnabled = false;
			textureImporter.spritePixelsPerUnit = _pixelsPerUnits;
			textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture2D), ImportAssetOptions.ForceUpdate);
		}
	}
}