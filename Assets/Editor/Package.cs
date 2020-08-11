using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class Package
{

	private static readonly string OupPutFolder = "Bundles";
	
	[MenuItem("Package/Mark")]
	static void Mark()
	{
		MarkAssetBundleLabels("Assets/Resources");
	}

	private static uint Version = 1;

	[MenuItem("Package/Build/Android")]
	static void BuildAndroid()
	{
		var output = new DirectoryInfo(OupPutFolder);
		if (output.Exists) output.Delete(true);
		output.Create();
		var manifest = BuildPipeline.BuildAssetBundles(OupPutFolder, BuildAssetBundleOptions.None, BuildTarget.Android);
		var bundles = manifest.GetAllAssetBundles().ToList();
		bundles.Add("Bundles");
		var lines = new List<string>();
		foreach (var bundle in bundles)
		{
			var file = new FileInfo(output.FullName + "/" + bundle);
			var crc32 = CRC32.GetCRC32(File.ReadAllBytes(file.FullName)).ToString("X").ToLower();
			lines.Add(bundle + "." + crc32);

			file.MoveTo(file.FullName + "." + crc32);
		}

		var bundlesFile = new FileInfo("bundles.txt");
		File.WriteAllText(bundlesFile.FullName, string.Join("\n", lines.ToArray()));

		Debug.Log("BuildAndroid completed.");
	}

	private static void MarkAssetBundleLabels(string folder)
	{
		var assets = AssetDatabase.FindAssets(null, new[] {folder});
		foreach (var asset in assets)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(asset);
			if (AssetDatabase.IsValidFolder(assetPath))
			{
				MarkAssetBundleLabels(assetPath);
			}
			else
			{
				var relativePath = assetPath.Substring("Assets/Resources/".Length);
				string assetBundleName = relativePath.Substring(0, relativePath.LastIndexOf("."));
				var importer = AssetImporter.GetAtPath(assetPath);
				importer.SetAssetBundleNameAndVariant(assetBundleName, "ab");
			}
		}
		AssetDatabase.RemoveUnusedAssetBundleNames();
		Debug.Log(folder + ": Mark AssetBundle Labels ok.");
	}
}
