/*
 * Simple atlas packer 2013-2014
 * Author: M. Yaroma
 * All rights reserved.
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TestTool.AtlasPacker
{
	// texure atlas convert to sprite atlas
	public class SpritePacker
	{
		private static List<SpriteData> _data = new List<SpriteData>();

		// read settings file and settings sprites
		public static List<SpriteMetaData> Parse(TextAsset text, int height, int alignment)
		{
			using (StringReader reader = new StringReader(text.text))
			{
				ParseAtlasInfo(text);
				List<SpriteMetaData> spriteAtlas = new List<SpriteMetaData>();
				foreach (SpriteData spriteData in _data)
				{
					SpriteMetaData spriteMetaData = new SpriteMetaData();
					spriteMetaData.name = spriteData.GetName();
					Rect rect = spriteData.GetRect();
					spriteMetaData.rect = new Rect(rect.x, height - rect.y - rect.height, rect.width, rect.height);
					spriteMetaData.pivot = Vector2.zero;

					spriteMetaData.alignment = alignment;
					spriteAtlas.Add(spriteMetaData);
				}
				_data.Clear();
				return spriteAtlas;
			}
		}
		
		// read and parse
		private static void ParseAtlasInfo(TextAsset text)
		{
			using (StringReader reader = new StringReader(text.text))
			{
				while (true)
				{
					string line = reader.ReadLine();
					if (line != null)
					{
						string[] sprData = line.Split(new char[] { ' ', '\t' });
						if (sprData.Length >= 5)
						{
							string[] name = sprData[0].Split('.');
							SpriteData sprite = new SpriteData(
								name[0],
								int.Parse(sprData[1]),
								int.Parse(sprData[2]),
								int.Parse(sprData[3]),
								int.Parse(sprData[4])
								);
							_data.Add(sprite);
						}
					}
					else
					{
						break;
					}
				}
			}
		}
	}
}
#endif