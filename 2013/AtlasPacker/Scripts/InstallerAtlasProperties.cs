/*
 * Simple atlas packer  2013-2014
 * Author: M. Yaroma
 * All rights reserved.
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TestTool.AtlasPacker
{
	public class InstallerAtlasProperties
	{
		// set propertis readable for texture
		public static void SetReadable(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2D)) as TextureImporter;
				if (textureImporter != null)
				{
					textureImporter.isReadable = true;
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture2D), ImportAssetOptions.ForceUpdate);
				}
			}
		}

		// set default settings
		public static void SetDefaultSettings(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2D)) as TextureImporter;
				if (textureImporter != null)
				{
					textureImporter.isReadable = true;
					textureImporter.filterMode = FilterMode.Point;
					textureImporter.textureType = TextureImporterType.Default;
					textureImporter.textureFormat = TextureImporterFormat.ATC_RGBA8;
					textureImporter.maxTextureSize = 2048;
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture2D), ImportAssetOptions.ForceUpdate);
				}
			}
		}
	}
}
#endif