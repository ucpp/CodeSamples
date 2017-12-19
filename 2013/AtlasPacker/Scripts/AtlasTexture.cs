/*
 * Simple atlas packer  2013-2014
 * Author: M. Yaroma
 * All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TestTool.AtlasPacker
{
#if UNITY_EDITOR
	// altas
	public class AtlasTexture
	{
		public Texture2D Image = null;

		private Dictionary<string, Rect> _rectangleMap = new Dictionary<string, Rect>();
		private AtlasNode _root = null;

		public AtlasTexture(int width, int height, Borders borders = null)
		{
			if (borders == null)
				borders = new Borders();

			Image = new Texture2D(width, height);

			Color32 col = new Color32(255, 255, 255, 0);
			Color32[] pixels = Image.GetPixels32();

			for (int i = 0; i < pixels.Length; ++i)
				pixels[i] = col;

			Image.SetPixels32(pixels);
			Image.Apply();

			_root = new AtlasNode(borders.Left, borders.Bottom, width - borders.Left, height - borders.Bottom);
		}

		public bool AddImage(Texture2D img, string name, Borders borders)
		{
			AtlasNode node = _root.Insert(img, borders);
			if (node == null)
				return false;
			_rectangleMap.Add(name, node.Rectangle);

			try
			{
				Image.SetPixels((int)node.Rectangle.x, (int)node.Rectangle.y, (int)node.Rectangle.width, (int)node.Rectangle.height, img.GetPixels());
			}
			catch (Exception exceptionMessage)
			{
				Debug.Log(exceptionMessage);
				InstallerAtlasProperties.SetReadable(img);
				Image.SetPixels((int)node.Rectangle.x, (int)node.Rectangle.y, (int)node.Rectangle.width, (int)node.Rectangle.height, img.GetPixels());
			}

			if (borders.IsRepeat)
			{
				Color color = new Color();

				for (int b = 0; b < borders.RepeatSize; b++)
				{
					for (int i = 0; i < img.width; i++)
					{
						color = img.GetPixel(i, 0);
						Image.SetPixel(i + (int)node.Rectangle.x, (int)node.Rectangle.y - b - 1, color);

						color = img.GetPixel(i, img.height - 1);
						Image.SetPixel(i + (int)node.Rectangle.x, (int)node.Rectangle.y + (int)node.Rectangle.height + b, color);
					}
				}

				for (int b = 0; b < borders.RepeatSize; b++)
				{
					for (int i = 0; i < img.height; i++)
					{
						color = img.GetPixel(0, i);
						Image.SetPixel((int)node.Rectangle.x - b - 1, (int)node.Rectangle.y + i, color);

						color = img.GetPixel(img.width - 1, i);
						Image.SetPixel((int)node.Rectangle.x + (int)node.Rectangle.width + b, (int)node.Rectangle.y + i, color);
					}
				}
			}
			if (Image == null)
			{
				Debug.LogError("Image null!");
			}
			return true;
		}

		// write settings athlas file
		public void Write(string name)
		{
			TextWriter tw = new StreamWriter(Path.ChangeExtension(name, ".txt"));

			foreach (KeyValuePair<string, Rect> rectangle in _rectangleMap)
			{
				Rect rect = rectangle.Value;
				tw.WriteLine(rectangle.Key + " " + rect.x + " " + (Image.height - rect.y - rect.height) + " " + rect.width + " " + rect.height);
			}
			tw.Close();
			if (Image == null)
			{
				Debug.LogError("Image null!");
			}
			else
			{
				byte[] bytes = Image.EncodeToPNG();
				FileStream fileStream = new FileStream(name, FileMode.Create, FileAccess.Write);
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				for (int i = 0; i < bytes.Length; i++)
					binaryWriter.Write(bytes[i]);
				binaryWriter.Close();
			}
		}
	}
#endif
}
