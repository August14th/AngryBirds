using UnityEngine;

using UnityEditor;
using System.IO;

public static class ImageSlicer
{
	[MenuItem("Assets/ImageSlicer/Process to Sprites")]
	static void ProcessToSprite()
	{
		Texture2D atlas = Selection.activeObject as Texture2D;
		string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(atlas));
		string path = rootPath + "/" + atlas.name + ".PNG";


		TextureImporter atlasInfo = AssetImporter.GetAtPath(path) as TextureImporter;//获取图片入口


		AssetDatabase.CreateFolder(rootPath, atlas.name);//创建文件夹


		foreach (SpriteMetaData metaData in atlasInfo.spritesheet)//遍历小图集
		{
			Texture2D image = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

			//abc_0:(x:2.00, y:400.00, width:103.00, height:112.00)
			for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)//Y轴像素
			{
				for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
					image.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, atlas.GetPixel(x, y));
			}


			//转换纹理到EncodeToPNG兼容格式
			if (image.format != TextureFormat.ARGB32 && image.format != TextureFormat.RGB24)
			{
				Texture2D newTexture = new Texture2D(image.width, image.height);
				newTexture.SetPixels(image.GetPixels(0), 0);
				image = newTexture;
			}
			var pngData = image.EncodeToPNG();
			File.WriteAllBytes(rootPath + "/" + atlas.name + "/" + metaData.name + ".PNG", pngData);
			// 刷新资源窗口界面
			AssetDatabase.Refresh();
		}
	}
}