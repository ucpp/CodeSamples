/*
 * Simple atlas packer 2013-2014
 * Author: M. Yaroma
 * All rights reserved.
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TestTool.AtlasPacker
{
	// enum available atlas sizes
	public enum AtlasSize
	{
		_256 = 256,
		_512 = 512,
		_1024 = 1024,
		_2048 = 2048,
		_4096 = 4096
	}

	// custom window draw atlas packer tool
	public class AtlasPacker : EditorWindow
	{
		private List<FileInfo> _files = new List<FileInfo>();
		private FileInfo _delFile = null;
		private Vector2 _scrollPosition = new Vector2(0, 0);
		private AtlasSize _atlasWidth = AtlasSize._512;
		private AtlasSize _atlasHeight = AtlasSize._512;
		private string _atlasPath = "";
		private int _width = 0;
		private int _height = 0;
		private SpriteAlignment _spriteAligment = SpriteAlignment.Center;
		private List<AtlasTexture> _textures = null;
		private Borders _borders = new Borders();


		[MenuItem("TestTools/Simple Atlas Packer", false, 0)]
		private static void InitializeAtlasPackerWindow()
		{
			var window = CreateInstance<AtlasPacker>();

			string title = "Atlas packer ver. 1.0.2";
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
			window.title = title;
#else
			window.titleContent = new GUIContent(title);
#endif

			window.ShowUtility();
		}

		// draw gui
		private void OnGUI()
		{
			DrawAtlasSettingsGUI();

			if (GUILayout.Button("Changed images folder"))
			{
				_atlasPath = EditorUtility.OpenFolderPanel("Changed folder", "", "");
				if (_atlasPath != "")
					_files = GetImageFiles(GetDirectory(_atlasPath));
			}

			DrawSpritesListGUI();

			if (_atlasPath != "")
			{
				GUI.color = Color.green;
				if (GUILayout.Button("Generate texture atlas"))
					GenerateAtlas();
			}
		}

		private void DrawAtlasSettingsGUI()
		{
			_atlasWidth = (AtlasSize)EditorGUILayout.EnumPopup("Width:", _atlasWidth);
			_atlasHeight = (AtlasSize)EditorGUILayout.EnumPopup("Height:", _atlasHeight);
			_spriteAligment = (SpriteAlignment)EditorGUILayout.EnumPopup("Aligment", _spriteAligment);

			DrawSetBordersGUI();
		}

		// set sprite borders for fix artifacts
		private void DrawSetBordersGUI()
		{
			_borders.IsRepeat = EditorGUILayout.Toggle("Is repeat border", _borders.IsRepeat);
			if (_borders.IsRepeat)
			{
				_borders.RepeatSize = EditorGUILayout.IntField("Repeat size", _borders.RepeatSize);
			}
			else
			{
				_borders.Left = EditorGUILayout.IntField("Left border", _borders.Left);
				_borders.Right = EditorGUILayout.IntField("Right border", _borders.Right);
				_borders.Top = EditorGUILayout.IntField("Top border", _borders.Top);
				_borders.Bottom = EditorGUILayout.IntField("Bottom border", _borders.Bottom);
			}
		}

		private void DrawSpritesListGUI()
		{
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

			foreach (FileInfo file in _files)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(file.Name);
				if (GUILayout.Button("Delete"))
					_delFile = file;
				GUILayout.EndHorizontal();
			}
			if (_delFile != null)
			{
				_files.Remove(_delFile);
			}
			GUILayout.EndScrollView();
		}

		private DirectoryInfo GetDirectory(string path)
		{
			return new DirectoryInfo(path);
		}

		private void GenerateAtlas()
		{
			List<FileInfo> imageFiles = _files;
			List<ImageName> textureList = new List<ImageName>();

			SetSizeAtlas();

			foreach (FileInfo f in imageFiles)
			{
				string path = @"Assets\" + Regex.Split(f.FullName, @"Assets")[1].Remove(0, 1);
				Texture2D image = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

				if (image.width > _width || image.height > _height)
				{
					Debug.LogError("Error: '" + f.Name + "' (" + image.width + "x" + image.height
								   + ") is larger than the atlas (" + _width + "x" + _height + ")");
					return;
				}
				textureList.Add(new ImageName(image, f.Name));
			}
			_textures = new List<AtlasTexture>();
			_textures.Add(new AtlasTexture(_width, _height, _borders));
			int count = 0;
			foreach (ImageName imageName in textureList)
			{
				bool added = false;
				foreach (AtlasTexture texture in _textures)
				{
					if (texture.AddImage(imageName.image, imageName.name, _borders))
					{
						added = true;
						break;
					}
				}

				if (!added)
				{
					AtlasTexture texture = new AtlasTexture(_width, _height, _borders);
					texture.AddImage(imageName.image, imageName.name, _borders);
					_textures.Add(texture);
					Debug.Log("Creating another athlas");
				}
			}
			count = 0;
			Debug.Log("textures.Count = " + _textures.Count.ToString());
			foreach (AtlasTexture texture in _textures)
			{
				if (texture == null)
					Debug.LogError("Texture null!");
				else
				{
					string path = EditorUtility.SaveFilePanel("Save", Application.dataPath, "atlas" + count, "png");
					Debug.Log("Writing atlas: " + path);
					if (texture.Image == null)
						Debug.LogError("Image null!");
					texture.Image.Apply();
					texture.Write(path);
					count++;
					AssetDatabase.Refresh();
					GenerateSprites(path, _spriteAligment);
				}
			}

		}

		// generate sprite atlas from texture atlas
		private void GenerateSprites(string path, SpriteAlignment alignment)
		{
			string atlasPath = path.Replace(".png", ".txt");
			atlasPath = @"Assets\" + Regex.Split(atlasPath, @"Assets")[1].Remove(0, 1);
			TextAsset atlas = (TextAsset)AssetDatabase.LoadAssetAtPath(atlasPath, typeof(TextAsset));

			path = @"Assets\" + Regex.Split(path, @"Assets")[1].Remove(0, 1);
			Texture2D image = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

			List<SpriteMetaData> sprites = SpritePacker.Parse(atlas, _height, (int)alignment);
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(image)) as TextureImporter;
			textureImporter.spritesheet = sprites.ToArray();
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.spriteImportMode = SpriteImportMode.Multiple;
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			InstallerAtlasProperties.SetDefaultSettings(image);
		}

		// set width and height for atlas
		private void SetSizeAtlas()
		{
			_width = (int)_atlasWidth;
			_height = (int)_atlasHeight;
		}

		// get the paths of all selected textures 
		private List<FileInfo> GetImageFiles(DirectoryInfo directory)
		{
			string[] files = Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories)
				.Where(s => GetFormat(s)).ToArray();
			Debug.Log(files.Length + " textures found.");
			List<FileInfo> fileList = new List<FileInfo>();
			foreach (string file in files)
				fileList.Add(new FileInfo(file));
			return new List<FileInfo>(fileList);
		}

		// get texture format
		private bool GetFormat(string line)
		{
			return GetEnd(line, ".png") ||
				   GetEnd(line, ".tga") ||
				   GetEnd(line, ".jpg") ||
				   GetEnd(line, ".psd") ||
				   GetEnd(line, ".dds");
		}

		// whether a string ends with "line" substring "end"
		private bool GetEnd(string line, string end)
		{
			return line.ToLower().EndsWith(end);
		}

		// texture and name
		private class ImageName
		{
			public Texture2D image;
			public string name;

			public ImageName(Texture2D image, string name)
			{
				this.image = image;
				this.name = name;
			}
		}
	}
}
#endif