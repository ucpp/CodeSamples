/*
 * Simple atlas packer 2013-2014
 * Author: M. Yaroma
 * All rights reserved.
 */

using UnityEngine;

namespace TestTool.AtlasPacker
{
	public class AtlasNode
	{
		public AtlasNode[] Child = null;
		public Rect Rectangle = new Rect(0, 0, 0, 0);
		public Texture2D Image = null;

		public AtlasNode(int x, int y, int width, int height)
		{
			Rectangle = new Rect(x, y, width, height);
			Child = new AtlasNode[2];
			Child[0] = null;
			Child[1] = null;
			Image = null;
		}

		public bool IsLeaf()
		{
			return Child[0] == null && Child[1] == null;
		}

		public AtlasNode Insert(Texture2D image, Borders borders)
		{
			//if we're not a leaf then
			if (!IsLeaf())
			{
				//try inserting into first child
				AtlasNode newNode = Child[0].Insert(image, borders);
				if (newNode != null)
					return newNode;
				//no room, insert into second
				return Child[1].Insert(image, borders);
			}
			else
			{
				//if there's already a texture here, return
				if (this.Image != null)
					return null;
				//if we're too small, return
				if (image.width > Rectangle.width || image.height > Rectangle.height)
					return null;
				//if we're just right, accept
				if (image.width == Rectangle.width && image.height == Rectangle.height)
				{
					this.Image = image;
					return this;
				}
				//decide which way to split
				int dw = (int)Rectangle.width - image.width - borders.Width;
				int dh = (int)Rectangle.height - image.height - borders.Height;
				//create some kids
				if (dw > dh)
				{
					Child[0] = new AtlasNode(
						(int)Rectangle.x,
						(int)Rectangle.y,
						(int)image.width,
						(int)Rectangle.height);

					Child[1] = new AtlasNode(
						(int)Rectangle.x + (int)image.width + borders.Width,
						(int)Rectangle.y,
						(int)Rectangle.width - (int)image.width - borders.Width,
						(int)Rectangle.height);
				}
				else
				{
					Child[0] = new AtlasNode(
						(int)Rectangle.x,
						(int)Rectangle.y,
						(int)Rectangle.width,
						(int)image.height);

					Child[1] = new AtlasNode(
						(int)Rectangle.x,
						(int)Rectangle.y + (int)image.height + borders.Height,
						(int)Rectangle.width,
						(int)Rectangle.height - (int)image.height - borders.Height);
				}
				//insert into first child we created
				return Child[0].Insert(image, borders);
			}
		}
	}
}
